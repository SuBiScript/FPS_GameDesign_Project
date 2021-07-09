using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalPositionCalculator : MonoBehaviour
{
    public float InitialVelocity;
    public float InitialPosition;
    public float Acceleration;
    [Range(0.0f, 100f)] public float Time;

    private Vector3 NewPosition;

    private void Start()
    {
        NewPosition = Vector3.zero;
        this.transform.position = new Vector3(0, InitialPosition, 0);
    }

    private void Update()
    {
        CalculatePosition();
        this.gameObject.transform.position = NewPosition;
    }

    void CalculatePosition()
    {
        NewPosition.y = InitialPosition + InitialVelocity * Time - 0.5f * Acceleration * Mathf.Pow(this.Time, 2);
    }
}