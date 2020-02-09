using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefractionCubeEffect : MonoBehaviour
{
    public LineRenderer m_LineRenderer;
    public LayerMask m_CollisionLayerMask;
    [Range(1f, 400.0f)] public float m_MaxLineDistance;
    bool m_CreateRefraction;
    bool m_CubeRefracted;

    //material
    public Material m_StatusMaterial;
    private const string c_EmissionColor = "_EmissionColor";
    private Material[] m_StatusMaterials;
    private bool m_ChangeColorMaterial;

    //dinamyc collider
    public CapsuleCollider m_Collider;

    private void Start()
    {
        m_Collider = GetComponentInChildren<CapsuleCollider>();

        var renderers = GetComponents<Renderer>();
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
    }

    void Update()
    {
        m_LineRenderer.gameObject.SetActive(m_CreateRefraction);

        if (!m_CreateRefraction)
            m_StatusMaterials[0].SetColor(c_EmissionColor, Color.green);

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

        if (Physics.Raycast(new Ray(m_LineRenderer.transform.position, m_LineRenderer.transform.forward), out l_RaycastHit, m_MaxLineDistance, m_CollisionLayerMask.value))
        {
            l_EndRaycastPosition = Vector3.forward * l_RaycastHit.distance;

            if (l_RaycastHit.collider.tag == "Cube")
                l_RaycastHit.collider.GetComponent<RefractionCubeEffect>().CreateRefraction();
            else if (l_RaycastHit.collider.tag == "Switch")
            {
                l_RaycastHit.transform.GetComponent<SwitchController>().OpenRenderLineDoor();
            }
            else if (l_RaycastHit.transform.tag == "Player")
            {
                if (!GameController.Instance.m_PlayerDied)
                {
                    l_RaycastHit.transform.GetComponent<HealthManager>().DealDamage(l_RaycastHit.transform.GetComponent<HealthManager>().m_MaxHealth);
                }
            }
        }
        m_LineRenderer.SetPosition(1, l_EndRaycastPosition);

        m_Collider.height = l_RaycastHit.distance;
        m_Collider.center = new Vector3(0, 0, (l_RaycastHit.distance / 2));
    }
}
