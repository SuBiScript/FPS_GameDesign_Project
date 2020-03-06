using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class StartTransitionController : MonoBehaviour
{
    public string m_text;
    public Vector2 position;
    public PostProcessVolume volume;

    ChromaticAberration m_chromatic = null;

    private void Start()
    {
        volume.profile.TryGetSettings(out m_chromatic);
        m_chromatic.enabled.value = true;
        m_chromatic.intensity.value = 1;
        AudioManager.instance.Play("Breathing");
        AudioManager.instance.Play("Ambience");
    }

    public void ShowReticle()
    {
        GameController.Instance.m_CanvasController.ShowReticle();
        GameController.Instance.m_IntroFinished = true;

    }

    public void ShowText()
    {
        GameController.Instance.m_CanvasController.TextToDisplay(m_text, 30, position);
    }

    public void ReduceChromatic()
    {
        AudioManager.instance.Play("MusicLevel");
        StartCoroutine(LightInterpolation());
    }

    IEnumerator LightInterpolation()
    {
        yield return new WaitForSeconds(Time.deltaTime * 0.2f);

        if (m_chromatic.intensity.value <= 0.1f)
        {
            yield return 0;
            Destroy(gameObject);
        }

        else
        {
            m_chromatic.intensity.value -= 0.005f;
            StartCoroutine(LightInterpolation());
        }
    }
}

