using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class GameController : Singleton<GameController>
{
    [Header("Required attributes")] public CanvasController m_CanvasController;
    public GameObject m_PauseMenu;
    public GameObject m_GameOverMenu;
    public Image m_BloodFrame;
    public GameObject m_Intro;
    public bool m_NoIntro;

    [HideInInspector] public bool m_GamePaused;
    [HideInInspector] public bool m_PlayerDied;
    [HideInInspector] public bool m_IntroFinished;

    public struct PlayerComponents
    {
        public readonly HealthManager HealthManager;
        public readonly PlayerControllerFSM PlayerController;

        public PlayerComponents(PlayerControllerFSM playerController, HealthManager healthManager)
        {
            this.PlayerController = playerController;
            this.HealthManager = healthManager;
        }
    }

    public PlayerComponents playerComponents;

    void Awake()
    {
        m_GamePaused = false;
        m_PlayerDied = false;

        AddPlayerComponents();
    }

    private void Start()
    {
        if (m_NoIntro)
        {
            m_Intro.gameObject.SetActive(false);
            m_CanvasController.ShowReticle();
            m_IntroFinished = true;
        }
    }

    private void AddPlayerComponents()
    {
        try
        {
            var playerGameObject = FindObjectOfType<PlayerControllerFSM>();
            playerComponents = new PlayerComponents(playerGameObject, playerGameObject.GetComponent<HealthManager>());
            playerComponents.HealthManager.onCharacterDeath.AddListener(OnPlayerDeath);
        }
        catch (NullReferenceException)
        {
            Debug.LogError("FATAL ERROR. Player components not found.");
            Time.timeScale = 0f;
        }
    }

    public void OnPlayerDeath() //TODO Add player death functionality, a.k.a. show menus or play sounds.
    {
        if (m_PlayerDied) return;

        m_PlayerDied = true;
        m_BloodFrame.gameObject.SetActive(true);
        Invoke("GameOver", 1.5f);
        Debug.LogWarning("Player has died.");
    }


    void Update()
    {
        if (Input.GetButtonDown("Cancel") && !m_GamePaused && !m_PlayerDied && m_IntroFinished) //TODO Readd pause menu
            Pause();
    }


    public void Pause()
    {
        if (m_PauseMenu != null)
            m_PauseMenu.SetActive(true);
        m_GamePaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0;
        if (m_CanvasController != null)
            m_CanvasController.gameObject.SetActive(false);
    }

    public void GameOver()
    {
        //AudioManager.instance.StopAllSounds();
        m_GameOverMenu.SetActive(true);
        m_GamePaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0;
    }
}