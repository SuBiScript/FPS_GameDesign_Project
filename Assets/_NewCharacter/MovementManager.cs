using UnityEngine;

public static class MovementManager
{
    public static void MoveGameObject(GameObject gameObject, Vector3 direction, float speed)
    {
        if (gameObject == null) return;
        gameObject.transform.position += direction * (speed * Time.deltaTime);
    }

    public static void MoveRigidbody(Rigidbody rigidbody, Vector3 direction, float speed, float fixedDeltaTime)
    {
        if (rigidbody == null) return;
        rigidbody.MovePosition(
            rigidbody.gameObject.transform.position +
            rigidbody.gameObject.transform.TransformDirection(direction) * (speed * fixedDeltaTime));
    }

    /// <summary>
    /// Movement using the AddForce method of the rigidbody.
    /// </summary>
    /// <param name="rigidbody">The rigidbody you want to move.</param>
    /// <param name="direction">Normalized direction of the force</param>
    /// <param name="force">Amount of force added. Default 10f</param>
    /// <param name="forceMode">Rigidbody forcemode parameter. Default ForceMode.Impulse</param>
    public static void RigidbodyAddForce(Rigidbody rigidbody, Vector3 direction, float force = 10f, ForceMode forceMode = ForceMode.Impulse)
    {
        if (rigidbody == null) return;
        rigidbody.AddForce(direction * force, forceMode);
    }
}