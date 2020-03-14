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

    [CanBeNull] private Coroutine spawnCoroutine;
    void Start()
    {
        spawnCoroutine = executeCoroutine(spawnCoroutine, CubeSpawner());
    }

    IEnumerator CubeSpawner()
    {
        while (spawnObject)
        {
            yield return new WaitForSeconds(delay);
            Instantiate(objectToSpawn, spawnAtPosition.position, Quaternion.identity);
        }
    }

    Coroutine executeCoroutine(Coroutine coroutineHolder, IEnumerator coroutine)
    {
        if(coroutineHolder != null) StopCoroutine(coroutineHolder);
       return StartCoroutine(coroutine);
    }

    private void OnValidate()
    {
        if (!Application.isPlaying) return;
        if(spawnObject) spawnCoroutine = executeCoroutine(spawnCoroutine, CubeSpawner());
    }

    private void OnApplicationQuit()
    {
        StopCoroutine(spawnCoroutine);
    }
}
