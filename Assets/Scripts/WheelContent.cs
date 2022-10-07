using UnityEngine;

public class WheelContent: MonoBehaviour
{
    [SerializeField] private string _rewardName;
    public string RewardName { get { return _rewardName; } set { _rewardName = value; } }

    [SerializeField] private Sprite _icon;
    public Sprite Icon { get { return _icon; } set { _icon = value; } }

    [SerializeField] private int _amount;
    public int Amount { get { return _amount; } set { _amount = value; } }
    
    [Range(0f, 100f)]
    [SerializeField] private float _chance;
    public float Chance { get { return _chance; } set { _chance = value; } }

    [SerializeField] private double _weight = 0f;
    public double Weight { get { return _weight; } set { _weight = value; } }

    [SerializeField] private GameObject _object;
    public GameObject Object { get { return _object; } }

    private void Start()
    {
        _object = gameObject;
    }
}
