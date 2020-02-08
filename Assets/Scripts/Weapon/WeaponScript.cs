using System;
using System.Runtime.CompilerServices;
using ColorPanels;
using UnityEngine;

namespace Weapon
{
    public class WeaponScript : MonoBehaviour
    {
        public enum WeaponColor
        {
            None,
            Red,
            Green,
            Blue
        }

        public MaterialList materialList;
        public MeshRenderer weaponMeshRenderer;
        [Header("Attract Settings")] public GameObject attractPoint;
        private Material _currentMaterial;

        [Header("Raycast Settings")]
        [Tooltip("Max range for the Ray Casting")]
        public float maxRange = 150f;

        public LayerMask layerMask;

        private PlayerController m_AttachedCharacter;
        private Rigidbody _attachedRigidbody;
        private WeaponColor _currentColor = WeaponColor.None;

        public void Init(PlayerController attachedCharacter)
        {
            m_AttachedCharacter = attachedCharacter;
            _attachedRigidbody = null;
            ChangeColor(WeaponColor.Red);
            ChangeMeshRendererMaterial();
        }

        public void MainFire()
        {
            //Raycast to a target (interface) to interact and change color?
            var lRay = m_AttachedCharacter.m_AttachedCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
            if (Physics.Raycast(lRay, out var hit, maxRange, layerMask))
            {
                hit.collider.gameObject.GetComponent<ColorPanelObject>()
                    ?.ChangeColor(_currentColor, _currentMaterial);
            }
        }

        public void AltFire() => ChangeColor((int)_currentColor < 3 ? _currentColor + 1 : (WeaponColor)1);

        public void AttractObject()
        {
            if (_attachedRigidbody != null)
            {
                Debug.Log("ATTRACTING");
                ColorPanelEffects.AttractObject(_attachedRigidbody, attractPoint, 5f);
            }
            else
            {
                Debug.Log("PICKING RB");
                var lRay = m_AttachedCharacter.m_AttachedCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
                if (Physics.Raycast(lRay, out var hit, maxRange, layerMask))
                {
                    _attachedRigidbody = hit.collider.gameObject.GetComponent<Rigidbody>();
                }
            }
        }

        public void DetachObject()
        {
            if (_attachedRigidbody == null) return;
            _attachedRigidbody = null;
        }

        private void ChangeColor(WeaponColor newColor)
        {
            //Change material and play sounds?
            switch (newColor)
            {
                case WeaponColor.None:
                    _currentMaterial = materialList.defaultMaterial;
                    break;
                case WeaponColor.Red:
                    _currentMaterial = materialList.redMaterial;
                    break;
                case WeaponColor.Green:
                    _currentMaterial = materialList.greenMaterial;
                    break;
                case WeaponColor.Blue:
                    _currentMaterial = materialList.blueMaterial;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newColor), newColor, null);
            }

            _currentColor = newColor;
            ChangeMeshRendererMaterial();
        }

        private void ChangeMeshRendererMaterial()
        {
            Material[] newWeaponMaterial = weaponMeshRenderer.materials;
            newWeaponMaterial[0] = _currentMaterial;
            weaponMeshRenderer.materials = newWeaponMaterial;
        }
    }
}