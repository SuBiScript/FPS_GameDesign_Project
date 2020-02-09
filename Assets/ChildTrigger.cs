using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColorPanels
{
    public class ChildTrigger : MonoBehaviour
    {
        private ColorPanelObject parentController;

        private void Start()
        {
            parentController = GetComponentInParent<ColorPanelObject>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject != GameController.Instance.m_PlayerComponents.PlayerController.gameObject)
                parentController.OnChildTriggerEnter(other);
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject != GameController.Instance.m_PlayerComponents.PlayerController.gameObject)
                parentController.OnChildTriggerExit(other);
        }
    }
}