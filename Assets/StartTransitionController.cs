using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartTransitionController : MonoBehaviour
{
    public string m_text;
    public Vector2 position;

    public void ShowReticle()
    {
        GameController.Instance.m_CanvasController.ShowReticle();
        AudioManager.instance.Recover("Ambience", 1);
        GameController.Instance.m_IntroFinished = true;
    }

    public void ShowText()
    {
        GameController.Instance.m_CanvasController.TextToDisplay(m_text, 30, position);
    }
}
