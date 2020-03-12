using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using JetBrains.Annotations;
using UnityEngine;

public class SpawnerOnObjectDestroyed : MonoBehaviour
{
    public GameObject spawnedObject;
    public Transform originalTransform;

    private void Start()
    {
        originalTransform = spawnedObject.transform;
    }

    private void Update()
    {
        if (spawnedObject)
        {
            spawnedObject.transform.position = originalTransform.position;
            spawnedObject.transform.rotation = originalTransform.rotation;
        }
    }
}