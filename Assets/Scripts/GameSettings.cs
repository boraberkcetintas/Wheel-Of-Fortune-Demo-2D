using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Game Settings", menuName = "Game Settings")]
public class GameSettings : ScriptableObject
{
    public int SafeZoneLevel;
    public int SuperZoneLevel;
    public int MaxContentCountInWheel = 8; //Must be a fixed number
    public float MoveInCanvasStopLimit;
    public float MoveInCanvasLerp;

    [Header("Wheel Settings")]
    [Range(1, 15)] public int SpinDuration;
    public int WheelTurnCount;
    public float RewardPlaceHolderOffset;
    public float WaitingTimeAfterSpinCompleted;
    public DG.Tweening.Ease SpinningType;

    [Header("UI Settings")]
    public Vector2 WheelBombSpriteSize;
    public Vector2 WheelOtherSpriteSize;
    public float SpacingForEarnedPrizeBoard = -110f;

    [Header("Sound Settings")]
    public AudioClip TickAudioClip;
    [Range(0f, 1f)] public float Volume = .5f;
    [Range(-3f, 3f)] public float Pitch = 1f;

}
