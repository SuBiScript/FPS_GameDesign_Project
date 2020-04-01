using System;
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
    [Header("RaycastRequired Info")] public Camera mainSceneCamera;
    public LayerMask platformLayer;

    public ColorPanelObjectFSM[] options;
    public RefractionCubeEffect selectorCube;
    public TextMeshPro TextMeshPro;
    public Animation blackFadeOut;

    public GameObject optionsPanel;
    public GameObject loadingPanel;


    [Header("Transitions")] [Range(0f, 5f)] [Tooltip("Wait time between selecting an option and fading.")]
    public float standardWaitTime = 1f;

    private Coroutine hideCube;
    private int _currentOption;
    private bool optionSelected;
    private bool inOptionsMenu;
    private bool usingKeys;

    private string[] texts = new[]
    {
        "Play",
        "Controls",
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
        usingKeys = false;
        //Set cursor visible to select the options.
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;

        AudioManager.instance.Play("Perturbator");
        optionsPanel.SetActive(false);
        optionSelected = false;
        foreach (ColorPanelObjectFSM panel in options)
        {
            panel.ChangeColor(WeaponScript.WeaponColor.None);
        }

        currentOption = 0;
        inOptionsMenu = false;
    }

    void Update()
    {
        if (!optionSelected)
        {
            if ((Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) && !inOptionsMenu)
            {
                currentOption++;
                usingKeys = true;
            }
            else if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) && !inOptionsMenu)
            {
                currentOption--;
                usingKeys = true;
            }
            else if ((Input.GetAxis("Mouse X") > 0 || Input.GetAxis("Mouse Y") > 0 || !usingKeys ||
                      Input.GetMouseButtonDown(0)) && !inOptionsMenu)
            {
                usingKeys = false;
                Ray mouseRay = mainSceneCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit rayInfo;
                if (Physics.Raycast(mouseRay, out rayInfo, 250, platformLayer))
                {
                    try
                    {
                        SetAsCurrentOption(rayInfo.transform.GetComponent<ColorPanelObjectFSM>());
                        if (Input.GetMouseButtonDown(0))
                        {
                            ActivateOption();
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e.Message);
                    }
                }
            }
            else if (Input.GetKeyDown(KeyCode.Return))
            {
                ActivateOption();
            }
            else if (Input.GetKeyDown(KeyCode.Escape) && inOptionsMenu)
            {
                Options();
            }
        }
    }

    void ActivateOption()
    {
        //ACTIVATE OPTION
        switch (currentOption)
        {
            case 0:
                options[currentOption].ChangeColor(WeaponScript.WeaponColor.Blue);
                StartCoroutine(StartPlaying());
                optionSelected = true;
                break;
            case 1:
                Options();
                break;
            case 2:
                options[currentOption].ChangeColor(WeaponScript.WeaponColor.Blue);
                StartCoroutine(QuitWithStyle());
                optionSelected = true;
                break;
            default:
                Debug.LogWarning("WTF, how did you even... nevermind.");
                break;
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
        try
        {
            CheckpointManager.ClearListOfCheckpoints();
            CheckpointManager.Restart();
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    void Options()
    {
        //TODO SHOW THE DAMN OPTIONS!
        if (optionsPanel.activeSelf)
        {
            optionsPanel.SetActive(false);
            options[currentOption].ChangeColor(WeaponScript.WeaponColor.Green);
            selectorCube.gameObject.SetActive(true);
            selectorCube.Restart();
            StopCoroutine(hideCube);
            inOptionsMenu = false;
        }
        else
        {
            optionsPanel.SetActive(true);
            hideCube = ExecuteCoroutine(hideCube, CubeStopper());
            inOptionsMenu = true;
        }
    }

    void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    void SetAsCurrentOption(ColorPanelObjectFSM newPanel)
    {
        for (int i = 0; i < options.Length; i++)
        {
            if (newPanel.GetHashCode() == options[i].GetHashCode())
            {
                currentOption = i;
                return;
            }
        }
    }

    private Coroutine ExecuteCoroutine(Coroutine l_CoroutineHolder, IEnumerator l_MethodName)
    {
        if (l_CoroutineHolder != null)
            StopCoroutine(l_CoroutineHolder);
        return StartCoroutine(l_MethodName);
    }

    IEnumerator CubeStopper()
    {
        yield return new WaitForSeconds(standardWaitTime);

        selectorCube.gameObject.SetActive(false);
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