using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlickering : MonoBehaviour
{
    public float maxLightFlicker;

    public float minLightFlicker;

    public float currentFlicker;

    public float maxFlickerDeviation;

    public Light light;
    public Color originalColor;

    private System.Random randomGen = new System.Random(Guid.NewGuid().GetHashCode());

    void Start()
    {
        light = GetComponent<Light>();
        originalColor = light.color;
    }

    void Update()
    {
        
    }
}
