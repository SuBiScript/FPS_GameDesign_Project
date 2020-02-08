using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sound
{
    [HideInInspector] public AudioSource source;

    [Range(0f, 1f)] public float volume;
    [Range(1f, 3f)] public float pitch;

    public string name;
    public AudioClip clip;
    public bool loop;
}