using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SwitchController : MonoBehaviour
{
    public UnityEvent m_EventOnEnter;
    bool m_LineButtonOpen = false;
    public GateController gate;

    public AudioClip switchEnable;
    public AudioClip switchDisable;
    private AudioSource _audioSource;

    //emmision
    public Material m_StatusMaterial;
    private const string c_EmissionColor = "_EmissionColor";
    private Material[] m_StatusMaterials;


    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
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

    public void OpenRenderLineDoor()
    {
        if (!m_LineButtonOpen)
        {
            m_StatusMaterials[0].SetColor(c_EmissionColor, Color.green);
            m_LineButtonOpen = true;
            m_EventOnEnter.Invoke();
            PlaySound(switchEnable);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Laser")
        {
            DisableSwith();
        }
    }

    public void DisableSwith()
    {
        m_StatusMaterials[0].SetColor(c_EmissionColor, Color.red);
        gate.SwithStatus(true);
        m_LineButtonOpen = false;
        PlaySound(switchDisable);
    }

    private void PlaySound(AudioClip clip)
    {
        if (_audioSource == null)
        {
            _audioSource = new AudioSource();
            _audioSource.playOnAwake = false;
            _audioSource.spatialBlend = 1f;
            _audioSource.volume = 0.5f;
        }

        _audioSource.clip = clip;
        _audioSource.Play();
    }
}