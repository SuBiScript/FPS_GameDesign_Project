﻿using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public static class CheckpointManager
{
    public static Checkpoint[] Checkpoints { get; private set; }
    private static Checkpoint initialCheckpoint;
    private static Checkpoint currentCheckpoint;

    public static void Init(Checkpoint defaultCheckpoint, Checkpoint[] checkpoints)
    {
        Checkpoints = checkpoints;
        initialCheckpoint = defaultCheckpoint;
        EnableCheckpoints(true);
        SetNewCheckpoint(initialCheckpoint);
    }

    public static bool enabledCheckpoints { get; private set; }

    public static bool SetNewCheckpoint(Checkpoint checkpoint)
    {
        if (!enabledCheckpoints || Checkpoints.Length <= 0)
        {
            Debug.LogWarning("Checkpoints are disabled.");
            return false;
        }
        else if (checkpoint == null)
        {
            Debug.LogWarning("Default checkpoint not set!");
            return false;
        }

        for (int i = 0; i < Checkpoints.Length; i++)
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
        foreach (Checkpoint checkpoint in Checkpoints)
        {
            checkpoint.SetCheckpoint(false);
        }
    }

    public static Transform GetRespawnPoint()
    {
        return currentCheckpoint.respawnPosition;
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
            if (Checkpoints.Length <= 0)
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
}