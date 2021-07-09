using UnityEngine;

namespace Interfaces
{
    public interface ILaserCast
    {
        bool IsSleeping { get; }
        bool CastLaser();

        void StopCasting();
    }
}
