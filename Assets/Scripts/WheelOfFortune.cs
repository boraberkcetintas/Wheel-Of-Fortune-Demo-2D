using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class WheelOfFortune : MonoBehaviour
{
    [SerializeField] private GameSettings _gameSettings;
    [SerializeField] private WheelSprites _wheelSprites;
    [SerializeField] private Image image_spinning_wheel;
    [SerializeField] private Image Image_spinning_wheel_indicator;
    [Space]
    [SerializeField] private GameObject earnedprize_item;
    [SerializeField] private GameObject earnedprize_dataitem;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject gameOverPanelClaimText;
    [SerializeField] private GameObject gameOverPanelLoseText;
    [Space]
    [SerializeField] private Transform earnedprize_item_parent;
    [SerializeField] private Transform SpinningWheel;
    [Space]
    [SerializeField] private Button ui_text_button_spin;
    [SerializeField] private Button ui_text_button_claim;
    [Space]
    [SerializeField] private TextMeshProUGUI ui_text_button_spin_text;
    [SerializeField] private TextMeshProUGUI ui_text_level_value;
    [SerializeField] private TextMeshProUGUI ui_text_nextsafezone_value;
    [SerializeField] private TextMeshProUGUI ui_text_nextsuperzone_value;
    [Space]
    [SerializeField] private AudioSource audioSource;
    [Space]
    [Header("Item Pools")]
    [SerializeField] private RewardType[] rewardPool;
    [SerializeField] private RewardType[] rewardPoolForSuperZone;
    [SerializeField] private Transform[] rewardPlaceHolder;
    [SerializeField] private Image[] rewardPlaceHolderImageChild;
    [SerializeField] private RectTransform[] rewardPlaceHolderImageChildRectTransform;
    [SerializeField] private TextMeshProUGUI[] rewardPlaceHolderTextChild;

    public WheelContent[] wheelContent = new WheelContent[8];
    public List<WheelContent> earnedPrizes = new();

    private bool isSpinning = false;
    private double accumulatedWeight = 0;
    private int level = 1;
    private System.Random randomFromSystem = new System.Random();
    private int nextSafeZone;
    private int nextSuperZone;
    GameObject FloatingReward;
    private int EarnedPrizeIndex;
    private float NextPositionForNextReward;
    void Start()
    {
        ui_text_button_claim.interactable = false;
        NextPositionForNextReward = _gameSettings.SpacingForEarnedPrizeBoard;
        nextSafeZone = _gameSettings.SafeZoneLevel;
        nextSuperZone = _gameSettings.SuperZoneLevel;

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
            ui_text_button_claim.interactable = false;
            ui_text_button_spin_text.text = "Spinning";

            float prevAngle;
            prevAngle = SpinningWheel.eulerAngles.z;
            float sumDeltaRotationZ = 0;

            SpinningWheel
                .DORotate(CalculateTargetRotation(), _gameSettings.SpinDuration, RotateMode.Fast)
                .SetEase(_gameSettings.SpinningType)
                .OnUpdate(() =>
                {
                    float deltaRotationZ = Mathf.Abs(SpinningWheel.eulerAngles.z - prevAngle);
                    sumDeltaRotationZ = +deltaRotationZ;
                    if (sumDeltaRotationZ >= 360 / _gameSettings.MaxContentCountInWheel)
                    {
                        sumDeltaRotationZ = 0;
                        audioSource.PlayOneShot(audioSource.clip);
                    }
                    prevAngle = SpinningWheel.eulerAngles.z;
                })
                .OnComplete(() =>
                {
                    StartCoroutine(SpinCompleted(_gameSettings.WaitingTimeAfterSpinCompleted));
                });
        }
    }

    private Vector3 CalculateTargetRotation()
    {
        EarnedPrizeIndex = GetRandomReward();
        float anglePerPiece = -(360 / _gameSettings.MaxContentCountInWheel * EarnedPrizeIndex);
        float RewardPlaceHolderRightOffset = (anglePerPiece - _gameSettings.RewardPlaceHolderOffset) % 360;
        float RewardPlaceHolderLeftOffset = (anglePerPiece + _gameSettings.RewardPlaceHolderOffset) % 360;
        float randomAngle = Random.Range(RewardPlaceHolderLeftOffset, RewardPlaceHolderRightOffset);

        Vector3 targetRotation = Vector3.back * (randomAngle + (_gameSettings.WheelTurnCount * 360 * _gameSettings.SpinDuration));

        return targetRotation;
    }

    private void FillWheelWithRewards()
    {

        if (!(level % _gameSettings.SuperZoneLevel == 0))
        {
            GetRandomRewardsFromPool();
        }
        else
        {
            nextSuperZone += _gameSettings.SuperZoneLevel;
            GetRandomRewardsFromPoolforSuperZone();
        }

        for (int i = 0; i < wheelContent.Length; i++)
        {
            if (wheelContent[i].RewardName == "Bomb")
            {
                rewardPlaceHolderTextChild[i].text = " ";
                rewardPlaceHolderImageChildRectTransform[i].sizeDelta = _gameSettings.WheelBombSpriteSize;
            }
            else
            {
                rewardPlaceHolderTextChild[i].text = wheelContent[i].Amount.ToString();
                rewardPlaceHolderImageChildRectTransform[i].sizeDelta = _gameSettings.WheelOtherSpriteSize;
            }
            rewardPlaceHolderImageChild[i].sprite = wheelContent[i].Icon;
        }

    }

    private void GetRandomRewardsFromPool()
    {
        List<int> LastRandoms = new();
        for (int i = 0; i < _gameSettings.MaxContentCountInWheel; i++)
        {
            int randomRewardPoolIndex;

            if (level % _gameSettings.SafeZoneLevel == 0 || level == 1)
            {
                randomRewardPoolIndex = Random.Range(1, rewardPool.Length); //Bomb must be in index zero in reward pool.
            }
            else
            {
                randomRewardPoolIndex = 0;
            }


            while (LastRandoms.Contains(randomRewardPoolIndex)) // It ensures that the rewards do not repeat on the same wheel.
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
        for (int i = 0; i < _gameSettings.MaxContentCountInWheel; i++)
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
            if (r <= wheelContent[j].Weight)
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
            wheelContent[i].Weight = 0f;
        }
        accumulatedWeight = 0; // Set it zero for next calculation.

        for (int i = 0; i < wheelContent.Length; i++)
        {
            accumulatedWeight += wheelContent[i].Chance; //Calculate accumulated weight.
            wheelContent[i].Weight = accumulatedWeight; // Set accumulated weight to an item.
            Debug.Log(wheelContent[i].RewardName + " " + wheelContent[i].Weight);
        }
    }

    private void SetupAudio()
    {
        audioSource.clip = _gameSettings.TickAudioClip;
        audioSource.volume = _gameSettings.Volume;
        audioSource.pitch = _gameSettings.Pitch;
    }

    IEnumerator SpinCompleted(float time)
    {
        if (wheelContent[EarnedPrizeIndex].RewardName == "Bomb")
        {
            GameOver();
        }
        else
        {
            UpdateEarnedPrizes(EarnedPrizeIndex);

            yield return new WaitForSeconds(time);


            ui_text_button_spin.interactable = true;
            ui_text_button_claim.interactable = true;
            ui_text_button_spin_text.text = "Spin";
            isSpinning = false;
            level++;
            if (level % _gameSettings.SafeZoneLevel == 0)
            {
                nextSafeZone += _gameSettings.SafeZoneLevel;
            }
            ui_text_level_value.text = level.ToString();
            ui_text_nextsafezone_value.text = "Safezone " + nextSafeZone;
            ui_text_nextsuperzone_value.text = "Superzone " + nextSuperZone;
            FillWheelWithRewards();
            UpdateWheelSprite();
        }

    }

    private bool itemExist;
    private int itemIndex = 0;
    GameObject item;

    // Updates existing rewards (amounts) or calls another method to create new one.
    private void UpdateEarnedPrizes(int index)
    {
        itemExist = false;
        WheelContent wheelItem = wheelContent[index];
        for (int i = 0; i < earnedPrizes.Count; i++)
        {
            if (earnedPrizes[i].name == wheelItem.RewardName)
            {
                itemExist = true;
                earnedPrizes[i].Amount += wheelItem.Amount;
                Debug.Log(earnedPrizes[i].RewardName + " miktarı " + earnedPrizes[i].Amount);
                if (item != null)
                {
                    for (int x = 0; x < earnedprize_item_parent.childCount; x++)
                    {
                        if (earnedprize_item_parent.GetChild(x).transform.GetChild(0).GetComponent<Image>().sprite == earnedPrizes[i].Icon)
                        {
                            earnedprize_item_parent.transform.GetChild(x).transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = earnedPrizes[i].Amount.ToString();

                            FloatingReward = Instantiate(wheelItem.Object);
                            FloatingReward.transform.parent = transform;
                            FloatingReward.transform.position = wheelItem.Object.transform.position;
                            FloatingReward.transform.rotation = wheelItem.Object.transform.rotation;
                            StartCoroutine(MoveInCanvas(FloatingReward, earnedprize_item_parent.transform.GetChild(x).gameObject));
                        }
                    }
                }
                break;
            }
        }

        if (itemExist == false)
        {
            earnedPrizes.Add(InstantiateDataItem().GetComponent<WheelContent>());
            earnedPrizes[itemIndex].RewardName = wheelItem.RewardName;
            earnedPrizes[itemIndex].Amount = wheelItem.Amount;
            earnedPrizes[itemIndex].Icon = wheelItem.Icon;
            earnedPrizes[itemIndex].name = wheelItem.RewardName;

            item = InstantiateRewardInEarnedPrizeBoard();

            item.transform.GetChild(0).GetComponent<Image>().sprite = earnedPrizes[itemIndex].Icon;
            item.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = earnedPrizes[itemIndex].Amount.ToString();

            Debug.Log(earnedPrizes[itemIndex].RewardName + " listeye eklendi.");
            item.SetActive(false);

            FloatingReward = Instantiate(wheelItem.Object);
            FloatingReward.transform.parent = transform;
            FloatingReward.transform.position = wheelItem.Object.transform.position;
            FloatingReward.transform.rotation = wheelItem.Object.transform.rotation;
            StartCoroutine(MoveInCanvas(FloatingReward, item));

            itemIndex++;
        }


    }

    //Creates a new data item that holds data for rewards.
    private GameObject InstantiateDataItem()
    {
        return Instantiate(earnedprize_dataitem);
    }


    // Creates a new reward item for earned prize board.
    private GameObject InstantiateRewardInEarnedPrizeBoard()
    {
        GameObject item = Instantiate(earnedprize_item, earnedprize_item_parent);

        RectTransform rectTransform = item.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y + NextPositionForNextReward);
        NextPositionForNextReward += _gameSettings.SpacingForEarnedPrizeBoard;

        return item;
    }

    // Game overs for two situations. Player claims rewards or gets bomb from wheel.
    public void GameOver()
    {
        earnedprize_item_parent.parent.transform.SetParent(gameOverPanel.transform);
        gameOverPanel.SetActive(true);
        if (wheelContent[EarnedPrizeIndex].RewardName == "Bomb")
        {
           gameOverPanelLoseText.SetActive(true);
        }
        else
        {
            gameOverPanelClaimText.SetActive(true);
        }
    }


    // Make icons move to the reward board from the wheel.
    IEnumerator MoveInCanvas(GameObject icon, GameObject target)
    {
        icon.transform.position = Vector3.Lerp(icon.transform.position, target.transform.position, Time.deltaTime * _gameSettings.MoveInCanvasLerp);

        yield return null;

        if (Mathf.Abs(Vector3.Distance(target.transform.position, icon.transform.position)) > _gameSettings.MoveInCanvasStopLimit)
        {
            StartCoroutine(MoveInCanvas(icon, target));
        }
        else
        {
            Destroy(icon);
            target.SetActive(true);
        }
    }

    // Updates the sprite of the wheel for next level.
    private void UpdateWheelSprite()
    {
        if (level % _gameSettings.SuperZoneLevel == 0)
        {
            image_spinning_wheel.sprite = _wheelSprites.WheelGoldenBase;
            Image_spinning_wheel_indicator.sprite = _wheelSprites.WheelGoldenIndicator;

        }
        else if (level % _gameSettings.SafeZoneLevel == 0)
        {
            image_spinning_wheel.sprite = _wheelSprites.WheelSilverBase;
            Image_spinning_wheel_indicator.sprite = _wheelSprites.WheelSilverIndicator;
        }
        else
        {
            image_spinning_wheel.sprite = _wheelSprites.WheelBronzeBase;
            Image_spinning_wheel_indicator.sprite = _wheelSprites.WheelBronzeIndicator;
        }
    }
}