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

        [Header("LineRenderer Settings")]
        public LineRenderer m_LineRenderer;
        public LayerMask m_CollisionLayerMask;
        [Range(1f, 400.0f)] public float m_MaxLineDistance;
        bool m_CreateLine;

        void Start()
        {
            //meshRenderer = GetComponent<MeshRenderer>();
            _attachedObjectRigidbody = null;
            var temp = meshRenderer.materials;
            temp[1] = colorPanelProperties.materialList.defaultMaterial;
            meshRenderer.sharedMaterials = temp;
            ChangeColor(WeaponScript.WeaponColor.None, colorPanelProperties.materialList.defaultMaterial);
            m_CreateLine = false;
        }

        void Update()
        {
            m_LineRenderer.gameObject.SetActive(m_CreateLine);

            if (m_CreateLine)
                CreateRender();

            switch (currentMode)
            {
                case WeaponScript.WeaponColor.None:
                    break;
                case WeaponScript.WeaponColor.Red:
                    m_CreateLine = true;
                    break;
                case WeaponScript.WeaponColor.Green:
                    m_CreateLine = false;
                    try
                    {
                        ColorPanelEffects.AttractObject(_attachedObjectRigidbody, dragPosition);
                    }
                    catch (NullReferenceException)
                    {
                    }
                    break;
                case WeaponScript.WeaponColor.Blue:
                    m_CreateLine = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        void CreateRender()
        {
            Vector3 l_EndRaycastPosition = Vector3.forward * m_MaxLineDistance;
            RaycastHit l_RaycastHit;

            if (Physics.Raycast(new Ray(m_LineRenderer.transform.position, m_LineRenderer.transform.forward), out l_RaycastHit, m_MaxLineDistance, m_CollisionLayerMask.value))
            {
                l_EndRaycastPosition = Vector3.forward * l_RaycastHit.distance;

                if (l_RaycastHit.transform.tag == "Player")
                {
                    if (!GameController.Instance.m_PlayerDied)
                    {
                        l_RaycastHit.transform.GetComponent<HealthManager>().DealDamage(l_RaycastHit.transform.GetComponent<HealthManager>().m_MaxHealth);
                    }
                }

                else if (l_RaycastHit.transform.tag == "Cube")
                {
                    l_RaycastHit.collider.GetComponent<RefractionCubeEffect>().CreateRefraction();
                }
            }

            m_LineRenderer.SetPosition(1, l_EndRaycastPosition);

            Debug.DrawRay(m_LineRenderer.transform.position, m_LineRenderer.transform.forward * 200.0f, Color.blue);
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