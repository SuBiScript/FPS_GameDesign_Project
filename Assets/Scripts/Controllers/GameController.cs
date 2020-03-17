using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameController : Singleton<GameController>
{
    [Header("Required attributes")] public CanvasController m_CanvasController;
    public GameObject m_PauseMenu;
    public GameObject m_GameOverMenu;
    public Image m_BloodFrame;
    public GameObject m_Intro;
    public bool m_NoIntro = true;
    public Animation m_BlackFade;
    [Header("Checkpoint settings")] public Checkpoint defaultCheckpoint;
    private List<IRestartable> _restartables = new List<IRestartable>();

    public bool startGame;

    [HideInInspector] public SparkController[] m_sparks;

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

        if (m_NoIntro)
        {
            if (m_Intro != null)
                m_Intro.gameObject.SetActive(false);
        }

        var tempEnum = FindObjectsOfType<MonoBehaviour>().OfType<IRestartable>();
        foreach (IRestartable item in tempEnum)
        {
            _restartables.Add(item);
        }

        m_sparks = FindObjectsOfType<SparkController>();
    }

    private void Start()
    {
        if (m_NoIntro)
        {
            if (m_CanvasController != null)
                m_CanvasController.ShowReticle();
            m_IntroFinished = true;
        }
        else
        {
            if (m_BlackFade != null)
            {
                m_BlackFade.Play();
            }
        }

        CheckpointManager.Init(defaultCheckpoint);

        if (startGame)
        {
            CheckpointManager.SetObjectPositionToCheckpoint(playerComponents.PlayerController.gameObject);
            
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

    public void OnPlayerDeath()
    {
        if (m_PlayerDied) return;

        AudioManager.instance.Play("Scream");
        m_PlayerDied = true;
        m_BloodFrame.gameObject.SetActive(true);
        Invoke("GameOver", 1.5f);
        Debug.LogWarning("Player has died.");
    }

    public void ReloadGame()
    {
        foreach (IRestartable item in _restartables)
        {
            item.Restart();
        }

        m_PlayerDied = false;
        playerComponents.HealthManager.onCharacterRespawn.Invoke();
        m_BloodFrame.gameObject.SetActive(false);
        m_BlackFade.Stop();
        m_BlackFade.Play();
        //TODO Restart all restartable objects.
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

        foreach (SparkController s in m_sparks)
        {
            s.PauseSound();
        }
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