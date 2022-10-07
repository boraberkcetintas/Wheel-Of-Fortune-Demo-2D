using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class WheelOfFortune : MonoBehaviour
{
    [Range(1, 15)] public int spinDuration = 5;
    [SerializeField] private GameObject earnedprize_item;
    [SerializeField] private GameObject earnedprize_dataitem;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Transform earnedprize_item_parent;
    [SerializeField] private Transform SpinningWheel;
    [SerializeField] private Button ui_text_button_spin;
    [SerializeField] private TextMeshProUGUI ui_text_button_spin_text;
    [SerializeField] private TextMeshProUGUI ui_text_level_value;
    [SerializeField] private TextMeshProUGUI ui_text_nextsafezone_value;
    [SerializeField] private TextMeshProUGUI ui_text_nextsuperzone_value;
    [Space]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip tickAudioClip;
    [SerializeField] [Range(0f, 1f)] private float volume = .5f;
    [SerializeField] [Range(-3f, 3f)] private float pitch = 1f;
    [Space]
    [SerializeField] private RewardType[] rewardPool;
    [SerializeField] private RewardType[] rewardPoolForSuperZone;
    [SerializeField] private Transform[] rewardPlaceHolder;

    public WheelContent[] wheelContent = new WheelContent[8];
    public List<WheelContent> earnedPrizes = new();


    private bool isSpinning = false;
    private double accumulatedWeight = 0;
    private int maxIndex = 8; //Çarkda 8 boş alan olduğundan.
    [HideInInspector] public int level = 1;
    private System.Random randomFromSystem = new System.Random();
    private int nextSafeZone = 5;
    private int nextSuperZone = 30;
    GameObject spawnObject;
    [HideInInspector] public int index;
    void Start()
    {
        ui_text_nextsafezone_value.text = "Safezone " + nextSafeZone;
        ui_text_nextsuperzone_value.text = "Superzone " + nextSuperZone;
        FillWheelWithRewards();
        SetupAudio();
        ui_text_level_value.text = level.ToString();
    }


    public void Spin()
    {
        if (!isSpinning)
        {
            isSpinning = true;
            ui_text_button_spin.interactable = false;
            ui_text_button_spin_text.text = "Spinning";

            index = GetRandomReward();
            float angle = -(45 * index);
            float rightOffset = (angle - 13f) % 360;
            float leftOffset = (angle + 13) % 360;
            float randomAngle = Random.Range(leftOffset, rightOffset);

            Vector3 targetRotation = Vector3.back * (randomAngle + (2 * 360 * spinDuration));

            float prevAngle;
            prevAngle = SpinningWheel.eulerAngles.z;
            float sumDeltaRotationZ = 0;

            SpinningWheel
                .DORotate(targetRotation, spinDuration, RotateMode.Fast)
                .SetEase(Ease.InOutQuart)
                .OnUpdate(() =>
                {

                    float deltaRotationZ = Mathf.Abs(SpinningWheel.eulerAngles.z - prevAngle);
                    sumDeltaRotationZ = +deltaRotationZ;
                    if (sumDeltaRotationZ >= 45f)
                    {
                        sumDeltaRotationZ = 0;
                        audioSource.PlayOneShot(audioSource.clip);
                    }
                    prevAngle = SpinningWheel.eulerAngles.z;

                })
                .OnComplete(() =>
                {
                    StartCoroutine(SpinCompleted(1));
                });
        }
    }

    private void FillWheelWithRewards()
    {

        if (!(level % 30 == 0))
        {
            GetRandomRewardsFromPool();
        }
        else
        {
            nextSuperZone += 30;
            GetRandomRewardsFromPoolforSuperZone();
        }

        for (int i = 0; i < wheelContent.Length; i++)
        {
            if (wheelContent[i].RewardName == "Bomb")
            {
                rewardPlaceHolder[i].GetChild(1).GetComponent<TextMeshProUGUI>().text = " ";
                rewardPlaceHolder[i].GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(75, 75);
            }
            else
            {
                rewardPlaceHolder[i].GetChild(1).GetComponent<TextMeshProUGUI>().text = wheelContent[i].Amount.ToString();
                rewardPlaceHolder[i].GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(45, 45);
            }

            rewardPlaceHolder[i].GetChild(0).GetComponent<Image>().sprite = wheelContent[i].Icon;

        }

    }

    private void GetRandomRewardsFromPool()
    {
        List<int> LastRandoms = new();
        for (int i = 0; i < maxIndex; i++)
        {

            int randomRewardPoolIndex = Random.Range(1, rewardPool.Length);


            if (level % 5 == 0 || level == 1)
            {
                randomRewardPoolIndex = Random.Range(1, rewardPool.Length); //Bomba mutlaka 0.indeksde olmalıdır.
            }
            else
            {
                randomRewardPoolIndex = 0;
            }


            while (LastRandoms.Contains(randomRewardPoolIndex)) // Ödüllerin aynı çarkda tekrar etmemesini sağlıyor.
            {
                randomRewardPoolIndex = Random.Range(1, rewardPool.Length);
            }


            LastRandoms.Add(randomRewardPoolIndex);

            wheelContent[i].RewardName = rewardPool[randomRewardPoolIndex].RewardName;
            wheelContent[i].Icon = rewardPool[randomRewardPoolIndex].Icon;
            wheelContent[i].Amount = rewardPool[randomRewardPoolIndex].Amount;
            wheelContent[i].Chance = rewardPool[randomRewardPoolIndex].Chance;

        }
    }

    private void GetRandomRewardsFromPoolforSuperZone()
    {
        for (int i = 0; i < maxIndex; i++)
        {
            int randomRewardPoolIndex = Random.Range(0, rewardPoolForSuperZone.Length);

            wheelContent[i].RewardName = rewardPoolForSuperZone[randomRewardPoolIndex].RewardName;
            wheelContent[i].Icon = rewardPoolForSuperZone[randomRewardPoolIndex].Icon;
            wheelContent[i].Amount = rewardPoolForSuperZone[randomRewardPoolIndex].Amount;
            wheelContent[i].Chance = rewardPoolForSuperZone[randomRewardPoolIndex].Chance;
            //Havuza aynı ödüller gelmemesi için kontrol işlemi ekle;
        }
    }


    private int GetRandomReward()
    {
        CalculateWeights();
        double r = randomFromSystem.NextDouble() * accumulatedWeight;
        Debug.Log("Seçilen ağırlık " + r);

        for (int j = 0; j < wheelContent.Length; j++)
        {
            if (r <= wheelContent[j].weight)
            {
                return j;
            }

        }
        return 0;
    }

    void CalculateWeights()
    {
        for (int i = 0; i < wheelContent.Length; i++)
        {
            wheelContent[i].weight = 0f;
        }
        accumulatedWeight = 0;

        for (int i = 0; i < wheelContent.Length; i++)
        {
            accumulatedWeight += wheelContent[i].Chance; //Toplam ağırlığı hesapla.
            wheelContent[i].weight = accumulatedWeight; // Toplam ağırlığı ödüle ata.
            Debug.Log(wheelContent[i].RewardName + " " + wheelContent[i].weight);
            //Chance sıfır olanları dahil etmeme kodu yaz.
        }
    }

    private void SetupAudio()
    {
        audioSource.clip = tickAudioClip;
        audioSource.volume = volume;
        audioSource.pitch = pitch;
    }

    IEnumerator SpinCompleted(float time)
    {
        if (wheelContent[index].RewardName == "Bomb")
        {
            //Gameover
            GameOver();
        }
        else
        {
            UpdateEarnedPrizes(index);

            yield return new WaitForSeconds(time);


            ui_text_button_spin.interactable = true;
            ui_text_button_spin_text.text = "Spin";
            isSpinning = false;
            level++;
            if (level % 5 == 0)
            {
                nextSafeZone += 5;
            }
            ui_text_level_value.text = level.ToString();
            ui_text_nextsafezone_value.text = "Safezone " + nextSafeZone;
            ui_text_nextsuperzone_value.text = "Superzone " + nextSuperZone;
            FillWheelWithRewards();
        }

    }

    bool itemExist;
    int j = 0;
    GameObject item;
    private void UpdateEarnedPrizes(int index)
    {
        itemExist = false;
        WheelContent wheelItem = wheelContent[index];
        for (int i = 0; i < earnedPrizes.Count; i++)
        {
            if (earnedPrizes[i].name == wheelItem.RewardName)
            {
                itemExist = true;
                earnedPrizes[i].Amount = earnedPrizes[i].Amount + wheelItem.Amount;
                Debug.Log(earnedPrizes[i].RewardName + " miktarı " + earnedPrizes[i].Amount);
                if (item != null)
                {
                    for (int x = 0; x < earnedprize_item_parent.childCount; x++)
                    {
                        if (earnedprize_item_parent.GetChild(x).transform.GetChild(0).GetComponent<Image>().sprite == earnedPrizes[i].Icon)
                        {
                            earnedprize_item_parent.transform.GetChild(x).transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = earnedPrizes[i].Amount.ToString();

                            spawnObject = Instantiate(wheelItem.obje);
                            spawnObject.transform.parent = transform;
                            spawnObject.transform.position = wheelItem.obje.transform.position;
                            spawnObject.transform.rotation = wheelItem.obje.transform.rotation;
                            StartCoroutine(MoveInCanvas(spawnObject, earnedprize_item_parent.transform.GetChild(x).gameObject));
                        }
                    }
                }
                break;
            }
        }

        if (itemExist == false)
        {
            earnedPrizes.Add(InstantiateDataItem().GetComponent<WheelContent>());
            earnedPrizes[j].RewardName = wheelItem.RewardName;
            earnedPrizes[j].Amount = wheelItem.Amount;
            earnedPrizes[j].Icon = wheelItem.Icon;
            earnedPrizes[j].name = wheelItem.RewardName;

            item = InstantiateEPItem();

            item.transform.GetChild(0).GetComponent<Image>().sprite = earnedPrizes[j].Icon;
            item.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = earnedPrizes[j].Amount.ToString();

            Debug.Log(earnedPrizes[j].RewardName + " listeye eklendi.");
            item.SetActive(false);

            spawnObject = Instantiate(wheelItem.obje);
            spawnObject.transform.parent = transform;
            spawnObject.transform.position = wheelItem.obje.transform.position;
            spawnObject.transform.rotation = wheelItem.obje.transform.rotation;
            StartCoroutine(MoveInCanvas(spawnObject, item));

            j++;
        }


    }
    private GameObject InstantiateDataItem()
    {
        return Instantiate(earnedprize_dataitem);
    }

    float posY = -55;
    private GameObject InstantiateEPItem()
    {
        GameObject item = Instantiate(earnedprize_item, earnedprize_item_parent);

        RectTransform rectTransform = item.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y + posY);
        posY += -55;

        return item;
    }

    public void GameOver()
    {
        earnedprize_item_parent.parent.transform.SetParent(gameOverPanel.transform);
        gameOverPanel.SetActive(true);
        if (wheelContent[index].RewardName == "Bomb")
        {
            gameOverPanel.transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            gameOverPanel.transform.GetChild(1).gameObject.SetActive(true);
        }
    }

    IEnumerator MoveInCanvas(GameObject icon, GameObject target)
    {
        icon.transform.position = Vector3.Lerp(icon.transform.position, target.transform.position, Time.deltaTime * 12f);

        yield return null;

        if (Mathf.Abs(Vector3.Distance(target.transform.position, icon.transform.position)) > 0.2f)
        {
            StartCoroutine(MoveInCanvas(icon, target));
        }
        else
        {
            Destroy(icon);
            target.SetActive(true);
        }
    }
}
