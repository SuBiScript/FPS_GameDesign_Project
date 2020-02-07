using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColorPanels
{
    public class TriggerDelegate : MonoBehaviour
    {
        private ColorPanelObject parentController;

        private void Start()
        {
            parentController = GetComponentInParent<ColorPanelObject>();
        }

        private void OnTriggerEnter(Collider other)
        {
            parentController.OnTriggerDelegateEnter(other);
        }
    }
}