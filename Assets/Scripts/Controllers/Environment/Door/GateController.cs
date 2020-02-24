using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateController : MonoBehaviour
{
    [SerializeField] private Color m_LockedColor = Color.red;
    [SerializeField] private Color m_UnlockedColor = Color.green;
    public AudioClip gateOpen;
    public AudioClip gateClose;
    public Material m_StatusMaterial;
    public bool m_Locked;

    private const string c_ColorText = "_Color";
    private const string c_EmissionColor = "_EmissionColor";

    private Material[] m_StatusMaterials;
    private Renderer m_renderer;

    private Animator m_DoorAnim;
    private static int s_OpenHash = Animator.StringToHash("Open");
    private AudioSource m_AudioSource;

    private void Start()
    {
        m_DoorAnim = GetComponent<Animator>();
        m_AudioSource = GetComponent<AudioSource>();

        var renderers = GetComponentsInChildren<Renderer>();
        for (int i = 0; i < renderers.Length; ++i)
        {
            var materials = renderers[i].sharedMaterials;
            for (int j = 0; j < materials.Length; ++j)
            {
                if (materials[j] != m_StatusMaterial) //the shared material will allow for a valid comparison. The instance material should not be compared.
                    continue;

                if (m_StatusMaterials == null)
                    m_StatusMaterials = new Material[1];
                else
                    System.Array.Resize(ref m_StatusMaterials, m_StatusMaterials.Length + 1);

                m_StatusMaterials[m_StatusMaterials.Length - 1] = renderers[i].materials[j]; //cache the instance material so each status light operates independently.
            }
        }

        UpdateGateStatus();
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player" && !m_Locked)
        {
            UpdateGateStatus();
            m_DoorAnim.SetBool(s_OpenHash, true);
            m_AudioSource.PlayOneShot(gateOpen);
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.tag == "Player" && !m_Locked)
        {
            m_DoorAnim.SetBool(s_OpenHash, false);
            m_AudioSource.PlayOneShot(gateClose);
        }
    }

    public void UpdateGateStatus()
    {
        if (m_StatusMaterials != null)
        {
            for (int i = 0; i < m_StatusMaterials.Length; ++i)
            {
                var locked = m_Locked;
                //m_StatusMaterials[i].SetColor(c_ColorText, locked ? m_LockedColor : m_UnlockedColor);
                m_StatusMaterials[i].SetColor(c_EmissionColor, locked ? m_LockedColor : m_UnlockedColor);
            }
        }
    }

    public void ToggleGateStatus(bool open)
    {
        if (open)
        {
            m_DoorAnim.SetBool(s_OpenHash, true);
            m_AudioSource.PlayOneShot(gateOpen);
        }

        else
        {
            m_DoorAnim.SetBool(s_OpenHash, false);
            m_AudioSource.PlayOneShot(gateClose);
        }
        m_Locked = !m_Locked;
        UpdateGateStatus();
    }

    public void SwithStatus(bool door)
    {
        m_Locked = door;
        UpdateGateStatus();
    }
}
