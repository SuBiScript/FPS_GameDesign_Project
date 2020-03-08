using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using ColorPanels;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Weapon
{
    public class WeaponScript : MonoBehaviour , IRestartable
    {
        [System.Serializable]
        public class ObjectAttacher
        {
            [Header("Attaching Object")] public bool m_AttachingObject;

            [Header("Attaching Object")]
            [HideInInspector]
            public bool m_AttachedObject;

            public Rigidbody m_ObjectAttached;
            public float m_AttachingObjectSpeed;
            public Transform m_AttachingPosition;

            [HideInInspector] public Quaternion m_AttachingObjectStartRotation;
            [HideInInspector] public bool m_GravityShot;
            [HideInInspector] public bool m_CubeButton; // if cube has been attached
            [HideInInspector] public bool m_Rendered;

            [HideInInspector]
            public Transform[] m_ChildsAttachedObject; //change Layer attribute for rendering object attached

            public WeaponScript weapon;

            public ObjectAttacher()
            {
                m_ObjectAttached = null;
                m_AttachingObject = m_ObjectAttached != null;
            }

            //new attach method
            public void UpdateAttachedObject_()
            {
                Vector3 l_EulerAngles = m_AttachingPosition.rotation.eulerAngles;
                m_CubeButton = true;

                if (!m_AttachedObject)
                {
                    Vector3 l_Direction = m_AttachingPosition.transform.position - m_ObjectAttached.transform.position;
                    float l_Distance = l_Direction.magnitude;
                    float l_Movement = m_AttachingObjectSpeed * Time.deltaTime;

                    if (l_Movement >= l_Distance)
                    {
                        m_AttachedObject = true;
                        m_ObjectAttached.MovePosition(m_AttachingPosition.position);
                        m_ObjectAttached.MoveRotation(Quaternion.Euler(0.0f, l_EulerAngles.y, l_EulerAngles.z));
                    }
                    else
                    {
                        l_Direction /= l_Distance;
                        m_ObjectAttached.MovePosition(m_ObjectAttached.transform.position + l_Direction * l_Movement);
                        m_ObjectAttached.MoveRotation(Quaternion.Lerp(m_AttachingObjectStartRotation,
                            Quaternion.Euler(0.0f, l_EulerAngles.y, l_EulerAngles.z),
                            1.0f - Mathf.Min(l_Distance / 1.5f, 1.0f)));
                    }
                }
                else
                {
                    //l_EulerAngles.Set(0.0f, l_EulerAngles.y, l_EulerAngles.x);
                    //l_EulerAngles = l_EulerAngles.normalized * 0.001f;
                    //Quaternion deltaRotation = Quaternion.Euler(l_EulerAngles);
                    //m_ObjectAttached.MoveRotation(m_ObjectAttached.rotation * deltaRotation);
                    m_ObjectAttached.WakeUp();
                    m_ObjectAttached.transform.rotation = Quaternion.Euler(0.0f, l_EulerAngles.y, l_EulerAngles.z);
                    //m_ObjectAttached.MoveRotation(Quaternion.Euler(0.0f, l_EulerAngles.y, l_EulerAngles.z));
                    //m_ObjectAttached.MoveRotation(Quaternion.Slerp(m_AttachingObjectStartRotation, Quaternion.Euler(0.0f, l_EulerAngles.y, l_EulerAngles.z), 0.01f));
                    //Quaternion.Lerp(Quaternion.Euler(0, 0, 0), Quaternion.Euler(0.0f, l_EulerAngles.y, l_EulerAngles.z), 0.05f);
                    //m_ObjectAttached.MovePosition(m_AttachingPosition.position);
                    m_ObjectAttached.transform.position = m_AttachingPosition.position;

                    if (!m_Rendered)
                    {
                        m_ChildsAttachedObject = m_ObjectAttached.GetComponentsInChildren<Transform>();
                        foreach (Transform child in m_ChildsAttachedObject)
                        {
                            //if (child.tag == "MeshAttached")
                            child.gameObject.layer = LayerMask.NameToLayer("AttachedObject");
                            m_ObjectAttached.isKinematic = true;
                            m_ObjectAttached.WakeUp();
                            m_ObjectAttached.GetComponent<Collider>().isTrigger = true;
                            m_ObjectAttached.transform.parent = GameController.Instance.playerComponents
                                .PlayerController.m_PitchControllerTransform;
                            m_Rendered = true;
                        }
                    }
                }
            }

            public void AttachObjectVer2(Rigidbody rb)
            {
                var tryingToAttachThisObject = rb.gameObject.GetComponent<RefractionCubeEffect>();
                if (tryingToAttachThisObject.currentlyAttached) tryingToAttachThisObject.Detach(true);
                rb.gameObject.tag = "Attached";
                m_ObjectAttached = rb;
                m_AttachingObjectStartRotation = m_ObjectAttached.transform.rotation;
                m_GravityShot = true;
            }

            public void DetachObjectVer2(float force)
            {
                if (m_ObjectAttached == null) return;

                foreach (Transform child in m_ChildsAttachedObject)
                {
                    //if (child.tag == "MeshAttached")
                    child.gameObject.layer = LayerMask.NameToLayer("Cube");
                }

                try
                {
                    m_Rendered = false;
                    m_GravityShot = false;
                    m_AttachedObject = false;
                    m_CubeButton = false;
                    m_ObjectAttached.transform.parent = null;
                    m_ObjectAttached.useGravity = true;
                    m_ObjectAttached.GetComponent<Collider>().isTrigger = false;
                    m_ObjectAttached.isKinematic = false;
                    m_ObjectAttached.gameObject.tag = "Cube";
                    weapon.RestoreMass(m_ObjectAttached);
                    weapon.RestoreLayers();
                    m_ObjectAttached.AddForce(m_AttachingPosition.forward * force);
                    m_ObjectAttached = null;
                }
                catch (Exception e)
                {
                    Debug.Log(e.Message);
                }
            }
        }

        public ObjectAttacher m_ObjectAttacher = new ObjectAttacher();

        [Header("Weapon Effects")] public List<ParticleSystem> m_WeaponParticleList;
        public List<GameObject> m_WeaponMuzzleList;


        public enum WeaponColor
        {
            None,
            Red,
            Green,
            Blue
        }

        public MaterialList materialList;
        public MeshRenderer weaponMeshRenderer;
        public Material weaponMaterial;

        [Header("Attract Settings")] public GameObject attractPoint;

        private Material emissiveWithColor;
        private Material _currentMaterial;



        [Header("Raycast Settings")]
        [Tooltip("Max range for the Ray Casting")]
        public float maxRange = 150f;

        public LayerMask layerMask;

        [Header("Lights")] public Color[] lightColors = new[] { Color.red, Color.green, Color.blue };

        public Light playerLight;

        private PlayerControllerFSM m_AttachedCharacter;
        private WeaponColor _currentColor = WeaponColor.None;
        private IEnumerator restoreMassCoroutine;

        [Header("Weapon Shooting Properties")]
        [Range(60, 1800)]
        public float roundsPerMinute = 120;

        [Range(1, 300)] public int forceAttractRepetitions = 10;
        private float realROF;
        private float shootTimer;
        private IEnumerator attractObjectCoroutine;


        public void Init(PlayerControllerFSM attachedCharacter)
        {
            m_AttachedCharacter = attachedCharacter;
            materialList = Instantiate(materialList);
            ChangeColor(WeaponColor.Blue);
            realROF = 60f / roundsPerMinute;
        }

        private void Update()
        {
            if (shootTimer > 0) shootTimer -= Time.deltaTime;
        }

        private void FixedUpdate()
        {
            if (m_ObjectAttacher.m_GravityShot)
                m_ObjectAttacher.UpdateAttachedObject_();
        }

        public bool MainFire()
        {
            //Raycast to a target (interface) to interact and change color?
            if (!CanShoot()) return false;
            shootTimer = realROF;

            if (!m_ObjectAttacher.m_AttachedObject && GameController.Instance.m_IntroFinished)
            {
                GameController.Instance.playerComponents.PlayerController.weaponAnimator.SetTrigger("Shoot");
                AudioManager.instance.Play("Shot");
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
                }
            }

            var lRay = m_AttachedCharacter.cameraController.attachedCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
            if (Physics.Raycast(lRay, out var hit, maxRange, layerMask))
            {
                if (!m_ObjectAttacher.m_AttachedObject)
                {
                    var colorPanelHit = hit.collider.gameObject.GetComponent<ColorPanelObjectFSM>();
                    if (colorPanelHit)
                    {
                        colorPanelHit.ChangeColor(_currentColor);
                        return true;
                    }
                    if (_currentColor == WeaponColor.Green &&
                        hit.collider.gameObject.GetComponent<RefractionCubeEffect>())
                    {
                        attractObjectCoroutine = AttractObjectThroughTime(hit);
                        RestartCoroutine(attractObjectCoroutine);
                        return false;
                    }
                }
            }
            return true;
        }

        bool CanShoot()
        {
            return shootTimer <= 0;
        }

        public IEnumerator AttractObjectThroughTime(RaycastHit info)
        {
            for (int i = 0; i < forceAttractRepetitions; i++)
            {
                AttractObject(info);
                yield return null;
            }
        }


        public IEnumerator ShotLight(GameObject MuzzleFlash, float time)
        {
            MuzzleFlash.SetActive(true);
            yield return new WaitForSeconds(time);
            MuzzleFlash.SetActive(false);
        }

        public bool AltFire()
        {
            if (!m_ObjectAttacher.m_AttachedObject && GameController.Instance.m_IntroFinished)
            {
                ChangeColor((int)_currentColor < 3 ? _currentColor + 1 : (WeaponColor)1);
                AudioManager.instance.Play("ChangeColor");
                return true;
            }
            else
            {
                m_ObjectAttacher.DetachObjectVer2(0);
                return false;
            }

            return false;
        }

        public void AttractObject(RaycastHit hitInfo)
        {
            try
            {
                var rb = hitInfo.collider.gameObject.GetComponent<Rigidbody>();
                //Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Cube"), true);
                rb.velocity = Vector3.zero;
                m_ObjectAttacher.AttachObjectVer2(rb);
            }
            catch (MissingComponentException)
            {
            }
        }

        /*public void DetachObject()
        {
            m_ObjectAttacher.DetachObject();
        }*/

        public void RestoreLayers()
        {
            Invoke("RestoringLayers", .1f);
        }

        void RestoringLayers()
        {
            //Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Cube"), false);
        }

        public void RestoreMass(Rigidbody rb)
        {
            rb.mass = 0.001f;
            restoreMassCoroutine = RestoreMassAfterTime(rb, 0.2f);
            RestartCoroutine(restoreMassCoroutine);
            //Invoke("RestoringMass", .2f);
        }

        /*void RestoringMass(Rigidbody rigidbody)
        {
            rigidbody.mass = 1;
        }*/

        IEnumerator RestoreMassAfterTime(Rigidbody rigidbody, float time)
        {
            yield return new WaitForSeconds(time);
            rigidbody.mass = 1;
        }

        void RestartCoroutine(IEnumerator enumerator)
        {
            if (enumerator != null) StopCoroutine(enumerator);
            StartCoroutine(enumerator);
        }

        private void ChangeColor(WeaponColor newColor)
        {
            //Change material and play sounds?
            if (m_ObjectAttacher.m_ObjectAttached) return;

            switch (newColor)
            {
                case WeaponColor.None:
                    _currentMaterial = materialList.defaultMaterial;
                    break;
                case WeaponColor.Red:
                    ChangeReticleColor(WeaponColor.Red);
                    _currentMaterial = materialList.redMaterial;
                    ChangeLightColor(playerLight, lightColors[0]);
                    break;
                case WeaponColor.Green:
                    ChangeReticleColor(WeaponColor.Green);
                    _currentMaterial = materialList.greenMaterial;
                    ChangeLightColor(playerLight, lightColors[1]);
                    break;
                case WeaponColor.Blue:
                    ChangeReticleColor(WeaponColor.Blue);
                    _currentMaterial = materialList.blueMaterial;
                    ChangeLightColor(playerLight, lightColors[2]);
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
            catch (NullReferenceException)
            {
            }
        }

        private void ChangeLightColor(Light light, Color color)
        {
            if (light == null) return;

            light.color = color;
        }

        private void ChangeMeshRendererMaterial()
        {
            weaponMaterial.SetColor("_EmissionColor", _currentMaterial.GetColor("_EmissionColor"));
            /*Material[] newWeaponMaterial = weaponMeshRenderer.materials;
            try
            {
                emissiveWithColor = new Material(materialList.emissiveMaterial);
                emissiveWithColor.SetColor("_EmissionColor", _currentMaterial.GetColor("_EmissionColor"));
                newWeaponMaterial[1] = emissiveWithColor;
            }
            catch (NullReferenceException)
            {
                newWeaponMaterial[1] = _currentMaterial;
            }

            weaponMeshRenderer.materials = newWeaponMaterial;*/
        }

        private void OnDisable()
        {
            weaponMaterial.SetColor("_EmissionColor", Color.white);
        }

        public void Restart()
        {
            ChangeColor(WeaponColor.Blue);
        }
    }
}