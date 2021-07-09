using Panels;
using UnityEngine;

public class ChildTrigger : MonoBehaviour
{
    public bool OnTriggerEnterEnabled = true;
    public bool OnTriggerExitEnabled = true;
    private Panel parentController;

    private void Start()
    {
        parentController = GetComponentInParent<Panel>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!OnTriggerEnterEnabled) return;
        parentController.OnChildTriggerEnter(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!OnTriggerExitEnabled) return;
        parentController.OnChildTriggerExit(other);
    }
}