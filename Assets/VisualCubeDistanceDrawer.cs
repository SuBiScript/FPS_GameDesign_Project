#if UNITY_EDITOR
using Panels;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

public class VisualCubeDistanceDrawer : OdinEditorWindow
{
    [MenuItem("Tools/Custom/Cube Visual Distance Drawer")]
    private static void OpenWindow()
    {
        var window = GetWindow<VisualCubeDistanceDrawer>();

        window.position = GUIHelper.GetEditorWindowRect().AlignCenter(200, 50);
    }

    public Panel ColorPanel;
    public CalculateTrajectory TrajectoryScript;
    [Range(1f, 5f)] public float Time = 2f;
    [Range(1f, 50f)] public float Gravity = 20f;

    public void OnInspectorUpdate()
    {
        if (ColorPanel == null || TrajectoryScript == null)
            return;
        
        TrajectoryScript.CalculateLandingPoint(ColorPanel.transform.position, ColorPanel.PlayerFinalPosition.position, Time, Gravity);
        ColorPanel.PlayerForce = TrajectoryScript.InitialVelocity;
    }
}
#endif