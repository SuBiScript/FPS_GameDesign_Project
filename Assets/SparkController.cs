using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class SparkController : MonoBehaviour , IRestartable
{

    [Header("Sound settings")]
    public ParticleSystem m_SparkParticle;
    private AudioSource m_AudioSource;

    public UnityEvent m_EventOnEnter;

    void Start()
    {
        m_AudioSource = GetComponent<AudioSource>();
    }

    public void Restart()
    {
        m_SparkParticle.Stop();
        m_SparkParticle.Play();
        m_AudioSource.Play();
    }

    public void PauseSound()
    {
        m_AudioSource.Pause();
    }

    public void UnpauseSound()
    {
        m_AudioSource.UnPause();
    }
}
