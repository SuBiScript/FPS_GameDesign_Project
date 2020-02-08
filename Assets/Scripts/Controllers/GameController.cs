using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class GameController : Singleton<GameController>
{
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
        var playerGameObject = FindObjectOfType<PlayerController>();
        m_PlayerComponents = new PlayerComponents(playerGameObject, playerGameObject.GetComponent<HealthManager>());
    }
    
    
    void Update()
    {
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
}