using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Game Settings", menuName = "Game Settings")]
public class GameSettings : ScriptableObject
{
    public int SafeZoneLevel;
    public int SuperZoneLevel;
    public int MaxContentCountInWheel;
    [Header("Wheel Settings")]
    [Range(1, 15)] public int SpinDuration;
    public int WheelTurnCount;
    public float RewardPlaceHolderOffset;

}
