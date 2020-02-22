using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using ColorPanels;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Weapon
{
    public class WeaponScript : MonoBehaviour
    {
        [System.Serializable]
        public class ObjectAttacher
        {
            [Header("Attaching Object")] public bool m_AttachingObject;
            public Rigidbody m_ObjectAttached;
            public float m_AttachingObjectSpeed;
            public GameObject m_AttachingPosition;

            public ObjectAttacher()
            {
                m_ObjectAttached = null;
                m_AttachingObject = m_ObjectAttached != null;
            }

            public void UpdateAttachedObject()
            {
                if (m_ObjectAttached == null) return;
                //m_ObjectAttached.gameObject.transform.parent = m_Parent;
                if (m_AttachingObject)
                {
                    ColorPanelEffects.UpdateAttachedObject(m_ObjectAttached, m_AttachingPosition,
                        m_AttachingObjectSpeed);
                }

                /*else
                {
                    m_ObjectAttached.MoveRotation(Quaternion.Euler(0.0f, l_EulerAngles.y, l_EulerAngles.z));
                    m_ObjectAttached.MovePosition(m_AttachingPosition.position);
                }*/
            }

            public void AttachObject(Rigidbody l_ObjectToAttach)
            {
                if (m_AttachingObject) return;
                m_AttachingObject = true;
                m_ObjectAttached = l_ObjectToAttach;
                m_ObjectAttached.tag = "Attached";
                m_ObjectAttached.useGravity = false;
                m_ObjectAttached.isKinematic = true;
                m_ObjectAttached.GetComponent<Collider>().isTrigger = true;
            }

            public void DetachObject(float l_DetachForce = 20f)
            {
                if (!m_AttachingObject) return;
                m_AttachingObject = false;
                m_ObjectAttached.useGravity = true;
                m_ObjectAttached.isKinematic = false;
                m_ObjectAttached.GetComponent<Collider>().isTrigger = false;
                m_ObjectAttached.AddForce(m_AttachingPosition.transform.forward * l_DetachForce, ForceMode.Impulse);
                m_ObjectAttached.tag = "Cube";
                m_ObjectAttached = null;
            }
        }

        public ObjectAttacher m_ObjectAttacher = new ObjectAttacher();

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

        private Material emissiveWithColor;
        private Material _currentMaterial;

        public Material _weaponMaterial;


        [Header("Raycast Settings")] [Tooltip("Max range for the Ray Casting")]
        public float maxRange = 150f;

        public LayerMask layerMask;

        private PlayerControllerFSM m_AttachedCharacter;
        private WeaponColor _currentColor = WeaponColor.None;

        public void Init(PlayerControllerFSM attachedCharacter)
        {
            m_AttachedCharacter = attachedCharacter;
            materialList = Instantiate(materialList);
            ChangeColor(WeaponColor.Blue);
        }

        public void MainFire()
        {
            //Raycast to a target (interface) to interact and change color?
            var lRay = m_AttachedCharacter.cameraController.attachedCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
            if (Physics.Raycast(lRay, out var hit, maxRange, layerMask))
            {
                hit.collider.gameObject.GetComponent<ColorPanelObjectFSM>()
                    ?.ChangeColor(_currentColor, _currentMaterial);
            }
        }

        public void AltFire() => ChangeColor((int) _currentColor < 3 ? _currentColor + 1 : (WeaponColor) 1);

        public void AttractObject()
        {
            if (m_ObjectAttacher.m_ObjectAttached != null)
            {
                m_ObjectAttacher.UpdateAttachedObject();
            }
            else
            {
                var lRay = m_AttachedCharacter.cameraController.attachedCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
                if (Physics.Raycast(lRay, out var hit, maxRange, layerMask))
                {
                    try
                    {
                        var rb = hit.collider.gameObject.GetComponent<Rigidbody>();
                        rb.velocity = Vector3.zero;
                        m_ObjectAttacher.AttachObject(rb);
                    }
                    catch (MissingComponentException)
                    {
                    }
                }
            }
        }

        public void DetachObject()
        {
            m_ObjectAttacher.DetachObject();
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
                    ChangeReticleColor(WeaponColor.Red);
                    _currentMaterial = materialList.redMaterial;
                    break;
                case WeaponColor.Green:
                    ChangeReticleColor(WeaponColor.Green);
                    _currentMaterial = materialList.greenMaterial;
                    break;
                case WeaponColor.Blue:
                    ChangeReticleColor(WeaponColor.Blue);
                    _currentMaterial = materialList.blueMaterial;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newColor), newColor, null);
            }

            _currentColor = newColor;
            ChangeMeshRendererMaterial();
        }

        private void ChangeReticleColor(WeaponColor color)
        {
            try
            {
                GameController.Instance.m_CanvasController.ChangeReticleColor((int)color);
            }
            catch(NullReferenceException){}
        }

        private void ChangeMeshRendererMaterial()
        {
            Material[] newWeaponMaterial = weaponMeshRenderer.materials;
            try
            {
                emissiveWithColor = new Material(materialList.emissiveMaterial);
                emissiveWithColor.SetColor("_EmissionColor", _currentMaterial.color);
                newWeaponMaterial[0] = emissiveWithColor;
            }
            catch (NullReferenceException)
            {
                newWeaponMaterial[0] = _currentMaterial;
            }

            weaponMeshRenderer.materials = newWeaponMaterial;
        }
    }
}