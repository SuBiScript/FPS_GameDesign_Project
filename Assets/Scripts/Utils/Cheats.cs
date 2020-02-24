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
                Debug.LogError("Not in play mode.");
            }
        }
    }
}