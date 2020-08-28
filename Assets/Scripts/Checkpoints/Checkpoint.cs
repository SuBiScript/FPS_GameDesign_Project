using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class Checkpoint : MonoBehaviour
{
    [Header("Configuration")]
    public byte ID;
    public Transform respawnTransform;
    public Transform synchedRoom;
    
    [Space(10)]
    public UnityEvent OnCheckpointActivation;
    private new Collider collider;

    private void Awake()
    {
        collider = GetComponent<Collider>();
        collider.enabled = true;
        collider.isTrigger = true;
        if (!CheckpointManager.RegisterCheckpoint(this))
        {
            Debug.LogWarning($"Checkpoint {this.gameObject.name.ToString()} not registered!");
        }

        gameObject.name = "Checkpoint_" + ID + "_" + (synchedRoom!=null ? synchedRoom.gameObject.name : "");
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.GetComponent<PlayerControllerFSM>() != null)
        {
            if (CheckpointManager.SetNewCheckpoint(this))
            {
                OnCheckpointActivation.Invoke();
                this.SetCheckpoint(true);
            }
        }
    }

    /// <summary>
    /// Enables or disables the ability to save to this checkpoint
    /// </summary>
    /// <param name="disable">Disable or not the checkpoint</param>
    public void SetCheckpoint(bool disable)
    {
        this.collider.enabled = !disable;
    }

    public Vector3 GetPosition()
    {
        return respawnTransform.position;
    }
}
