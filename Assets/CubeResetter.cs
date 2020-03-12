using System;
using UnityEngine;

public class CubeResetter : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        try
        {
            other.GetComponent<IRestartable>().Restart();
            other.gameObject.SetActive(false);
        }
        catch (NullReferenceException)
        {
            
        }
    }
}