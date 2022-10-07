using UnityEngine;

public class WheelContent: MonoBehaviour
{
    public double weight = 0f;
    public GameObject obje;
    public string RewardName;
    public Sprite Icon;
    public int Amount;

    [Range(0f, 100f)]
    public float Chance = 100;


    private void Start()
    {
        obje = gameObject;
    }
}
