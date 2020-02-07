using System;
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

        [Header("Materials")] public Material defaultMaterial;
        public Material redMaterial;
        public Material greenMaterial;
        public Material blueMaterial;
        private Material _currentMaterial;

        [Header("Raycast Settings")] [Tooltip("Max range for the Ray Casting")]
        public float maxRange = 150f;

        public LayerMask layerMask;

        private PlayerController m_AttachedCharacter;
        private WeaponColor _currentColor = WeaponColor.None;

        public void Init(PlayerController attachedCharacter)
        {
            m_AttachedCharacter = attachedCharacter;
            ChangeColor(WeaponColor.Red);
        }

        public void Something(string var)
        {
            switch (_currentColor)
            {
                case WeaponColor.None:
                    break;
                case WeaponColor.Red:
                    break;
                case WeaponColor.Green:
                    break;
                case WeaponColor.Blue:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
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

        public void AltFire() => ChangeColor((int) _currentColor < 3 ? _currentColor + 1 : (WeaponColor) 1);

        private void ChangeColor(WeaponColor newColor)
        {
            //Change material and play sounds?
            switch (newColor)
            {
                case WeaponColor.None:
                    _currentMaterial = defaultMaterial;
                    break;
                case WeaponColor.Red:
                    _currentMaterial = redMaterial;
                    break;
                case WeaponColor.Green:
                    _currentMaterial = greenMaterial;
                    break;
                case WeaponColor.Blue:
                    _currentMaterial = blueMaterial;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newColor), newColor, null);
            }

            _currentColor = newColor;
        }
    }
}