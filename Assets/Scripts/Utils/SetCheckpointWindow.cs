using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

#if UNITY_EDITOR
public class SetCheckPointCheat : EditorWindow
{
    [MenuItem("Cheats/Editor/Go to Checkpoint")]
    public static void ShowWindow()
    {
        GetWindow<SetCheckPointCheat>("Checkpoint cheats");
    }

    string checkPointNumberString = "-1";

    private void OnGUI()
    {
        GUILayout.Label("Write the checkpoint number: ", EditorStyles.boldLabel);

        checkPointNumberString = EditorGUILayout.TextField("Zone number", checkPointNumberString);

        if (GUILayout.Button("Set new CheckPoint"))
        {
            int checkPointNumber = 0;

            if (int.TryParse(checkPointNumberString, out checkPointNumber))
            {
                if (!Application.isPlaying)
                {
                    Debug.LogError("The game is not in play mode.");
                    return;
                }

                try
                {
                    CheckpointManager.SetNewCheckpoint(CheckpointManager.Checkpoints[checkPointNumber]);
                    GameController.Instance.playerComponents.PlayerController.gameObject.transform.position =
                        CheckpointManager.GetRespawnPoint().position;
                }
                catch (IndexOutOfRangeException)
                {
                    Debug.LogWarning("Checkpoint does not exist.");
                }
            }
            else
            {
                Debug.LogError("The zone is not a number.");
            }
        }
    }
}
#endif