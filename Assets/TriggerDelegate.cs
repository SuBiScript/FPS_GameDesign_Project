using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColorPanels
{
    public class TriggerDelegate : MonoBehaviour
    {
        private ColorPanelController parentController;

        private void Start()
        {
            parentController = GetComponentInParent<ColorPanelController>();
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("Delegate On TriggerEnter");
            parentController.OnTriggerDelegateEnter(other);
        }
    }
}