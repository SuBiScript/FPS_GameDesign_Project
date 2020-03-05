using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class StartTransitionController : MonoBehaviour
{
    public string m_text;
    public Vector2 position;
    public float intensity = 0.1f;
    public PostProcessVolume volume;

    ChromaticAberration m_chromatic = null;
    bool m_Ischromatic;


    private void Start()
    {
        volume.profile.TryGetSettings(out m_chromatic);

        // later in this class during handling and changing
        m_chromatic.enabled.value = true;
        m_chromatic.intensity.value = 1;
    }

    //private void Update()
    //{
    //    if (m_Ischromatic)
    //        m_chromatic.intensity.value = Mathf.Lerp(1, 0.1f, 0.8f);
    //}

    public void ShowReticle()
    {
        GameController.Instance.m_CanvasController.ShowReticle();
        AudioManager.instance.Play("Ambience");
        GameController.Instance.m_IntroFinished = true;
    }

    public void ShowText()
    {
        GameController.Instance.m_CanvasController.TextToDisplay(m_text, 30, position);
    }

    //public void ReduceChromatic()
    //{
    //    m_Ischromatic = true;
    //}

    public void ReduceChromatic()
    {
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

