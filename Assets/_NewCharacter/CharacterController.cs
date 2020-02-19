using PlayerFSM;
using UnityEngine;

public abstract class CharacterController : MonoBehaviour
{
    [Header("Required Components")] 
    public Brain defaultBrain;
    public State defaultState;
    public StateMachine stateMachine;
    public Rigidbody rigidbody;

    [Space(5)] [Header("Properties")]
    public CharacterProperties characterProperties;
}