using UnityEngine;

public abstract class Brain : MonoBehaviour
{
    public bool Shooting { get; protected set; }
    public bool Aiming { get; protected set; }
    public bool Jumping { get; protected set; }
    public bool Running { get; protected set; }
    public bool Interacting { get; protected set; }
    public bool Crouching { get; protected set; }
    public bool Forward { get; protected set; }
    public bool Backwards { get; protected set; }
    public bool Right { get; protected set; }
    public bool Left { get; protected set; }
    
    public Vector2 Direction { get; protected set; }

    public abstract void GetActions();
}