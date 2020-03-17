using UnityEngine;

namespace ColorPanels
{
    public class ChildTrigger : MonoBehaviour
    {
        private ColorPanelObjectFSM parentController;

        private void Start()
        {
            parentController = GetComponentInParent<ColorPanelObjectFSM>();
        }

        private void OnTriggerEnter(Collider other)
        {
            parentController.OnChildTriggerEnter(other);
        }

        private void OnTriggerExit(Collider other)
        {
            parentController.OnChildTriggerExit(other);
        }
    }
}