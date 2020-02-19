using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Default_CharacterProperties", menuName = "TESTING/Character Properties")]
public class CharacterProperties : ScriptableObject
{
    public float WalkSpeed = 1f;
    public float RunSpeed = 2f;
    public float JumpForce = 7f;
    
}
