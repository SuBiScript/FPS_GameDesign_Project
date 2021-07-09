using Sirenix.OdinInspector;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(LineRenderer))]
public class CalculateTrajectory : MonoBehaviour
{
    [Range(1, 25)] public int CalculatedSteps;
    public Vector3 InitialPosition;
    public Vector3 InitialVelocity;
    [Range(0f, 100f)] public float Gravity = 10f;
    private LineRenderer LR;

    [Header("Target Settings")] [OnValueChanged("UpdateLine")] public Transform LandingPoint;
    [Range(0.01f, 30f)] public float Time = 3f;

    [Header("Material")]
     [OnValueChanged("UpdateLRMaterial")] public Material LineRendererMaterial;

    private void Awake()
    {
        LR = GetComponent<LineRenderer>();
        if (LR == null)
            LR = new LineRenderer();
        
        if(LineRendererMaterial != null)
            UpdateLRMaterial();
        
        InitialPosition = this.gameObject.transform.position;
    }

    public void UpdateLine()
    {
        if (LandingPoint == null)
        {
            Calculate(InitialPosition, InitialVelocity, Gravity, CalculatedSteps);
        }
        else
        {
            CalculateLandingPoint(InitialPosition, LandingPoint.transform.position, Time, Gravity,
                CalculatedSteps);
        }
    }

    public void CalculateLandingPoint(Vector3 InitialPosition, Vector3 LandingPoint, float Time = 2f,
        float Gravity = 20f, int Spaces = 20)
    {
        float XVel = 0f, YVel = 0f, ZVel = 0f;

        XVel = (LandingPoint.x - InitialPosition.x) / Time;
        YVel = (LandingPoint.y - InitialPosition.y) / Time + Gravity * Time / 2;
        ZVel = (LandingPoint.z - InitialPosition.z) / Time;

        InitialVelocity = new Vector3(XVel, YVel, ZVel);
        Calculate(InitialPosition, InitialVelocity, Gravity, Spaces);
    }

    public void Calculate(Vector3 InitialPosition, Vector3 InitialVelocity, float Gravity = 20f, int Spaces = 20)
    {
        float ToF = 2 * InitialVelocity.y / Gravity; // Time of flight. (Time it takes to reach the 0 mark.)
        
        // Separate the time of flight for how many points we want. For default 10 now.
        // Probably ToF/Spaces * i; This should be the time travelled for each point and based on that we can get the position.
        //LR.SetPosition();
        LR.positionCount = Spaces + 1;

        float Time;
        float X, Y, Z;
        for (float i = 0; i <= Spaces; i++)
        {
            Time = ToF * (i / Spaces);
            X = InitialPosition.x + InitialVelocity.x * Time;
            Y = InitialPosition.y + InitialVelocity.y * Time - (0.5f * Gravity * Mathf.Pow(Time, 2));
            Z = InitialPosition.z + InitialVelocity.z * Time;
            LR.SetPosition((int) i, new Vector3(X, Y, Z));
        }

        //Time = ToF;
    }

    public void UpdateLRMaterial()
    {
        LR.material = LineRendererMaterial;
    }
}