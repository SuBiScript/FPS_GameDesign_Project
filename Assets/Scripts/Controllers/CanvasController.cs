using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
    [Header("Requirements")]
    public Image m_Reticle;
    public RectTransform m_textToDisplayPos;
    public Text m_textToDisplay;
    public Text m_textShadowToDisplay;

    [HideInInspector] public GameObject m_textToDisplayAnim;

    private void Awake()
    {
        m_Reticle.enabled = false;
    }

    void Start()
    {
        m_textToDisplayAnim.GetComponent<Animation>();
        m_textToDisplayPos.GetComponent<RectTransform>();
    }

    public void TextToDisplay(string text, int fontSize, Vector2 position)
    {
        m_textToDisplayAnim.SetActive(false);
        m_textToDisplayAnim.SetActive(true);
        m_textToDisplayPos.anchoredPosition = position;
        m_textToDisplay.text = text;
        m_textToDisplay.fontSize = fontSize;
        m_textShadowToDisplay.text = text;
        m_textShadowToDisplay.fontSize = fontSize;
    }

    public void TextToDisplay(string text)
    {
        m_textToDisplayAnim.SetActive(false);
        m_textToDisplayAnim.SetActive(true);
        m_textToDisplayPos.anchoredPosition = new Vector2(0, 350);
        m_textToDisplay.text = text;
        m_textToDisplay.fontSize = 35;
        m_textShadowToDisplay.text = text;
        m_textShadowToDisplay.fontSize = 35;
    }

    public void RemovingText(GameObject gameObject)
    {
        gameObject.SetActive(false);
    }

    public void ChangeReticleColor(int color)
    {
        switch (color)
        {
            case 1:
                m_Reticle.color = Color.red;
                break;
            case 2:
                m_Reticle.color = Color.green;
                break;
            case 3:
                m_Reticle.color = Color.blue;
                break;
            default:
                m_Reticle.color = Color.yellow;
                break;
        }
    }

    public void ShowReticle()
    {
        m_Reticle.enabled = true;
    }
}
