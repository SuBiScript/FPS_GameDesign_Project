using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBoostable
{
    /// <summary>
    /// The generic method for boosting any object capable of receiving a force.
    /// </summary>
    /// <param name="Force">The direction and amount of force to boost the object with.</param>
    void Boost(Vector3 Force);
}
