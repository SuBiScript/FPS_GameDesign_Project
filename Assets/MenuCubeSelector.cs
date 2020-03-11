using System.Collections;
using System.Collections.Generic;
using ColorPanels;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Weapon;

public class MenuCubeSelector : MonoBehaviour
{
    public ColorPanelObjectFSM[] options;
    public TextMeshPro TextMeshPro;
    public Animation blackFadeOut;

    [Header("Transitions")] [Range(0f, 5f)] [Tooltip("Wait time between selecting an option and fading.")]
    public float standardWaitTime = 1f;

    private int _currentOption;
    private bool optionSelected;

    private string[] texts = new[]
    {
        "Play",
        "Options",
        "Exit"
    };

    private int currentOption
    {
        get => _currentOption;
        set
        {
            if (value > options.Length - 1)
            {
                _currentOption = 0;
            }
            else if (value < 0)
            {
                _currentOption = options.Length - 1;
            }
            else
            {
                _currentOption = value;
            }

            UpdateCube();
        }
    }

    void Start()
    {
        optionSelected = false;
        foreach (ColorPanelObjectFSM panel in options)
        {
            panel.ChangeColor(WeaponScript.WeaponColor.None);
        }

        currentOption = 0;
    }

    void Update()
    {
        if (!optionSelected)
        {
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                currentOption++;
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                currentOption--;
            }
            else if (Input.GetKeyDown(KeyCode.Return))
            {
                //ACTIVATE OPTION
                options[currentOption].ChangeColor(WeaponScript.WeaponColor.Blue);
                optionSelected = true;
                switch (currentOption)
                {
                    case 0:
                        StartCoroutine(StartPlaying());
                        break;
                    case 1:
                        Debug.LogWarning("NO OPTIONS AVAILABLE! DAMN!");
                        break;
                    case 2:
                        StartCoroutine(QuitWithStyle());
                        break;
                    default:
                        Debug.LogWarning("WTF, how did you even... nevermind.");
                        break;
                }
            }
        }
    }

    void UpdateCube()
    {
        for (int i = 0; i < options.Length; i++)
        {
            if (i == currentOption)
            {
                options[currentOption].ChangeColor(WeaponScript.WeaponColor.Green);
                TextMeshPro.text = texts[currentOption];
            }
            else
            {
                options[i].ChangeColor(WeaponScript.WeaponColor.None);
            }
        }
    }

    void Play() //Play it with style, with a coroutine.
    {
        SceneManager.LoadScene("PrimersNivells");
        AudioManager.instance.Stop("Perturbator");
    }

    void Options()
    {
        //TODO SHOW THE DAMN OPTIONS!
    }

    void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    IEnumerator StartPlaying()
    {
        yield return new WaitForSeconds(standardWaitTime); //Let the cube fly...
        blackFadeOut.Play("BlackFadeIn"); //Play close animation
        while (blackFadeOut.IsPlaying("BlackFadeIn"))
        {
            yield return null;
        } //I should wait for the animation to be over...

        Debug.Log("START THE SHOW");
        Play(); //START THE SHOW!
    }

    IEnumerator QuitWithStyle()
    {
        yield return new WaitForSeconds(standardWaitTime);
        QuitGame();
    }
}