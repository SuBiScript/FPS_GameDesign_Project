using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PauseController : MonoBehaviour
{
    public bool gameOver;
    public GameObject[] buttons;

    private EventSystem es;

    void Start()
    {
        es = GameObject.Find("EventSystem").GetComponent<EventSystem>();
    }

    void Update()
    {
        if ((Input.GetButtonDown("Cancel") && !gameOver))
            Resume();

        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S))
            if (!AnyButtonSelected())
                es.SetSelectedGameObject(buttons[0]);
    }

    bool AnyButtonSelected()
    {
        for (int b = 0; b < buttons.Length; b++)
        {
            if (EventSystem.current.currentSelectedGameObject == buttons[b])
                return true;
        }
        return false;
    }

    public void Resume()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1;
        gameObject.SetActive(false);
        GameController.Instance.m_CanvasController.gameObject.SetActive(true);
        GameController.Instance.m_GamePaused = false;
        GameController.Instance.m_CanvasController.m_textToDisplayAnim.SetActive(false);
    }

    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    public void ExitToMenu()
    {
        Time.timeScale = 1;

        if (FindObjectOfType<AudioManager>() != null)
        {
            SceneManager.LoadScene("MainMenu");
            AudioManager.instance.StopAllSounds();
        }
    }


    public void Quit()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
