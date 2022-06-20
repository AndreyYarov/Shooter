using UnityEngine;

namespace Shooter.Utils.Pool
{
    public abstract class PoolBehaviour<T> : MonoBehaviour
        where T : PoolBehaviour<T>
    {
        public virtual void Init() { }
        public virtual void Deinit() { }
    }
}
