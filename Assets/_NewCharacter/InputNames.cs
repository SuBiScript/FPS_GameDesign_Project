using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Default Inputs", menuName = "TESTING/Input List")]
public class InputNames : ScriptableObject
{
    [Header("Weapon")] 
    public string Shoot = "Shoot";
    public string ChangeColor = "Change Color";

    [Header("Movement")] 
    public string HorizontalMovement = "Horizontal";
    public string ForwardMovement = "Vertical";
    public string Run = "Run";
    public string Crouch = "Crouch";
    public string Jump = "Jump";
    
    [Header("Game")] 
    public string Interact = "Interact";

    [Header("Menu")]
    public string Submit = "Submit";
    public string Cancel = "Cancel";


}