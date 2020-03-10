using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

#if UNITY_EDITOR
public class SetCheckPointCheat : EditorWindow
{
    [MenuItem("Cheats/Editor/Teleport to Checkpoint")]
    public static void ShowWindow()
    {
        GetWindow<SetCheckPointCheat>("Player Teleporter");
    }

    string checkPointNumberString = "0";

    private void OnGUI()
    {
        GUILayout.Label("Write the checkpoint ID: ", EditorStyles.boldLabel);

        checkPointNumberString = EditorGUILayout.TextField("Checkpoint ID", checkPointNumberString);

        if (GUILayout.Button("Set new checkpoint."))
        {
            byte checkPointNumber = 0;

            if (byte.TryParse(checkPointNumberString, out checkPointNumber))
            {
                if (!Application.isPlaying)
                {
                    Debug.LogError("The game is not in play mode.");
                    return;
                }

                try
                {
                    CheckpointManager.SetObjectPositionToCheckpoint(
                        GameController.Instance.playerComponents.PlayerController.gameObject,
                        checkPointNumber);
                    /*CheckpointManager.SetNewCheckpoint();
                    GameController.Instance.playerComponents.PlayerController.gameObject.transform.position =
                        CheckpointManager.GetRespawnPoint().position;*/
                }
                catch (Exception e)
                {
                    Debug.LogWarning(e.Message);
                }
            }
            else
            {
                Debug.LogError("Introduce a number.");
            }
        }
    }
}
#endif