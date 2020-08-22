using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    public static SceneController instance;
    public GameObject m_LoadingScreen;
    public Slider m_ProgressBar;


    List<AsyncOperation> m_ScenesLoading = new List<AsyncOperation>();
    float m_TotalScreenProgress;

    void Awake()
    {
        instance = this;
        SceneManager.LoadSceneAsync((int)SceneIndexes.Tittle_Screen, LoadSceneMode.Additive);
    }

    public void LoadGame()
    {
        m_LoadingScreen.gameObject.SetActive(true);
        m_ScenesLoading.Add(SceneManager.UnloadSceneAsync((int)SceneIndexes.Tittle_Screen));
        m_ScenesLoading.Add(SceneManager.LoadSceneAsync((int)SceneIndexes.Level, LoadSceneMode.Additive));

        StartCoroutine(GetSceneLoadProgress());
    }

    public IEnumerator GetSceneLoadProgress()
    {
        for (int i = 0; i < m_ScenesLoading.Count; i++)
        {
            while (!m_ScenesLoading[i].isDone)
            {
                m_TotalScreenProgress = 0;

                foreach (AsyncOperation operation in m_ScenesLoading)
                {
                    m_TotalScreenProgress += operation.progress;
                }

                m_TotalScreenProgress = (m_TotalScreenProgress / m_ScenesLoading.Count) * 100;

                m_ProgressBar.value = Mathf.Round(m_TotalScreenProgress);

                yield return null;
            }
        }

        m_LoadingScreen.gameObject.SetActive(false);
    }
}
