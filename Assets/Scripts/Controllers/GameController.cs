using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class GameController : Singleton<GameController>
{
    [Header("Required attributes")]
    public PlayerController m_PlayerController;
    public CanvasController m_CanvasController;
    public GameObject m_PauseMenu;
    public GameObject m_GameOverMenu;
    public Image m_BloodFrame;

    KeyCode m_DebugLockAngleKeyCode = KeyCode.I;
    KeyCode m_DebugLockKeyCode = KeyCode.O;
    [HideInInspector] public bool m_AngleLocked;
    [HideInInspector] public bool m_GamePaused;
    [HideInInspector] public bool m_PlayerDied;
    
    public struct PlayerComponents
    {
        public HealthManager HealthManager;
        public PlayerController PlayerController;

        public PlayerComponents(PlayerController playerController, HealthManager healthManager)
        {
            this.PlayerController = playerController;
            this.HealthManager = healthManager;
        }
    }
    public PlayerComponents m_PlayerComponents;

    void Awake()
    {
        m_GamePaused = false;
        m_PlayerDied = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        AddPlayerComponents();
    }

    private void AddPlayerComponents()
    {
        try
        {
            var playerGameObject = FindObjectOfType<PlayerController>();
            m_PlayerComponents = new PlayerComponents(playerGameObject, playerGameObject.GetComponent<HealthManager>());
            m_PlayerComponents.HealthManager.onCharacterDeath.AddListener(OnPlayerDeath);
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
        if (Input.GetKeyDown(KeyCode.Escape) && !m_GamePaused && !m_PlayerDied)
            Pause();

#if UNITY_EDITOR
        UnityEditorUpdates();
#endif
    }
    
    /// <summary>
    /// Unity editor stuff
    /// </summary>
    #if UNITY_EDITOR
    void UnityEditorUpdates()
    {

        if (Input.GetKeyDown(m_DebugLockAngleKeyCode))
            m_AngleLocked = !m_AngleLocked;

        if (Input.GetKeyDown(m_DebugLockKeyCode))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
                Cursor.lockState = CursorLockMode.None;
            else
                Cursor.lockState = CursorLockMode.Locked;
        }

    }
#endif

    void Pause()
    {
        m_PauseMenu.SetActive(true);
        m_GamePaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0;
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