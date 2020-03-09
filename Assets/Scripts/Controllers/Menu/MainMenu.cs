using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject[] buttons;
    //public ParticleSystem[] particles;

    private EventSystem es;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        es = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        AudioManager.instance.Play("Perturbator");
    }

    void Update()
    {
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

    public void PlayButtonSound()
    {
        //AudioManager.instance.Play("MenuButton2");
    }

    public void Play()
    {
        SceneManager.LoadScene("PrimersNivells");
        AudioManager.instance.Stop("Perturbator");
    }

    public void Quit()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}