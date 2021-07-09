using System;
using Aura2API;
using ColorPanels;
using Interfaces;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using Utils;
using Debug = UnityEngine.Debug;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class Cube_Version2 : MonoBehaviour, IRestartable, IParentable, IAttachable, IBoostable, ILaserCast
{
    private bool m_CreateRefraction;
    private bool m_CubeRefracted;

    //material
    public Material m_StatusMaterial;
    private const string c_EmissionColor = "_EmissionColor";
    private Material[] m_StatusMaterials;
    private bool m_ChangeColorMaterial;
    public Rigidbody ownRigidbody { get; set; }

    public bool currentlyAttached { get; set; }
    public ColorPanelObjectFSM AttachedOnThisPanel { get; set; }

    //dynamic collider
    public MeshRenderer meshRenderer;

    /// <summary>
    /// Restarting positon variables
    /// </summary>
    private Vector3 startingPosition;

    private Quaternion initialRotation;

    private SwitchController m_raySwitch;

    private FixedJoint _joint;

    private LaserCollider _laser;

    private ConfigurableJoint _attachJoint;

    [DisableInPlayMode] public Cube_Version2 prefab;

    public UnityEvent OnAttachOverride { get; set; } // TODO REVISE THIS WORKS CORRECTLY.

    private void Awake()
    {
        _laser = GetComponentInChildren<LaserCollider>();
        ownRigidbody = GetComponent<Rigidbody>();
        ownRigidbody.Sleep();

        OnAttachOverride = new UnityEvent();
    }

    private void Start()
    {
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        startingPosition = transform.position;
        initialRotation = transform.rotation;
        _attachJoint = GetComponent<ConfigurableJoint>();
        Destroy(_attachJoint);

        StopCasting();

        // var renderers = GetComponentsInChildren<Renderer>();
        // for (int i = 0; i < renderers.Length; ++i)
        // {
        //     var materials = renderers[i].sharedMaterials;
        //     for (int j = 0; j < materials.Length; ++j)
        //     {
        //         if (materials[j] != m_StatusMaterial)
        //             continue;
        //
        //         if (m_StatusMaterials == null)
        //             m_StatusMaterials = new Material[1];
        //         else
        //             System.Array.Resize(ref m_StatusMaterials, m_StatusMaterials.Length + 1);
        //
        //         m_StatusMaterials[m_StatusMaterials.Length - 1] = renderers[i].materials[j];
        //     }
        // }
        //
        // currentlyAttached = false;
    }

    // void Update()
    // {
    //     if (m_ChangeColorMaterial)
    //     {
    //         m_StatusMaterials[0].SetColor(c_EmissionColor, Color.red);
    //         m_ChangeColorMaterial = false;
    //     }
    // }

    public void Attach(ColorPanelObjectFSM attachedTo)
    {
        AttachedOnThisPanel = attachedTo;
        currentlyAttached = true;
        if (_joint != null) Destroy(_joint);
    }

    public void Detach(bool force = false)
    {
        if (force) AttachedOnThisPanel.ForceDetach();
        AttachedOnThisPanel = null;
        currentlyAttached = false;
    }

    [Button("Restart", ButtonSizes.Medium)]
    public void Restart()
    {
        if (gameObject.activeInHierarchy)
        {
            transform.position = startingPosition;
            transform.rotation = initialRotation;
            ownRigidbody.velocity = Vector3.zero;
        }

        //ChangeMaterials(cubeMaterialList[0]);
    }

    public Transform ReturnSelf()
    {
        return this.gameObject.transform;
    }

    public bool Emparent(GameObject newParent)
    {
        RaycastHit raycastHit;
        if (Physics.Raycast(this.gameObject.transform.position, this.transform.up * -1, out raycastHit, 5f))
        {
            if (raycastHit.collider.gameObject.GetHashCode() == newParent.GetHashCode())
            {
                _joint = gameObject.AddComponent<FixedJoint>();
                _joint.enableCollision = true;
                _joint.connectedBody = newParent.GetComponent<Rigidbody>();
                return true;
            }
            else
            {
                return false;
            }
        }

        return false;
    }

    public bool Deparent(Transform oldParent)
    {
        try
        {
            this.gameObject.transform.parent = oldParent;
            Destroy(_joint);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            return false;
        }
    }

    private void CreateAttachJoint(Transform parent)
    {
        _attachJoint = gameObject.AddComponent<ConfigurableJoint>();
        _attachJoint.autoConfigureConnectedAnchor = false;
        _attachJoint.axis = Vector3.forward;
        _attachJoint.secondaryAxis = Vector3.forward;
        _attachJoint.connectedAnchor = Vector3.zero;
        _attachJoint.swapBodies = true;

        // _attachJoint.linearLimit = new SoftJointLimit()
        // {
        //     limit = 3f
        // };
        _attachJoint.angularXMotion = ConfigurableJointMotion.Free;
        _attachJoint.angularYMotion = ConfigurableJointMotion.Free;
        _attachJoint.angularZMotion = ConfigurableJointMotion.Free;

        _attachJoint.xDrive = new JointDrive()
        {
            positionSpring = 3500,
            positionDamper = 750,
            maximumForce = Mathf.Infinity
        };
        _attachJoint.yDrive = _attachJoint.xDrive;
        _attachJoint.zDrive = _attachJoint.xDrive;

        // TODO https://answers.unity.com/questions/278147/how-to-use-target-rotation-on-a-configurable-joint.html
        UpdateAttachJoint(parent);
        //_attachJoint.targetRotation = Quaternion.FromToRotation(transform.forward, parent.forward);
        _attachJoint.angularYZDrive = new JointDrive()
        {
            positionSpring = 5000f,
            positionDamper = 500f,
            maximumForce = Mathf.Infinity
        };
        _attachJoint.angularXDrive = _attachJoint.angularYZDrive;
    }

    private void UpdateAttachJoint(Transform parent)
    {
        // TODO Solve rotation.
        //Quaternion finalRotation;
        //Vector3 currentRotation = transform.rotation.eulerAngles;
        //Vector3 destineRotation = parent.rotation.eulerAngles;

        //finalRotation = Quaternion.RotateTowards(transform.rotation, parent.localRotation, 600f);

        // finalRotation = Quaternion.FromToRotation(transform.up, parent.up);
        // finalRotation *= Quaternion.FromToRotation(transform.forward, parent.forward);
        // finalRotation *= Quaternion.FromToRotation(transform.right, parent.right);
        //_attachJoint.targetRotation = finalRotation;
    }

    #region Attach Interface

    // https://answers.unity.com/questions/530178/how-to-get-a-component-from-an-object-and-add-it-t.html
    public void Attach(Rigidbody rb)
    {
        // ownRigidbody.velocity = Vector3.zero;
        if (_attachJoint == null)
            CreateAttachJoint(rb.gameObject.transform);
        else
        {
            UpdateAttachJoint(rb.gameObject.transform);
            OnAttachOverride.Invoke();
        }

        ownRigidbody.useGravity = false;
        _attachJoint.connectedBody = rb;
    }

    public void Detach()
    {
        ownRigidbody.useGravity = true;
        // _attachJoint.connectedBody = null;
        Destroy(_attachJoint);
    }

    #endregion

    public void Boost(Vector3 Force)
    {
        // Maybe reset the angular velocity?
        // Maybe stop it before boosting it tbh.
        ownRigidbody.velocity = Force;
        // ownRigidbody.AddForce(Force, ForceMode.Impulse);
        //ownRigidbody.velocity = Force;
    }

    public bool IsSleeping => ownRigidbody.IsSleeping();

    public bool CastLaser()
    {
        try
        {
            if (!_laser.gameObject.activeSelf)
            {
                // If deactivated, activate the laser.
                _laser.gameObject.SetActive(true);
            }
            else if (IsSleeping)
            {
                // If activated and sleeping then skip the update.
                return false;
            }
            _laser.UpdateLaserAutonomously();
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public void StopCasting()
    {
        try
        {
            if (_laser.gameObject.activeSelf)
                _laser.gameObject.SetActive(false);
        }
        catch (Exception e)
        {
            if (e is NullReferenceException || e is MissingComponentException)
            {
                // Do nothing.
            }
        }
    }

    private void OnBecameInvisible()
    {
        ownRigidbody.Sleep();
    }

    private void OnBecameVisible()
    {
        ownRigidbody.WakeUp();
    }
}