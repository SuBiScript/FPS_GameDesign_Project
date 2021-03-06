﻿using System;
using System.Collections.Generic;
using UnityEngine;

public static class CheckpointManager
{
    private static Dictionary<int, Checkpoint> Checkpoints = new Dictionary<int, Checkpoint>();
    private static Checkpoint initialCheckpoint;
    private static Checkpoint currentCheckpoint;

    public static bool enabledCheckpoints { get; private set; }

    public static void Init(Checkpoint defaultCheckpoint)
    {
        initialCheckpoint = defaultCheckpoint;
        EnableCheckpoints(true);
        SetNewCheckpoint(initialCheckpoint);
    }

    public static bool RegisterCheckpoint(Checkpoint checkpoint)
    {
        if (checkpoint == null) return false;

        if (Checkpoints.ContainsKey(checkpoint.ID) || Checkpoints.ContainsValue(checkpoint))
        {
            Debug.LogWarning("Checkpoint already registered!");
            return false;
        }

        Checkpoints.Add(checkpoint.ID, checkpoint);
        return true;
    }


    public static bool SetNewCheckpoint(Checkpoint checkpoint)
    {
        if (!enabledCheckpoints || Checkpoints.Count <= 0)
        {
            Debug.LogWarning("Checkpoints are disabled.");
            return false;
        }
        else if (checkpoint == null)
        {
            Debug.LogWarning("Default checkpoint not set!");
            return false;
        }

        for (byte i = 0; i < Checkpoints.Count; i++)
        {
            if (checkpoint.GetHashCode() == Checkpoints[i].GetHashCode())
            {
                currentCheckpoint = checkpoint;
                currentCheckpoint.SetCheckpoint(true);
                return true;
            }
        }

        return false;
    }

    public static bool TeleportToCheckpoint(Transform _transform, int index)
    {
        try
        {
            Checkpoint chosenCheckpoint = Checkpoints[index];
            
            _transform.position = chosenCheckpoint.respawnTransform.position;
            
            /*var newQuaternion = gameObject.transform.rotation;
            newQuaternion.y = chosenCheckpoint.respawnTransform.rotation.y; //TODO make it work with the camera.
            gameObject.transform.rotation = newQuaternion;*/
            
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            return false;
        }
    }
    
    public static void TeleportToCurrentCheckpoint(Transform _transform)
    {
        try
        {
            _transform.transform.position =
                currentCheckpoint.GetPosition(); //chosenCheckpoint.respawnTransform.position;
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message, _transform);
        }
    }
    

    private static void SetStartingCheckpoint(Checkpoint checkpoint)
    {
        if (SetNewCheckpoint(checkpoint))
        {
            Debug.LogWarning("Cannot set default checkpoint. Checkpoints are disabled.");
        }
    }

    private static void ResetAllCheckpoints()
    {
        if (!enabledCheckpoints) return;
        foreach (var dictionaryValue in Checkpoints)
        {
            dictionaryValue.Value.SetCheckpoint(false);
        }
    }

    public static Transform GetRespawnPoint()
    {
        return currentCheckpoint.respawnTransform;
    }

    public static void Restart()
    {
        try
        {
            ResetAllCheckpoints();
            SetStartingCheckpoint(initialCheckpoint);
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.Message);
        }
    }

    public static void EnableCheckpoints(bool enable)
    {
        if (enable)
        {
            if (Checkpoints.Count <= 0)
            {
                Debug.LogWarning(
                    "No checkpoints available. Set checkpoints in Respawn Manager. (In GameManager's GameObject)");
                return;
            }

            enabledCheckpoints = true;
            Debug.LogWarning("Enabling checkpoints.");
            return;
        }
        else
        {
            enabledCheckpoints = false;
            Debug.LogWarning("Disabling checkpoints.");
            return;
        }
    }

    public static void ClearListOfCheckpoints()
    {
        Checkpoints = new Dictionary<int, Checkpoint>();
    }

    public static List<Checkpoint> GetCheckpoints()
    {
        List<Checkpoint> checkpoints=new List<Checkpoint>();
        var Check = Checkpoints.Values;
        foreach (Checkpoint l_Checkpoint in Checkpoints.Values)
        {
            checkpoints.Add(l_Checkpoint);
        }

        return checkpoints;
    }
}