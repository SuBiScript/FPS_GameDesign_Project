using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ControlsMenu : MonoBehaviour
{
    public GameObject previousMenu;
    public GameObject backButton;
    private EventSystem es;

    void Start()
    {
        es = GameObject.Find("EventSystem").GetComponent<EventSystem>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            previousMenu.SetActive(true);
            this.gameObject.SetActive(false);
        }

        if (EventSystem.current.currentSelectedGameObject != backButton)
            es.SetSelectedGameObject(backButton);
    }
}
