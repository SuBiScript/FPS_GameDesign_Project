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
    private string[] checkpointNames=new []{"Not Initialized"};
    private int selectedCheckpoint = 0;
    void PrepareDropdown()
    {
        List<Checkpoint> checkpoints=CheckpointManager.GetCheckpoints();
        checkpointNames=new string[checkpoints.Count];
        for (int i = 0; i < checkpoints.Count; i++)
        {
            checkpointNames[i] = checkpoints[i].name;
        }
    }

    private void OnGUI()
    {
        if (Application.isPlaying)
        {
            PrepareDropdown();
        }

        selectedCheckpoint = EditorGUILayout.Popup("Checkpoint", selectedCheckpoint, checkpointNames);
        //EditorGUI.DropdownButton(new Rect(10,100,500,50),GUIContent.none, FocusType.Passive);

        if (GUILayout.Button("Teleport to Checkpoint."))
        {
            if (!Application.isPlaying)
            {
                Debug.LogError("The game is not in play mode.");
                return;
            }

            try
            {
                CheckpointManager.TeleportToCheckpoint(
                        GameController.Instance.playerComponents.PlayerController.gameObject.transform,
                        selectedCheckpoint);
                    /*CheckpointManager.SetNewCheckpoint();
                    GameController.Instance.playerComponents.PlayerController.gameObject.transform.position =
                        CheckpointManager.GetRespawnPoint().position;*/
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.Message);
            }
        }
    }
}

#endif