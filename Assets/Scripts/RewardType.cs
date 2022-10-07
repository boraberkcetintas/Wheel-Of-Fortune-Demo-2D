using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Reward Type", menuName = "Reward Type")]
public class RewardType : ScriptableObject
{
    [HideInInspector] public double weight = 0f;

    public string RewardName;
    public Sprite Icon;
    public int Amount;

    [Range(0f, 100f)]
    public float Chance = 100;

}
