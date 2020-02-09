using System;
using System.Runtime.CompilerServices;
using ColorPanels;
using UnityEngine;

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
            }

            public void DetachObject(float l_DetachForce = 20f)
            {
                if (!m_AttachingObject) return;
                m_AttachingObject = false;
                m_ObjectAttached.useGravity = true;
                m_ObjectAttached.isKinematic = false;
                m_ObjectAttached.AddForce(m_AttachingPosition.transform.forward * l_DetachForce, ForceMode.Impulse);
                m_ObjectAttached.tag = "Untagged";
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
        private Material _currentMaterial;
        public Material _weaponMaterial;


        [Header("Raycast Settings")] [Tooltip("Max range for the Ray Casting")]
        public float maxRange = 150f;

        public LayerMask layerMask;

        private PlayerController m_AttachedCharacter;
        private WeaponColor _currentColor = WeaponColor.None;


        //Material mymat = GetComponent<Renderer>().material;
        //mymat.SetColor("_EmissionColor", Color.red);

        public void Init(PlayerController attachedCharacter)
        {
            m_AttachedCharacter = attachedCharacter;
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

        public void AltFire() => ChangeColor((int) _currentColor < 3 ? _currentColor + 1 : (WeaponColor) 1);

        public void AttractObject()
        {
            if (m_ObjectAttacher.m_ObjectAttached != null)
            {
                Debug.Log("ENTERING THE ATTRACTOBJCET IN PLAYER");
                m_ObjectAttacher.UpdateAttachedObject();
            }
            else
            {
                var lRay = m_AttachedCharacter.m_AttachedCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
                if (Physics.Raycast(lRay, out var hit, maxRange, layerMask))
                {
                    var rb = hit.collider.gameObject.GetComponent<Rigidbody>();
                    rb.velocity = Vector3.zero;
                    m_ObjectAttacher.AttachObject(rb);
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
                    //_weaponMaterial.SetColor("_EmissionColor", Color.yellow);
                    break;
                case WeaponColor.Red:
                    GameController.Instance.m_CanvasController.ChangeReticleColor((int) WeaponColor.Red);
                    _currentMaterial = materialList.redMaterial;
                    //_weaponMaterial.SetColor("_EmissionColor", Color.red);
                    break;
                case WeaponColor.Green:
                    GameController.Instance.m_CanvasController.ChangeReticleColor((int) WeaponColor.Green);
                    _currentMaterial = materialList.greenMaterial;
                    //_weaponMaterial.SetColor("_EmissionColor", Color.green);
                    break;
                case WeaponColor.Blue:
                    GameController.Instance.m_CanvasController.ChangeReticleColor((int) WeaponColor.Blue);
                    _currentMaterial = materialList.blueMaterial;
                    //_weaponMaterial.SetColor("_EmissionColor", Color.blue);
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