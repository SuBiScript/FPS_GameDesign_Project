using UnityEngine;
using UnityEngine.Events;

namespace Interfaces
{
    public interface IAttachable
    {
        Rigidbody ownRigidbody { get; set; }
        void Attach(Rigidbody rb);
        void Detach();
        // void ChangeLayers(string newLayer);
        UnityEvent OnAttachOverride { get; set; }
    }
}
