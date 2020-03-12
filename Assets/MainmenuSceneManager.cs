using ColorPanels;
using UnityEngine;
using Weapon;

public class MainmenuSceneManager : MonoBehaviour
{
    public ColorPanelObjectFSM[] colorPanelObject;
    void Start()
    {
        //colorPanelObject = FindObjectsOfType<ColorPanelObjectFSM>();
        if (colorPanelObject.Length <= 0) return;
        foreach (ColorPanelObjectFSM panel in colorPanelObject)
        {
            panel.ChangeColor(WeaponScript.WeaponColor.Blue);
        }

        //AudioManager.instance.Play("Perturbator");
    }
}