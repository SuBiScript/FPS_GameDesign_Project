using System;
using Weapon;
using UnityEngine;

namespace ColorPanels
{
    public class ColorPanelObject : MonoBehaviour
    {
        private WeaponScript.WeaponColor currentMode { get; set; }
        [Header("Basic Settings")] public ColorPanelProperties colorPanelProperties;
        public MeshRenderer meshRenderer;

        [Header("Magnet Mode Settings")] public GameObject dragPosition;
        private Rigidbody _attachedObjectRigidbody;

        void Start()
        {
            //meshRenderer = GetComponent<MeshRenderer>();
            _attachedObjectRigidbody = null;
            var temp = meshRenderer.materials;
            temp[1] = colorPanelProperties.materialList.defaultMaterial;
            meshRenderer.sharedMaterials = temp;
            ChangeColor(WeaponScript.WeaponColor.None, colorPanelProperties.materialList.defaultMaterial);
        }

        void Update()
        {
            switch (currentMode)
            {
                case WeaponScript.WeaponColor.None:
                    break;
                case WeaponScript.WeaponColor.Red:

                    break;
                case WeaponScript.WeaponColor.Green:
                    try
                    {
                        ColorPanelEffects.AttractObject(_attachedObjectRigidbody, dragPosition);
                    }
                    catch (NullReferenceException)
                    {
                    }
                    break;
                case WeaponScript.WeaponColor.Blue:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        private void OnCollisionEnter(Collision other) => OnCollisionInteract(other);

        private void OnCollisionInteract(Collision other) //OnCollision
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
                    ColorPanelEffects.ThrowObject(this.gameObject, other, transform.up, colorPanelProperties);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void OnChildTriggerEnter(Collider objectCollider) => OnTriggerInteract(objectCollider);

        private void OnTriggerInteract(Collider collidedCollider) //OnTrigger
        {
            switch (currentMode)
            {
                case WeaponScript.WeaponColor.None:
                    break;
                case WeaponScript.WeaponColor.Red:
                    break;
                case WeaponScript.WeaponColor.Green:
                    if (_attachedObjectRigidbody == null)
                    {
                        _attachedObjectRigidbody = collidedCollider.GetComponent<Rigidbody>();
                        _attachedObjectRigidbody.useGravity = false;
                    }
                    break;
                case WeaponScript.WeaponColor.Blue:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void OnChildTriggerExit(Collider other) => DetachObject(other.GetComponent<Rigidbody>());

        public bool ChangeColor(WeaponScript.WeaponColor color, Material material)
        {
            //Here you change the weapon material of the block and stuff.
            if (currentMode == color) return false;

            DetachObject(_attachedObjectRigidbody);
            currentMode = color;
            var temp = meshRenderer.materials;
            temp[1] = material;
            meshRenderer.materials = temp;
            return true;
        }

        private void DetachObject(Rigidbody other) //DetachObject
        {
            try
            {
                if (other != _attachedObjectRigidbody) return;

                _attachedObjectRigidbody.useGravity = true;
                _attachedObjectRigidbody = null;
            }
            catch (NullReferenceException)
            {
            }
        }
    }
}