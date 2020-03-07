using System;
using Weapon;
using UnityEngine;

namespace ColorPanels
{
    public class ColorPanelObjectFSM : MonoBehaviour
    {
        private WeaponScript.WeaponColor currentMode { get; set; }

        [System.Serializable]
        public struct ColorPanelProperties
        {
            public MaterialList materialList;
            public float playerPropulsionForce;
            public float playerOnAirPropulsionForce;
            public float objectPropulsionForce;
            public bool enableAirControl;

            public ColorPanelProperties(MaterialList materialList, float playerPropulsionForce,
                float playerOnAirPropulsionForce,
                float objectPropulsionForce, bool enableAirControl)
            {
                this.materialList = materialList;
                this.playerPropulsionForce = playerPropulsionForce;
                this.playerOnAirPropulsionForce = playerOnAirPropulsionForce;
                this.objectPropulsionForce = objectPropulsionForce;
                this.enableAirControl = enableAirControl;
            }
        }

        [Header("Basic Settings")] public ColorPanelProperties colorPanelProperties;
        public MeshRenderer meshRenderer;

        [Header("Magnet Mode Settings")] public GameObject dragPosition;
        private Rigidbody _attachedObjectRigidbody;
        public bool m_AttachingObject;
        public float m_AttachingObjectSpeed;

        [Header("LineRenderer Settings")] public LineRenderer m_LineRenderer;
        public LayerMask m_CollisionLayerMask;
        [Range(1f, 400.0f)] public float m_MaxLineDistance;
        bool m_CreateLine;

        void Start()
        {
            //meshRenderer = GetComponent<MeshRenderer>();
            _attachedObjectRigidbody = null;
            ChangeColor(WeaponScript.WeaponColor.None);
            m_CreateLine = false;
            m_AttachingObject = false;
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
                        ColorPanelEffects.UpdateAttachedObject(_attachedObjectRigidbody, dragPosition,
                            m_AttachingObjectSpeed);
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

            if (Physics.Raycast(new Ray(m_LineRenderer.transform.position, m_LineRenderer.transform.forward),
                out l_RaycastHit, m_MaxLineDistance, m_CollisionLayerMask.value))
            {
                l_EndRaycastPosition = Vector3.forward * l_RaycastHit.distance;

                if (l_RaycastHit.transform.tag == "Player")
                {
                    if (!GameController.Instance.m_PlayerDied)
                    {
                        l_RaycastHit.transform.GetComponent<HealthManager>()
                            .DealDamage(l_RaycastHit.transform.GetComponent<HealthManager>().m_MaxHealth);
                    }
                }

                else if (l_RaycastHit.transform.tag == "Cube" || l_RaycastHit.collider.tag == "Attached")
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
                case WeaponScript.WeaponColor.Blue:
                    var pcController = other.gameObject.GetComponent<PlayerControllerFSM>();
                    if (pcController != null)
                    {
                        ColorPanelEffects.PanelSetProperties(colorPanelProperties, transform.up);
                        pcController.stateMachine.SwitchState<Player_State_PlatformJumping>();
                        return;
                    }

                    ColorPanelEffects.ThrowObject(this.gameObject, other, transform.up, colorPanelProperties);
                    break;
            }
        }

        public void OnChildTriggerEnter(Collider objectCollider) => OnTriggerInteract(objectCollider);

        private void OnTriggerInteract(Collider collidedCollider) //OnTrigger
        {
            switch (currentMode)
            {
                case WeaponScript.WeaponColor.Green:
                    RefractionCubeEffect cubeEffect;
                    try
                    {
                        cubeEffect = collidedCollider.GetComponent<RefractionCubeEffect>();
                    }
                    catch (MissingComponentException)
                    {
                        return;
                    }
                    if (collidedCollider.CompareTag("Player") || cubeEffect == null || cubeEffect.currentlyAttached) return;
                    
                    if (_attachedObjectRigidbody == null && !m_AttachingObject)
                    {
                        if (collidedCollider.CompareTag("Attached") && collidedCollider.gameObject ==
                            GameController.Instance.playerComponents.PlayerController.equippedWeapon.m_ObjectAttacher
                                .m_ObjectAttached.gameObject)
                        {
                            GameController.Instance.playerComponents.PlayerController.equippedWeapon.m_ObjectAttacher.DetachObjectVer2(0f);
                        }

                        var newRB = collidedCollider.GetComponent<Rigidbody>();
                        AttachObject(newRB);
                    }

                    break;
            }
        }

        public void OnChildTriggerExit(Collider other)
        { 
            //DetachObject();
        }

        public bool ChangeColor(WeaponScript.WeaponColor color)
        {
            //Here you change the weapon material of the block and stuff.
            if (currentMode == color) return false;

            DetachObject();

            InternalChangeColor(color);

            return true;
        }

        private void InternalChangeColor(WeaponScript.WeaponColor color)
        {
            Material[] objectMaterials = meshRenderer.materials;
            Material colorIndicatorMaterialToChange = objectMaterials[1];
            switch (color)
            {
                case WeaponScript.WeaponColor.None:
                    colorIndicatorMaterialToChange.SetColor("_EmissionColor",
                        colorPanelProperties.materialList.defaultMaterial.GetColor("_EmissionColor"));
                    break;
                case WeaponScript.WeaponColor.Red:
                    colorIndicatorMaterialToChange.SetColor("_EmissionColor",
                        colorPanelProperties.materialList.redMaterial.GetColor("_EmissionColor"));
                    break;
                case WeaponScript.WeaponColor.Green:
                    colorIndicatorMaterialToChange.SetColor("_EmissionColor",
                        colorPanelProperties.materialList.greenMaterial.GetColor("_EmissionColor"));
                    break;
                case WeaponScript.WeaponColor.Blue:
                    colorIndicatorMaterialToChange.SetColor("_EmissionColor",
                        colorPanelProperties.materialList.blueMaterial.GetColor("_EmissionColor"));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(color), color, null);
            }

            objectMaterials[1] = colorIndicatorMaterialToChange;
            meshRenderer.materials = objectMaterials;

            currentMode = color;
        }

        private void AttachObject(Rigidbody l_ObjectToAttach)
        {
            if (_attachedObjectRigidbody || m_AttachingObject) return;
            m_AttachingObject = true;
            _attachedObjectRigidbody = l_ObjectToAttach;
            _attachedObjectRigidbody.useGravity = false;
            _attachedObjectRigidbody.isKinematic = true;
            _attachedObjectRigidbody.gameObject.GetComponent<RefractionCubeEffect>().Attach(this);
        }

        private void DetachObject(float l_DetachForce = 10f)
        {
            if (!m_AttachingObject) return;
            m_AttachingObject = false;
            if (!_attachedObjectRigidbody.CompareTag("Attached"))
            {
                _attachedObjectRigidbody.useGravity = true;
                _attachedObjectRigidbody.isKinematic = false;
            }

            _attachedObjectRigidbody.gameObject.GetComponent<RefractionCubeEffect>().Detach();
            _attachedObjectRigidbody = null;
        }

        public void ForceDetach()
        {
            DetachObject();
        }
    }
}