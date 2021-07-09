using System;
using Panels;
using UnityEngine;

[RequireComponent(typeof(Panel))]
[RequireComponent(typeof(CalculateTrajectory))]
[ExecuteInEditMode]
public class CalculateTrajectoryWrapper : MonoBehaviour
{
    Panel ColorPanel;
    CalculateTrajectory TrajectoryScript;

    private void Start()
    {
        ColorPanel = GetComponent<Panel>();
        TrajectoryScript = GetComponent<CalculateTrajectory>();
    }

    public void Update()
    {
        var position = ColorPanel.realBoostPoint;
        try
        {
            TrajectoryScript.CalculateLandingPoint(position,
                ColorPanel.PlayerFinalPosition.position, ColorPanel.TimeOfFlight, ColorPanel.Gravity);
            ColorPanel.PlayerForce = TrajectoryScript.InitialVelocity;
        }
        catch (Exception e)
        {
            if (e is NullReferenceException || e is UnassignedReferenceException)
            {
                TrajectoryScript.Calculate(position, ColorPanel.PlayerForce, ColorPanel.Gravity);
                return;
            }
            // throw;
        }
    }
}