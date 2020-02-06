using System;
using UnityEngine;

namespace Prototyping.Jordi
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

        [Header("Raycast Settings")] [Tooltip("Max range for the Ray Casting")]
        public float maxRange = 50f;

        public LayerMask layerMask;

        private PlayerController m_AttachedCharacter;
        private WeaponColor _currentColor = WeaponColor.None;

        public void Init(PlayerController attachedCharacter)
        {
            m_AttachedCharacter = attachedCharacter;
            ChangeColor(WeaponColor.Red);
        }

        public void MainFire()
        {
            //Raycast to a target (interface) to interact and change color?
            var lRay = m_AttachedCharacter.m_AttachedCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
            if (Physics.Raycast(lRay, out var hit, maxRange, layerMask))
            {
                hit.collider.gameObject.GetComponent<ColorPanelController>()?.ChangeColor(_currentColor);
                //Debug.DrawRay(lRay.origin, hit.point, Color.green, 3f);
            }
        }

        public void AltFire()
        {
            ChangeColor((int) _currentColor < 3 ? _currentColor + 1 : (WeaponColor) 1);
        }

        private void ChangeColor(WeaponColor newColor)
        {
            //Change material and play sounds?
            _currentColor = newColor;
        }
    }
}