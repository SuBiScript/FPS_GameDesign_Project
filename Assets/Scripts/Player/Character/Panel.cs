using System;
using Interfaces;
using Sirenix.OdinInspector;
using Weapon;
using UnityEngine;
using Component = UnityEngine.Component;

namespace Panels
{
    [RequireComponent(typeof(CalculateTrajectory))]
    [RequireComponent(typeof(AudioSource))]
    public class Panel : MonoBehaviour, IRestartable, IAutoDisable, IInteractable, IPlayerCollide, ILaserCast
    {
        #region Modes

        [DisableInPlayMode] public WeaponColor defaultMode = WeaponColor.None;

        [ShowInInspector]
        [Sirenix.OdinInspector.ReadOnly]
        [PropertyOrder(-1)]
        private WeaponColor currentMode { get; set; }

        #endregion

        #region Components

        [FoldoutGroup("Components")] [ShowInInspector]
        private AudioSource m_AudioSource;

        [FoldoutGroup("Components")] private LaserCollider _laser;

        [FoldoutGroup("Components")] [HideInInspector]
        public PanelVisuals _panelVisuals;

        [FoldoutGroup("Components")] private Rigidbody _attachedObjectRigidbody;
        [FoldoutGroup("Components")] private MeshRenderer meshRenderer;
        [FoldoutGroup("Components")] public BoxCollider _collider;

        #endregion

        #region Audios

        [FoldoutGroup("Audios")] public AudioClip m_JumpPlatform;
        [FoldoutGroup("Audios")] public AudioClip magnetSound;

        #endregion

        #region Red

        #endregion

        #region Green

        [TabGroup("Settings", "Green")] [Range(0f, 5f)] [OnValueChanged("UpdateObjectAttachPoint")]
        public float AttachDistance = 2f;

        [TabGroup("Settings", "Green")] public Rigidbody ObjectAttachPoint;
        [TabGroup("Settings", "Green")] private IAttachable _attachedObject;


        [TabGroup("Settings", "Green")] [Title("Debug")] [Space(5)]
        public bool ShowDebugMesh = false;

        #endregion

        #region Blue

        [TabGroup("Settings", "Blue")] [DisableIf("PlayerFinalPosition")]
        public Vector3 PlayerForce;

        [TabGroup("Settings", "Blue")] [Tooltip("Where will the player land. Used for visualization too.")]
        public Transform PlayerFinalPosition;

        [TabGroup("Settings", "Blue")] [DisableIf("ObjectFinalPosition")] [Space(10)]
        public Vector3 ObjectForce;

        [TabGroup("Settings", "Blue")] public Transform ObjectFinalPosition;

        [Space(10)] [TabGroup("Settings", "Blue")] [ShowIf("PlayerFinalPosition")] [Range(1f, 5f)]
        public float TimeOfFlight = 2f;

        [TabGroup("Settings", "Blue")] [ShowIf("PlayerFinalPosition")] [Range(0f, 25f)]
        public float Gravity = 20f;

        [TabGroup("Settings", "Blue")]
        [HideInInspector]
        public Vector3 realBoostPoint => transform.position + transform.up * 0.2f;

        #endregion

        #region Debug

        [Tooltip("Mesh to show where the cube will be attached on Green mode.")]
        [ShowIf("ShowDebugMesh")]
        [TabGroup("Settings", "Green")]
        [AssetsOnly]
        public Mesh DebugCube;

        #endregion

        #region Plane

        private float PlaneDistance = 0.25f;
        private Plane PanelPlane;

        #endregion

        private void Awake()
        {
            GetAllComponents();
        }

        private void GetAllComponents()
        {
            meshRenderer = GetComponentInChildren<MeshRenderer>();
            m_AudioSource = GetComponent<AudioSource>();
            _panelVisuals = GetComponentInChildren<PanelVisuals>();
            _laser = GetComponentInChildren<LaserCollider>();
            _collider = GetComponent<BoxCollider>();
        }

        private void Start()
        {
            ChangeColor(defaultMode, true);
            SetupPlane();
            // UpdateObjectAttachPoint();
        }

        private void SetupPlane()
        {
            var o = gameObject;
            var up = o.transform.up;
            PanelPlane = new Plane(up, o.transform.position + up * PlaneDistance);
        }


        private void OnCollisionEnter(Collision other) => ActivateCollision(other);

        private void OnCollisionStay(Collision other) => ActivateCollision(other);

        private void ActivateCollision(Collision other) //OnCollision
        {
            switch (currentMode)
            {
                case WeaponColor.Blue:
                    IBoostable Object = other.gameObject.GetComponent<IBoostable>();
                    if (Object == null)
                    {
                        Debug.Log("Collision " + other.gameObject.name + " is NOT boostable.");
                        return;
                    }

                    // Check if its on the correct side of the object.
                    if (!PanelPlane.GetSide(other.gameObject.transform.position))
                        return;

                    // Boost the interface, not caring if its an object or a player.
                    Object.Boost(PlayerForce); // TODO Check if everybody is ok with the rework.

                    ClipAndPlay(m_JumpPlatform);
                    break;
            }
        }

        public void OnChildTriggerEnter(Collider objectCollider) => OnTriggerInteraction(objectCollider);

        private void OnTriggerInteraction(Collider collidedCollider) //OnTrigger
        {
            switch (currentMode)
            {
                case WeaponColor.Green:
                    if (GreenAttach(collidedCollider))
                        ClipAndPlay(magnetSound);
                    break;
            }
        }

        private bool GreenAttach(Component collision)
        {
            if (_attachedObject != null) return false;
            try
            {
                _attachedObject = collision.GetComponent<IAttachable>();
                _attachedObject.Attach(ObjectAttachPoint);
                _attachedObject.OnAttachOverride.AddListener(ForcedDetach);
                return true;
            }
            catch (NullReferenceException)
            {
                return false;
            }
        }

        private void ForcedDetach()
        {
            _attachedObject?.OnAttachOverride.RemoveListener(ForcedDetach);
            _attachedObject = null;
        }

        public void OnChildTriggerExit(Collider other)
        {
            if (other.gameObject.GetComponent<IAttachable>() == _attachedObject)
                Detach();
        }

        private void Detach()
        {
            _attachedObject?.Detach();
            ForcedDetach();
        }

        private bool ChangeColor(WeaponColor color, bool forceChange = false)
        {
            if (currentMode == color && !forceChange) return false;

            ChangePanelFunction(color);
            _panelVisuals.ChangeVisualColor(color);
            return true;
        }

        /// <summary>
        /// This changes the panel function even if the color does not work / match.
        /// </summary>
        /// <param name="newColor"></param>
        private void ChangePanelFunction(WeaponColor newColor)
        {
            // This code executes code for exiting the current color mode.
            // And after that, executing code for entering the current mode.
            // Hard to break honestly. Hope it does not.
            switch (currentMode)
            {
                case WeaponColor.None:
                    _laser.gameObject.SetActive(false);
                    break;
                case WeaponColor.Red:
                    _laser.gameObject.SetActive(false);
                    break;
                case WeaponColor.Green:
                    Detach();
                    break;
                case WeaponColor.Blue:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            switch (newColor)
            {
                case WeaponColor.None:
                    _laser.gameObject.SetActive(false);
                    break;
                case WeaponColor.Red:
                    _laser.gameObject.SetActive(true);
                    _laser.UpdateLaserAutonomously();
                    break;
                case WeaponColor.Green:
                    break;
                case WeaponColor.Blue:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newColor), newColor, null);
            }

            currentMode = newColor;
        }

        private void ClipAndPlay(AudioClip clip)
        {
            if (m_AudioSource == null) return;

            m_AudioSource.clip = clip;
            m_AudioSource.Play();
        }

        #region Variable Functions

        // Region for functions that apply when variables change.

        private void UpdateObjectAttachPoint()
        {
            ObjectAttachPoint.transform.localPosition = new Vector3(0f, AttachDistance, 0f);
            // Maybe also update the whole collider so it centers on that position and extends to touch the mesh.
            // collider.center = objectattachpoint.transform.position
            // collider.bounds = bounds { z = attachdistance * 2f }
        }

        #endregion

        #region Interfaces

        public void Interact(WeaponColor color)
        {
            ChangeColor(color);
        }

        public void Restart()
        {
            ChangeColor(defaultMode);
        }

        public bool IsSleeping { get; set; }

        public bool CastLaser()
        {
            // Nothing.
            return false;
        }

        public void StopCasting()
        {
            // Nothing?
        }

        #endregion

        #region Inspector Buttons

#if UNITY_EDITOR
        [DisableInEditorMode]
        [ButtonGroup("ColorChangers")]
        [Button("Set Red")]
        public void ChangeToRed()
        {
            ChangeColor(WeaponColor.Red);
        }

        [DisableInEditorMode]
        [ButtonGroup("ColorChangers")]
        [Button("Set Green")]
        public void ChangeToGreen()
        {
            ChangeColor(WeaponColor.Green);
        }

        [DisableInEditorMode]
        [ButtonGroup("ColorChangers")]
        [Button("Set Blue")]
        public void ChangeToBlue()
        {
            ChangeColor(WeaponColor.Blue);
        }
#endif

        #endregion

        #region Gizmos

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (ShowDebugMesh)
                Gizmos.DrawMesh(DebugCube, ObjectAttachPoint.transform.position);

            // Gizmos.DrawCube(PanelPlane.ClosestPointOnPlane(this.gameObject.transform.position), new Vector3(2f, 0.05f, 2f));
        }
#endif

        #endregion

        #region Player Only Methods

        public bool Collide(GameObject self, Vector3 collisionPoint)
        {
            try
            {
                ActivatePlayerCollision(self.GetComponent<IBoostable>(), collisionPoint);
                return true;
            }
            catch (NullReferenceException)
            {
                return false;
            }
        }

        void ActivatePlayerCollision(IBoostable hit, Vector3 worldPosition)
        {
            switch (currentMode)
            {
                case WeaponColor.Blue:
                    if (!PanelPlane.GetSide(worldPosition))
                        return;

                    hit.Boost(PlayerForce); // Boosts the player or the object with appropriate forces.

                    ClipAndPlay(m_JumpPlatform);
                    break;
            }
        }

        #endregion
    }
}