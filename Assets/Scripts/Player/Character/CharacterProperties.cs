using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Default_CharacterProperties", menuName = "TESTING/Character Properties")]
public class CharacterProperties : ScriptableObject
{
    [Header("Moving speeds")]
    public float WalkSpeed = 1f;
    public float RunSpeed = 2f;
    [Tooltip("This speed should not be modified as it will change during playtime.")]
    public float TemporalPropulsionSpeed = 1f;
    [Header("Air Speeds")]
    public float AirControlSpeed = 0.5f;
    public float OnAirSpeed = 0.5f;
    [Header("Other forces")] public float JumpForce = 7f;
    [Header("Gameplay Aid Settings")] public int MaxCoyoteFrames = 10;
}
