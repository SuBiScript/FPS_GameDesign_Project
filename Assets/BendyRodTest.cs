using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class BendyRodTest : MonoBehaviour
{
    [OnValueChanged("UpdateLR")] public bool ShowLR = false;
    public LineRenderer LR;

    public GameObject Rigid;

    public GameObject Bent;

    public Transform AttractionPoint;

    [Range(2, 50)] public int iterations = 10;

    [OnValueChanged("AdaptJoint")] [Range(1f, 10f)]
    public float length = 5f;

    [Range(1f, 2f)] public float maximumBending = 1.5f;

    public ConfigurableJoint EndPointJoint;

    private List<Vector3> CalculatedPoints;


    void AdaptJoint()
    {
        EndPointJoint.anchor = new Vector3(0f, 0f, length);
    }

    void UpdateLR()
    {
        LR.enabled = ShowLR;
    }

    //private void Awake()
    //{
    //   CalculatedPoints = new List<Vector3>();
    //}

    private void Start()
    {
        Rigid = FindObjectOfType<PlayerMover>().gameObject.GetComponentInChildren<Rigidbody>().gameObject;
        AttractionPoint = Rigid.GetComponentsInChildren<Rigidbody>()[1].gameObject.transform;            
        AdaptJoint();
    }

    void Update()
    {
        CalculateBeam();
    }


    // VERSION 2 NEW FORMULA OF MOVING THE OBJECT WITH CANTILLIAN BEAM PHYSICS.
    private void CalculateBeam()
    {
        CalculatedPoints = new List<Vector3>();
        // w = x^2(3L - x)
        // Where: L = maximum length of the beam.
        // x = position along the beam.
        // "Because we know where the deflected tip is, at the tip of the rod..."
        // x = L. So we rewrite the formula using w = L^2(3L - L). w = L^2*2L. w = 2L^3
        // This is the maximumd deflection the rod can achieve. We actually want many positions ALONG the beam.
        // So we use this: w = x^2(3L - x) / 2L^3.

        // So, here's my thought. We have two vectors, or two lines. The rigid and the bent.
        // The ratio is how much the position deviates from being rigid.
        // 

        // First iteration
        // Works well, pretty well!
        // Props to this guy!
        // https://www.youtube.com/watch?v=SYSeoTwG_qk
        //int iterations = 10;
        //float length = 1f;

        // CalculatedPoints = new List<Vector3>(iterations + 1);
        Vector3 CalculatedPosition = Vector3.zero;
        Vector3 A, B, AB, C;
        var position = Rigid.transform.position;
        for (float i = 0; i <= iterations; i++)
        {
            // DONE Note that THIS only works with a rod that measures 1 UNIT OF DISTANCE.
            // You have to consider the forward AND the actual rod distance for the points. 
            // Then, use that new position in the bendy percentage to get the final positions.

            float iterator = (i / iterations);
            A = position + Rigid.transform.forward * length * iterator;
            B = Bent.transform.position + Bent.transform.forward * length * iterator;

            AB = B - A;
            CalculatedPosition = A + AB * GetBendyPercentage(iterator, maximumBending);

            C = position + (CalculatedPosition - position) * A.magnitude;

            CalculatedPoints.Add(CalculatedPosition);
            // LR.SetPosition((int) i, CalculatedPosition);
        }

        if (ShowLR)
        {
            LR.positionCount = iterations + 1;
            LR.SetPositions(CalculatedPoints.ToArray());
        }

        Transform transform1;
        (transform1 = AttractionPoint.transform).position = GetLastPoint();
        transform1.forward = Rigid.transform.parent.transform.parent.forward;
    }

    private float GetBendyPercentage(float x, float L)
    {
        return Mathf.Pow(x, 2) * (3f * L - x) / 2f * Mathf.Pow(L, 3);
    }

    public Vector3 GetLastPoint()
    {
        return CalculatedPoints[CalculatedPoints.Count - 1];
    }
}