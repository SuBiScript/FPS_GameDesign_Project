using System.Collections;
using System.Collections.Generic;
using UnityEngine.Video;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CanvasController : MonoBehaviour
{
    [Header("Requirements")]
    public Image m_Reticle;
    public RectTransform m_textToDisplayPos;
    public Text m_textToDisplay;
    public Text m_textShadowToDisplay;
    public GameObject m_VideoPanel;
    public TextMeshProUGUI m_VideoPanelTextToDisplay;
    public VideoPlayer m_VideoPlayer;
    public TextMeshProUGUI m_ColorTipText;
    public VideoPlayer m_BlackScreenVideoPlayer;

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

    public void VideoToDisplay(string text, PlayerDetector.ColorPanel colorpanel, VideoClip videoClip)
    {
        m_VideoPanel.SetActive(true);
        m_VideoPlayer.clip = videoClip;
        m_VideoPanelTextToDisplay.text = text;
        GameController.Instance.m_VideoPlaying = true;
        GameController.Instance.m_GamePaused = true;
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        m_Reticle.enabled = false;

        switch (colorpanel)
        {
            case PlayerDetector.ColorPanel.Blue:
                m_ColorTipText.text = "BLUE";
                m_ColorTipText.color = Color.blue;
                break;
            case PlayerDetector.ColorPanel.Green:
                m_ColorTipText.text = "GREEN";
                m_ColorTipText.color = Color.green;
                break;
            default:
                m_ColorTipText.text = "RED";
                m_ColorTipText.color = Color.red;
                break;
        }
    }

    public void RemovePlayingVideo()
    {
        m_VideoPlayer.Stop();
        m_VideoPlayer.clip = null;
        m_VideoPanel.SetActive(false);
        GameController.Instance.m_VideoPlaying = false;
        GameController.Instance.m_GamePaused = false;
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        ShowReticle();
        m_BlackScreenVideoPlayer.Play();
    }
}
