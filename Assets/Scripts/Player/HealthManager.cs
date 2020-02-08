using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class HealthManager : MonoBehaviour
{
    [Header("General Settings")] public bool m_DisableOnDeath;
    public bool m_SpawnLootOnDeath;

    [Header("Health Settings")] public int m_MaxHealth;
    public Gradient m_Gradient;

    public Color ColorFromGradient(float value) //Between 0 to 1
    {
        return m_Gradient.Evaluate(value);
    }

    [Header("Shield Settings")] public int m_MaxShield;
    [Range(0.0f, 1.0f)] public float m_ShieldAbsorbption;
    private float m_HealthAbsorbption;

    [Header("Events")] public UnityEvent m_OnDamageTaken;
    public UnityEvent m_CharacterDeath;
    public UnityEvent m_RespawnCharacter;

    private bool m_OwnerIsPlayer;

    private int m_CurrentHealth;
    private int m_CurrentShield;

    [Header("UI Images")] public Image m_CanvasHealthBar;
    public Image m_CanvasShieldBar;

    private GameObject m_AttachedGameObject;
    private bool m_IsAttachedCharacterDead;

    void Start()
    {
        m_AttachedGameObject = this.gameObject;
        m_OwnerIsPlayer = m_AttachedGameObject == GameController.Instance.m_PlayerComponents.PlayerController.gameObject;

        m_CurrentHealth = m_MaxHealth;
        m_CurrentShield = m_MaxShield;
        m_HealthAbsorbption = 1.0f - m_ShieldAbsorbption;
        m_IsAttachedCharacterDead = (m_CurrentHealth <= 0);

        //Register into the Events
        m_OnDamageTaken.AddListener(CheckDeath);
        m_CharacterDeath.AddListener(OnCharacterDeath);
        m_RespawnCharacter.AddListener(OnCharacterRespawn);
    }

    public void DealDamage(float amount)
    {
        int l_Damage = (int) amount;
        if (m_IsAttachedCharacterDead) return;
        if (l_Damage <= 0)
        {
            Debug.LogError($"Someone sent incorrect damage values to {this.gameObject.name}");
            return;
        }

        if (m_CurrentShield > 0)
        {
            var shieldDamage = (int) Mathf.Ceil(l_Damage * m_ShieldAbsorbption);
            m_CurrentShield -= shieldDamage;
            m_CurrentHealth -= l_Damage - shieldDamage;
            if (m_CurrentShield < 0) m_CurrentShield = 0;
        }
        else
        {
            m_CurrentHealth -= l_Damage;
        }

        m_OnDamageTaken.Invoke();
        if (!m_OwnerIsPlayer)
            Debug.Log($"{this.gameObject.name} stats: {m_CurrentHealth} / {m_CurrentShield}");
    }

    //Getters
    public int GetCurrentHealth()
    {
        return m_CurrentHealth;
    }

    public int GetCurrentShield()
    {
        return m_CurrentShield;
    }

    public bool RestoreHealth(int l_Amount)
    {
        if (m_CurrentHealth >= m_MaxHealth) return false;

        int l_Difference = m_MaxHealth - m_CurrentHealth;
        m_CurrentHealth += Mathf.Min(l_Difference, l_Amount);
        if (m_CurrentHealth > m_MaxHealth) m_CurrentHealth = m_MaxHealth;

        return true;
    }

    public bool RestoreShield(int l_Amount)
    {
        if (m_CurrentShield >= m_MaxShield) return false;

        int l_Difference = m_MaxShield - m_CurrentShield;
        m_CurrentShield += Mathf.Min(l_Difference, l_Amount);
        if (m_CurrentShield > m_MaxShield) m_CurrentShield = m_MaxShield;

        return true;
    }

    ///
    /// Private listeners.
    ///
    private void CheckDeath()
    {
        if (m_CurrentHealth <= 0)
        {
            m_CurrentHealth = 0;
            m_IsAttachedCharacterDead = true;
            m_CharacterDeath.Invoke();
        }
    }

    private void OnCharacterDeath()
    {
        if (m_DisableOnDeath)
        {
            m_AttachedGameObject.SetActive(false);
        }
    }

    private void OnCharacterRespawn()
    {
        m_CurrentHealth = m_MaxHealth;
        m_CurrentShield = m_MaxShield;
        m_IsAttachedCharacterDead = !(m_CurrentHealth > 0);
        if (m_IsAttachedCharacterDead)
        {
            Debug.LogWarning($"ERROR While respawning {this.gameObject.name}");
        }
    }

    public bool IsCharacterDead()
    {
        return m_IsAttachedCharacterDead;
    }
}