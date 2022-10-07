using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Reward Type", menuName = "Reward Type")]
public class RewardType : ScriptableObject
{
    [SerializeField] private string _rewardName;
    public string RewardName { get { return _rewardName; } }

    [SerializeField] private Sprite _icon;
    public Sprite Icon { get { return _icon; } }

    [SerializeField] private int _amount;
    public int Amount { get { return _amount; } }

    [Range(0f, 100f)]
    [SerializeField] private float _chance;
    public float Chance { get { return _chance; } }
}
