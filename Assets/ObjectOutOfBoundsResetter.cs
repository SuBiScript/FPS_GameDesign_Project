using System;
using UnityEngine;

public class ObjectOutOfBoundsResetter : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        try
        {
            other.gameObject.GetComponent<IRestartable>().Restart();
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.Message);
        }
    }
}