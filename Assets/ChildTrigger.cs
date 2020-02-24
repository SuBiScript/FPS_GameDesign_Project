using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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