using UnityEngine;

// https://docs.unity3d.com/ScriptReference/MenuItem.html
namespace Utils
{
    public class Cheats : MonoBehaviour
    {
        public static void DamagePlayer(int damage)
        {
            if (Application.isPlaying)
            {
                GameController.Instance.playerComponents.HealthManager.DealDamage(damage);
            }
            else
            {
                Debug.LogWarning("Not in play mode.");
            }
        }

        public static void ToggleGodMode()
        {
            if (Application.isPlaying)
            {
                bool infoBool = GameController.Instance.playerComponents.HealthManager.m_GodMode =
                    !GameController.Instance.playerComponents.HealthManager.m_GodMode;
                Debug.LogWarning(infoBool ? "Enabled god mode! >:D" : "Disabled god mode! :(" );
            }
            else
            {
                Debug.LogWarning("Not in play mode.");
            }
        }
    }
}