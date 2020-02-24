using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Default_CharacterProperties", menuName = "TESTING/Character Properties")]
public class CharacterProperties : ScriptableObject
{
    public float WalkSpeed = 1f;
    public float AirControlSpeed = 0.5f;
    public float RunSpeed = 2f;
    public float OnAirSpeed = 0.5f;
    public float JumpForce = 7f;

    public int MaxCoyoteFrames = 10;

}
