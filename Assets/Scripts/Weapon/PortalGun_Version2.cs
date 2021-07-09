using System;
using System.Collections;
using System.Collections.Generic;
using ColorPanels;
using Interfaces;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Debug = UnityEngine.Debug;

namespace Weapon
{
    // More like a physics gun like the one in GMOD or Half Life 2 BETA. 
    // That thing is rad.
    public class PortalGun_Version2 : MonoBehaviour, IRestartable
    {
        [ReadOnly] [ShowInInspector] private WeaponColor _currentColor = WeaponColor.None;
        public WeaponColor defaultColor = WeaponColor.Blue;

        [FoldoutGroup("Components")] public Light playerLight;
        [FoldoutGroup("Components")] public MeshRenderer weaponMeshRenderer;

        [FoldoutGroup("Materials")] public Material defaultMaterial;
        [FoldoutGroup("Materials")] public Material redMaterial;
        [FoldoutGroup("Materials")] public Material greenMaterial;
        [FoldoutGroup("Materials")] public Material blueMaterial;

        // public Material weaponMaterial;

        [TabGroup("Raycast Settings")] [Tooltip("Max range for the Ray Casting")]
        public float maxRange = 150f;

        [TabGroup("Raycast Settings")] public LayerMask layerMask;

        // [Header("Lights")] public Color[] lightColors = new[] {Color.red, Color.green, Color.blue};

        [TabGroup("Gun Settings")] [Range(60, 1800)]
        public float roundsPerMinute = 120;

        [TabGroup("Gun Settings")] public Rigidbody attractObject;

        [TableList] public List<ParticleSystem> m_WeaponParticleList;
        [TableList] public List<GameObject> m_WeaponMuzzleList;

        private float realROF;
        private float shootTimer;

        private Material _emissiveWithColor;
        private Material _currentMaterial;
        private PlayerMover m_AttachedCharacter;
        private Camera _camera;

        private IAttachable _attachedObject;

        private UnityEvent OnColorChanged = new UnityEvent();

        // private IEnumerator attractObjectCoroutine;
        // private IEnumerator restoreMassCoroutine;


        private void Awake()
        {
            m_AttachedCharacter = GetComponent<PlayerMover>();
            _camera = GetComponentInChildren<Camera>();
        }

        public void Start()
        {
            ChangeColor(defaultColor);
            realROF = 60f / roundsPerMinute;
        }

        // Could use a custom updater yknow.
        private void Update()
        {
            if (shootTimer > 0) shootTimer -= Time.deltaTime;
        }

        private void PlaySoundAndVisuals()
        {
            // m_AttachedCharacter.weaponAnimator.SetTrigger("Shoot");
            AudioManager.instance.Play("Shot");
        }

        private void ActivateParticles()
        {
            switch (_currentColor)
            {
                case WeaponColor.Blue:
                    m_WeaponParticleList[0].Stop();
                    m_WeaponParticleList[0].Play();
                    StartCoroutine(ShotLight(m_WeaponMuzzleList[0], 0.1f));
                    break;
                case WeaponColor.Green:
                    m_WeaponParticleList[1].Stop();
                    m_WeaponParticleList[1].Play();
                    StartCoroutine(ShotLight(m_WeaponMuzzleList[1], 0.1f));
                    break;
                case WeaponColor.Red:
                    m_WeaponParticleList[2].Stop();
                    m_WeaponParticleList[2].Play();
                    StartCoroutine(ShotLight(m_WeaponMuzzleList[2], 0.1f));
                    break;
                case WeaponColor.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


//         private T GetRaycastedElement<T>() where T : IAttachable, IInteractable
//         {
//             var lRay = m_AttachedCharacter.cameraController.attachedCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
//             if (Physics.Raycast(lRay, out var hit, maxRange, layerMask))
//             {
//                 if (!m_ObjectAttacher.m_AttachedObject)
//                 {
//                     var colorPanelHit = hit.collider.gameObject.GetComponent<ColorPanelObjectFSM>();
//                     if (colorPanelHit)
//                     {
//                         // colorPanelHit.ChangeColor(_currentColor); // TODO REENABLE THE PANEL CHANGING ABILITY.
//                         return true;
//                     }
//
//                     if (_currentColor == WeaponColor.Green &&
//                         hit.collider.gameObject.GetComponent<IAttachable>() != null)
//                     {
//                         attractObjectCoroutine = AttractObjectThroughTime(hit);
//                         RestartCoroutine(attractObjectCoroutine);
//                         return false;
//                     }
//                 }
//                 try
//                 {
//                     
//                 }
//                 catch (NullReferenceException)
//                 {
//                               
//                 }
//             }
//
//             return null;
//         }

        #region FireModes

        public void MainFire()
        {
            //Raycast to a target (interface) to interact and change color?
            if (!CanShoot()) return;
            shootTimer = realROF;

            //PlaySoundAndVisuals();
            //ActivateParticles();
            //GameController.Instance.playerComponents.PlayerController.weaponAnimator.SetTrigger("Shoot");

            if (_attachedObject != null) return;
            // This activates whatever it needs for attachment. Physics are handled outside.
            if (GetRaycastInfo(out var hitInfo))
            {
                GameObject go = hitInfo.collider.gameObject;
                if (hitInfo.collider != null)
                {
                    go.GetComponent<IInteractable>()?.Interact(_currentColor);
                    // Debug.Log(go.name);

                    _attachedObject = go.GetComponent<IAttachable>();
                    _attachedObject?.Attach(attractObject);
                    _attachedObject?.OnAttachOverride.AddListener(ForceDetach);
                }
            }
        }

        private void ForceDetach()
        {
            // For now it just sets it to null. Nothing else for now.
            // Forcing it to Detach from this item would bring problems with physics and gravity.
            // 
            _attachedObject?.OnAttachOverride.RemoveListener(ForceDetach);
            _attachedObject = null;
        }

        private void AltFire()
        {
            try
            {
                _attachedObject.Detach();
                _attachedObject = null;
            }
            catch (NullReferenceException)
            {
                ChangeColor((int) _currentColor < 3 ? _currentColor + 1 : (WeaponColor) 1);
                // AudioManager.instance.Play("ChangeColor");
            }
        }

        #endregion

        private bool GetRaycastInfo(out RaycastHit info)
        {
            var lRay = _camera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
            if (Physics.Raycast(lRay, out info, maxRange, layerMask))
            {
                return true;
            }

            return false;
        }

        bool CanShoot()
        {
            return shootTimer <= 0;
        }

        private IEnumerator ShotLight(GameObject MuzzleFlash, float time)
        {
            MuzzleFlash.SetActive(true);
            yield return new WaitForSeconds(time);
            MuzzleFlash.SetActive(false);
        }
        
        #region Callbacks

        public void MainFireCB(InputAction.CallbackContext context)
        {
            if (context.performed)
                MainFire();
        }

        public void AltFireCB(InputAction.CallbackContext context)
        {
            if (context.performed)
                AltFire();
        }

        #endregion

        private void ChangeColor(WeaponColor newColor)
        {
            //Change material and play sounds?
            // if (m_ObjectAttacher.m_ObjectAttached) return;

            // UnityEvent ON WEAPON CHANGE COLOR.
            // THEN CALL IT AND MAKE ALL UI CHANGES OUT OF HERE.
            switch (newColor)
            {
                case WeaponColor.None:
                    //_currentMaterial = defaultMaterial;
                    break;
                case WeaponColor.Red:
                    //ChangeReticleColor(WeaponColor.Red);
                    //_currentMaterial = redMaterial;
                    //ChangeLightColor(playerLight, lightColors[0]);
                    break;
                case WeaponColor.Green:
                    //ChangeReticleColor(WeaponColor.Green);
                    //_currentMaterial = greenMaterial;
                    //ChangeLightColor(playerLight, lightColors[1]);
                    break;
                case WeaponColor.Blue:
                    //ChangeReticleColor(WeaponColor.Blue);
                    //_currentMaterial = blueMaterial;
                    //ChangeLightColor(playerLight, lightColors[2]);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newColor), newColor, null);
            }

            _currentColor = newColor;
            OnColorChanged.Invoke();
        }

        // TODO move to external script 
        // THIS DOES NOT BELONG HERE.
        // private void ChangeReticleColor(WeaponColor color)
        // {
        //     try
        //     {
        //         GameController.Instance.m_CanvasController.ChangeReticleColor((int) color);
        //     }
        //     catch (NullReferenceException)
        //     {
        //     }
        //
        // private void ChangeLightColor(Light light, Color color)
        // {
        //     if (light == null) return;
        //
        //     light.color = color;
        // }

        public void Restart()
        {
            // _attachedObject?.Detach();
        }
    }
}