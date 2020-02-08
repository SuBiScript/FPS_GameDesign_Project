using UnityEngine.Audio;
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [HideInInspector] public bool volumeChange;
    [HideInInspector] public bool extend;
    [HideInInspector] public bool volumeChangeSteps;
    [HideInInspector] public bool extendStepsSound;

    public Sound[] sounds;
    public static AudioManager instance;
    private AudioSource[] m_AllSounds;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.Log("Sound: " + name + " not found");
            return;
        }
        s.source.Play();
    }

    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.Stop();
    }

    public void Reset(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.volume = 0;
        s.source.Stop();
    }

    public void Recover(string name, float volume)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.volume = volume;
        s.source.Play();
    }

    //method to reduce sound progressively (general)
    public void ReduceSound(string name, float delay, float volume)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.Log("Sound: " + name + " not found");
            return;
        }

        StartCoroutine(ReduceSound(s, delay, volume));
    }

    IEnumerator ReduceSound(Sound s, float delay, float volume)
    {
        yield return new WaitForSeconds(delay);

        s.source.volume -= volume;

        if (s.source.volume > 0f)
            StartCoroutine(ReduceSound(s, delay, volume));
        else
            s.source.Stop();
    }

    public void StopAllSounds()
    {
        m_AllSounds = FindObjectsOfType<AudioSource>();

        foreach (AudioSource audio in m_AllSounds)
        {
            audio.Stop();
        }
    }
}