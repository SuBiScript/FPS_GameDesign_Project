using System;
using ColorPanels;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class RefractionCubeEffect : MonoBehaviour, IRestartable, IParentable, IAttachable
{
    [System.Serializable]
    public struct CubeMaterials
    {
        public Material cubeMaterial;
        public Material colorIndicatorMaterial;
    }

    public CubeMaterials[] cubeMaterialList = new CubeMaterials[2];
    [Space(10)] public LineRenderer m_LineRenderer;
    public LayerMask m_CollisionLayerMask;
    [Range(1f, 400.0f)] public float m_MaxLineDistance;
    bool m_CreateRefraction;
    bool m_CubeRefracted;

    //material
    public Material m_StatusMaterial;
    private const string c_EmissionColor = "_EmissionColor";
    private Material[] m_StatusMaterials;
    private bool m_ChangeColorMaterial;
    public Rigidbody ownRigidbody { get; set; }

    public bool currentlyAttached { get; set; }
    public ColorPanelObjectFSM AttachedOnThisPanel { get; set; }

    //dynamic collider
    public CapsuleCollider m_Collider;
    public MeshRenderer meshRenderer;

    /// <summary>
    /// Restarting positon variables
    /// </summary>
    private Vector3 startingPosition;

    private Quaternion initialRotation;

    private SwitchController m_raySwitch;

    private FixedJoint _joint;

    [Header("Particles")]
    public ParticleSystem laserParticles;

    private void Start()
    {
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        startingPosition = transform.position;
        initialRotation = transform.rotation;
        ownRigidbody = GetComponent<Rigidbody>();

        m_Collider = GetComponentInChildren<CapsuleCollider>();

        var renderers = GetComponentsInChildren<Renderer>();
        for (int i = 0; i < renderers.Length; ++i)
        {
            var materials = renderers[i].sharedMaterials;
            for (int j = 0; j < materials.Length; ++j)
            {
                if (materials[j] != m_StatusMaterial)
                    continue;

                if (m_StatusMaterials == null)
                    m_StatusMaterials = new Material[1];
                else
                    System.Array.Resize(ref m_StatusMaterials, m_StatusMaterials.Length + 1);

                m_StatusMaterials[m_StatusMaterials.Length - 1] = renderers[i].materials[j];
            }
        }

        currentlyAttached = false;
    }

    void Update()
    {
        m_LineRenderer.gameObject.SetActive(m_CreateRefraction);

        if (!m_CreateRefraction)
        {
            m_StatusMaterials[0].SetColor(c_EmissionColor, Color.green);
            ToggleLaserParticles(false, transform.position, transform.forward);
            if (m_raySwitch != null)
            {
                m_raySwitch.DisableSwith();
                m_raySwitch = null;
            }
        }

        m_CreateRefraction = false;
        m_CubeRefracted = false;

        if (m_ChangeColorMaterial)
        {
            m_StatusMaterials[0].SetColor(c_EmissionColor, Color.red);
            m_ChangeColorMaterial = false;
        }
    }

    public void CreateRefraction()
    {
        if (m_CubeRefracted)
            return;

        m_CubeRefracted = true;
        m_CreateRefraction = true;
        m_LineRenderer.enabled = true;
        m_ChangeColorMaterial = true;

        Vector3 l_EndRaycastPosition = Vector3.forward * m_MaxLineDistance;
        RaycastHit l_RaycastHit;

        if (Physics.Raycast(new Ray(m_LineRenderer.transform.position, m_LineRenderer.transform.forward),
            out l_RaycastHit, m_MaxLineDistance, m_CollisionLayerMask.value))
        {
            l_EndRaycastPosition = Vector3.forward * l_RaycastHit.distance;

            if (l_RaycastHit.collider.tag == "Cube" || l_RaycastHit.collider.tag == "Attached")
                l_RaycastHit.collider.GetComponent<RefractionCubeEffect>().CreateRefraction();
            else if (l_RaycastHit.collider.tag == "Switch")
            {
                m_raySwitch = l_RaycastHit.transform.GetComponent<SwitchController>();
                l_RaycastHit.transform.GetComponent<SwitchController>().OpenRenderLineDoor();
            }
            else if (l_RaycastHit.transform.tag == "Player")
            {
                if (!GameController.Instance.m_PlayerDied)
                {
                    l_RaycastHit.transform.GetComponent<HealthManager>()
                        .DealDamage(l_RaycastHit.transform.GetComponent<HealthManager>().m_MaxHealth);
                }
            }
        }

        ToggleLaserParticles(true, l_RaycastHit.point, l_RaycastHit.normal);
        m_LineRenderer.SetPosition(1, l_EndRaycastPosition);

        m_Collider.height = l_RaycastHit.distance + 1;
        m_Collider.center = new Vector3(0, 0, (l_RaycastHit.distance / 2));
    }

    public void Attach(ColorPanelObjectFSM attachedTo)
    {
        AttachedOnThisPanel = attachedTo;
        currentlyAttached = true;
        if (_joint != null) Destroy(_joint);
        ChangeMaterials(cubeMaterialList[0]);
    }

    public void Detach(bool force = false)
    {
        if (force) AttachedOnThisPanel.ForceDetach();
        AttachedOnThisPanel = null;
        currentlyAttached = false;
    }

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

    public void Attach()
    {
        if (_joint) Destroy(_joint);
        Physics.IgnoreCollision(m_Collider, GameController.Instance.playerComponents.PlayerController.attachedCollider, true);
        //ChangeMaterials(cubeMaterialList[1]);
    }

    public void ChangeLayers(string newLayer)
    {
        meshRenderer.gameObject.layer = LayerMask.NameToLayer(newLayer);
    }

    public void MakeTransparent(bool makeTransparent)
    {
        ChangeMaterials(!makeTransparent ? cubeMaterialList[0] : cubeMaterialList[1]);
    }

    public void Detach()
    {
        //Nothing for now
        Physics.IgnoreCollision(m_Collider, GameController.Instance.playerComponents.PlayerController.attachedCollider, false);
        //ChangeMaterials(cubeMaterialList[0]);
    }

    public void ChangeMaterials(CubeMaterials materialList)
    {
        Material[] newMaterials = new[]
        {
            Instantiate(materialList.cubeMaterial),
            Instantiate(materialList.colorIndicatorMaterial),
        };
        meshRenderer.materials = newMaterials;
    }

    private void ToggleLaserParticles(bool enable, Vector3 position, Vector3 forward)
    {
        if (laserParticles == null) return;
        Transform laserPartGO = laserParticles.gameObject.transform;
        laserPartGO.position = position;
        laserPartGO.forward = forward;
        if (enable && !laserParticles.gameObject.activeSelf)
        {
            laserParticles.gameObject.SetActive(true);
            laserParticles.Play();
        }
        else if (!enable && laserParticles.gameObject.activeSelf)
        {
            laserParticles.Stop();
            laserParticles.gameObject.SetActive(false);
        }
    }
}