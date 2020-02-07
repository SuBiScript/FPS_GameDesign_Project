using System;
using JetBrains.Annotations;
using Weapon;
using UnityEngine;

namespace ColorPanels
{
    public class ColorPanelController : MonoBehaviour
    {
        //float m_JumpForce = 10.0f;
        public Material defaultMaterial;
        public WeaponScript.WeaponColor currentMode;
        public MeshRenderer meshRenderer;
        public GameObject dragPosition;
        [CanBeNull] private Rigidbody attachedObjectRigidbody;

        void Start()
        {
            //meshRenderer = GetComponent<MeshRenderer>();
            attachedObjectRigidbody = null;
            var temp = meshRenderer.materials;
            temp[1] = defaultMaterial;
            meshRenderer.sharedMaterials = temp;
            ChangeColor(WeaponScript.WeaponColor.None, defaultMaterial);
        }

        void Update()
        {
            switch (currentMode)
            {
                case WeaponScript.WeaponColor.None:
                    break;
                case WeaponScript.WeaponColor.Red:
                    if (attachedObjectRigidbody != null)
                    {
                        ColorPanelEffects.AttractObject(attachedObjectRigidbody, dragPosition);
                    }

                    break;
                case WeaponScript.WeaponColor.Green:
                    break;
                case WeaponScript.WeaponColor.Blue:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void OnTriggerDelegateEnter(Collider collider) => Interact(collider);

        private void OnTriggerExit(Collider other) => DetachObject(other.GetComponent<Rigidbody>());

        private void Interact(Collider collider)
        {
            switch (currentMode)
            {
                case WeaponScript.WeaponColor.None:
                    break;
                case WeaponScript.WeaponColor.Red:
                    //Debug.Log("RED Activated!");
                    if (attachedObjectRigidbody == null)
                    {
                        attachedObjectRigidbody = collider.GetComponent<Rigidbody>();
                        attachedObjectRigidbody.useGravity = false;
                    }
                    break;
                case WeaponScript.WeaponColor.Green:
                    Debug.Log("GREEN Activated!");
                    break;
                case WeaponScript.WeaponColor.Blue:
                    ColorPanelEffects.ThrowObject(this.gameObject, collider, transform.up);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public bool ChangeColor(WeaponScript.WeaponColor color, Material material)
        {
            //Here you change the weapon material of the block and stuff.
            if (currentMode == color) return false;

            DetachObject(attachedObjectRigidbody);
            currentMode = color;
            var temp = meshRenderer.materials;
            temp[1] = material;
            meshRenderer.materials = temp;
            return true;
        }

        public void DetachObject(Rigidbody other)
        {
            if (attachedObjectRigidbody == null || other != attachedObjectRigidbody) return;
            attachedObjectRigidbody.useGravity = true;
            attachedObjectRigidbody = null;
        }
    }
}