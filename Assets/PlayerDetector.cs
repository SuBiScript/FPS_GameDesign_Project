using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Video;
using UnityEngine;
using UnityEngine.Events;


[RequireComponent(typeof(Collider))]
public class PlayerDetector : MonoBehaviour, IRestartable
{
    public enum ColorPanel { Blue, Green, Red };

    [Header("Settings")]
    public bool StartEnabled;
    public ColorPanel m_ColorPanelTutorial = ColorPanel.Blue;
    public VideoClip m_VideoClip;
    public String m_TipToDisplay;
    public UnityEvent m_ChangeRoomsStatus;

    [Space(10)]
    [Tooltip("Invoked when Player enters the trigger zone. Can be reset.")]
    //public UnityEvent OnPlayerDetection;

    [Header("Components")] private Collider attachedCollider;
    [Header("Variables")] private bool HasBeenTriggered;

    [SerializeField]
    private TutorialChange m_TutorialChange;

    private void Awake()
    {
        attachedCollider = GetComponent<Collider>();
        if (StartEnabled)
        {
            attachedCollider.enabled = StartEnabled;
            HasBeenTriggered = !StartEnabled;
        }
        attachedCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        ManageCollision(other);
    }
    
    /// <summary>
    /// Switches between enabling and disabling the detector.
    /// </summary>
    /// <param name="enable"> Set if enable.</param>
    /// <returns>Returns true if no error ocurred, otherwise returns false.</returns>
    public bool EnableDetector(bool enable)
    {
        try
        {
            HasBeenTriggered = false;
            attachedCollider.enabled = true;
            return true;
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.Message);
            return false;
        }
    }
    
    private void ManageCollision(Collider other)
    {
        try
        {
            if (!HasBeenTriggered && other.gameObject.GetComponent<PlayerControllerFSM>() != null)
            {
                //OnPlayerDetection.Invoke();
                m_TutorialChange?.Invoke(m_TipToDisplay, m_ColorPanelTutorial, m_VideoClip);
                HasBeenTriggered = true;
                attachedCollider.enabled = false;
                m_ChangeRoomsStatus.Invoke();
                return;
            }
        }
        catch (NullReferenceException)
        {
        }
    }

    public void Restart()
    {
        EnableDetector(true);
    }
}