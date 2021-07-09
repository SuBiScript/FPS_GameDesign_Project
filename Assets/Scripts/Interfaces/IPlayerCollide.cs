using UnityEngine;

namespace Interfaces
{
    public interface IPlayerCollide
    {
        bool Collide(GameObject self, Vector3 collisionPoint);
    }
}
