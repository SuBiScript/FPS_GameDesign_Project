using System;
using Weapon;
using UnityEngine;

namespace ColorPanels
{
    public class ColorPanelObject : MonoBehaviour
    {
        //float m_JumpForce = 10.0f;
        public Material defaultMaterial;
        public WeaponScript.WeaponColor currentMode;
        public MeshRenderer meshRenderer;
        public GameObject dragPosition;
        private Rigidbody attachedObjectRigidbody;

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


        private void OnTriggerExit(Collider other) => DetachObject(other.GetComponent<Rigidbody>());

        private void OnCollisionEnter(Collision other) => InteractTwo(other);

        private void InteractTwo(Collision other) //OnCollision
        {
            switch (currentMode)
            {
                case WeaponScript.WeaponColor.None:
                    break;
                case WeaponScript.WeaponColor.Red:
                    break;
                case WeaponScript.WeaponColor.Green:
                    break;
                case WeaponScript.WeaponColor.Blue:
                    ColorPanelEffects.ThrowObject(this.gameObject, other, transform.up);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        public void OnTriggerDelegateEnter(Collider collider) => Interact(collider);

        private void Interact(Collider collider) //OnTrigger
        {
            switch (currentMode)
            {
                case WeaponScript.WeaponColor.None:
                    break;
                case WeaponScript.WeaponColor.Red:
                    if (attachedObjectRigidbody == null)
                    {
                        attachedObjectRigidbody = collider.GetComponent<Rigidbody>();
                        attachedObjectRigidbody.useGravity = false;
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
//ChangColorZone
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

        public void DetachObject(Rigidbody other) //DetachObject
        {
            if (attachedObjectRigidbody == null || other != attachedObjectRigidbody) return;
            
            attachedObjectRigidbody.useGravity = true;
            attachedObjectRigidbody = null;
        }
    }
}