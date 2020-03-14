using UnityEngine;

public interface IAttachable
{
    Rigidbody ownRigidbody { get; set; }
    void Attach();
    void Detach();
}
