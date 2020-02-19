using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
            try
            {
                other.gameObject.GetComponent<PlayerControllerFSM>();
                parentController.OnChildTriggerEnter(other);
            }
            catch (NullReferenceException)
            {
            }

            //if (other.gameObject != GameController.Instance.playerComponents.PlayerController.gameObject)
        }

        private void OnTriggerExit(Collider other)
        {
            try
            {
                other.gameObject.GetComponent<PlayerControllerFSM>();
                parentController.OnChildTriggerExit(other);
            }
            catch (NullReferenceException)
            {
            }

            //if (other.gameObject != GameController.Instance.playerComponents.PlayerController.gameObject)
        }
    }
}