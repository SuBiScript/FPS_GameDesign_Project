using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MaxHeightChecker : EditorWindow
{
    [MenuItem("Tools/Custom/Max Height Checker")]
    public static void ShowWindow()
    {
        GetWindow<MaxHeightChecker>("Height Checker").Show();
    }

    public static float MaxValue = 0f;
    public static GameObject selectedObj;

    private void UpdateValues()
    {
        if (selectedObj.transform.position.y > MaxValue)
        {
            MaxValue = selectedObj.transform.position.y;
        }
    }

    private void OnGUI()
    {
        selectedObj = Selection.activeGameObject;
        EditorGUILayout.ObjectField(selectedObj, typeof(GameObject), true);
        if (selectedObj != null)
        {
            UpdateValues();   
            GUILayout.Label("Maximum Value: " + MaxValue);
        }
    }
}