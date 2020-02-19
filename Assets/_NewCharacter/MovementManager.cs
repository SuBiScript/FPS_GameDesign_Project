using System.Collections;
using System.Collections.Generic;
using UnityEditor.Profiling.Memory.Experimental;
using UnityEngine;

public static class MovementManager
{
    public static void MoveGameObject(GameObject gameObject, Vector3 direction, float speed)
    {
        if (gameObject == null) return;
        gameObject.transform.position += direction * (speed * Time.deltaTime);
    }

    public static void MoveRigidbody(Rigidbody rigidbody, Vector3 direction, float speed)
    {
        if (rigidbody == null) return;
        rigidbody.MovePosition(
            rigidbody.transform.position +
            rigidbody.transform.TransformDirection(direction) * (speed * Time.fixedTime));
    }

    /// <summary>
    /// Movement using the AddForce method of the rigidbody.
    /// </summary>
    /// <param name="rigidbody">The rigidbody you want to move.</param>
    /// <param name="direction">Normalized direction of the force</param>
    /// <param name="force">Amount of force added</param>
    /// <param name="forceMode">Rigidbody forcemode parameter</param>
    public static void RigidbodyAddForce(Rigidbody rigidbody, Vector3 direction, float force, ForceMode forceMode)
    {
        if (rigidbody == null) return;
        rigidbody.AddForce(direction * force, forceMode);
    }
}