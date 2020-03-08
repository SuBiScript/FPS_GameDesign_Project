using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class Checkpoint : MonoBehaviour
{
    private Collider collider;

    public Transform respawnPosition;
    public UnityEvent checkpointActivated;

    private void Awake()
    {
        collider = GetComponent<Collider>();
        collider.enabled = true;
        collider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.GetComponent<PlayerControllerFSM>() != null)
        {
            if (CheckpointManager.SetNewCheckpoint(this))
            {
                checkpointActivated.Invoke();
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
}
