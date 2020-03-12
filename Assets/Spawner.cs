using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject objectToSpawn;
    public Transform spawnAtPosition;
    [Range(0, byte.MaxValue)]public byte delay;
    public bool spawnObject;

    [CanBeNull] private IEnumerator spawnCoroutine;
    void Start()
    {
        spawnCoroutine = CubeSpawner();
        if(spawnObject) RestartCoroutine();
    }

    IEnumerator CubeSpawner()
    {
        while (spawnObject)
        {
            yield return new WaitForSeconds(delay);
            Instantiate(objectToSpawn, spawnAtPosition.position, Quaternion.identity);
        }
    }

    void RestartCoroutine()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
        }
        StartCoroutine(spawnCoroutine);
    }

    private void OnValidate()
    {
        if (!Application.isPlaying) return;
        if(spawnObject) RestartCoroutine();
    }

    private void OnApplicationQuit()
    {
        StopCoroutine(spawnCoroutine);
    }
}
