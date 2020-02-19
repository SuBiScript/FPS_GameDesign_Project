using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace Utils
{
    public class EditorCheats : MonoBehaviour
    {
        [MenuItem("Cheats/Player/Kill Player #K")]
        public static void DamagePlayer()
        {
            Cheats.DamagePlayer(999);
        }

        [MenuItem("Cheats/Player/God Mode =) #G")]
        public static void GodMode()
        {
            if (!Application.isPlaying)
            {
                Debug.LogError("Game not running");
                return;
            }
            GameController.Instance.playerComponents.HealthManager.m_GodMode = true;
        }

        [MenuItem("Cheats/Player/Normal Mode =( %#G")]
        public static void NormalMode()
        {
            if (!Application.isPlaying)
            {
                Debug.LogError("Game not running");
                return;
            }
            GameController.Instance.playerComponents.HealthManager.m_GodMode = false;
        }

        [MenuItem("Cheats/Player/Replenish Shield")]
        public static void ReplenishShields()
        {
            if (!Application.isPlaying)
            {
                Debug.LogError("Game not running");
                return;
            }
            GameController.Instance.playerComponents.HealthManager.RestoreShield(999);
        }

        public static void GiveShields(int amount)
        {
            if (!Application.isPlaying)
            {
                Debug.LogError("Game not running");
                return;
            }
            GameController.Instance.playerComponents.HealthManager.RestoreShield(amount);
        }

        [MenuItem("Cheats/Player/Replenish Health")]
        public static void GiveHealth()
        {
            if (!Application.isPlaying)
            {
                Debug.LogError("Game not running");
                return;
            }
            GameController.Instance.playerComponents.HealthManager.RestoreHealth(999);
        }

        public static void GiveHealth(int amount)
        {
            if (!Application.isPlaying)
            {
                Debug.LogError("The game is not running!");
                return;
            }
            GameController.Instance.playerComponents.HealthManager.RestoreHealth(amount);
        }
    }
}
#endif