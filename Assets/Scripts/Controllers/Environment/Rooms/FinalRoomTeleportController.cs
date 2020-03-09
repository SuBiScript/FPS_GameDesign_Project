using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalRoomTeleportController : MonoBehaviour
{

    [Header("Required attributes")]
    [SerializeField] private string m_Text;
    [SerializeField] private GameObject m_WhiteFadeOut;

    public void WhiteFadeOut()
    {
        m_WhiteFadeOut.GetComponent<Animator>().SetBool("WhiteFadeOut", true);
        AudioManager.instance.Play("TeleportMenu");
        AudioManager.instance.Stop("MusicLevel");
        AudioManager.instance.Stop("Ambience");
        Invoke("FadeToNextScene", 5);
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            GameController.Instance.m_CanvasController.TextToDisplay(m_Text.ToUpper(), 30, new Vector2(0, -450));
            //GameController.Instance.playerComponents.PlayerController.stateMachine.enabled = false;
            GameController.Instance.m_PlayerDied = true;
            GameController.Instance.playerComponents.PlayerController.weaponAnimator.SetBool("Walking", false);

            Invoke("WhiteFadeOut", 2);
        }
    }

    public void FadeToNextScene()
    {
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        SceneManager.LoadScene("MainMenu");
        yield return null;
    }
}
