using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Wheel Sprites", menuName = "Wheel Sprites")]
public class WheelSprites : ScriptableObject
{
    [SerializeField] private Sprite _wheelGoldenBase;
    public Sprite WheelGoldenBase { get { return _wheelGoldenBase; } }
    
    [SerializeField] private Sprite _wheelSilverBase;
    public Sprite WheelSilverBase { get { return _wheelSilverBase; } }
    
    [SerializeField] private Sprite _wheelBronzeBase;
    public Sprite WheelBronzeBase { get { return _wheelBronzeBase; } }

    [SerializeField] private Sprite _wheelGoldenIndicator;
    public Sprite WheelGoldenIndicator { get { return _wheelGoldenIndicator; } }
    
    [SerializeField] private Sprite _wheelSilverIndicator;
    public Sprite WheelSilverIndicator { get { return _wheelSilverIndicator; } }
    
    [SerializeField] private Sprite _wheelBronzeIndicator;
    public Sprite WheelBronzeIndicator { get { return _wheelBronzeIndicator; } }
}
