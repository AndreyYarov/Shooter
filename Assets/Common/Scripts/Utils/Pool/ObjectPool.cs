using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Shooter.Utils.Pool
{
    public class ObjectPool
    {
        private static Transform inactiveParent;
        private static Dictionary<Type, ObjectPool> pools;

        private Stack<Object> inactiveInstances = new Stack<Object>();

        public static T Instantiate<T>(T origin, Vector3 position, Quaternion rotation, Transform parent) where T : PoolBehaviour<T>
        {
            T beh;
            if (pools != null && pools.TryGetValue(typeof(T), out var pool) && pool.inactiveInstances.TryPop(out Object obj))
            {
                beh = (T)obj;
                beh.transform.SetParent(parent);
                beh.transform.SetPositionAndRotation(position, rotation);
            }
            else
                beh = Object.Instantiate(origin, position, rotation, parent);
            beh.Init();
            return beh;
        }

        public static void Destroy<T>(T beh) where T : PoolBehaviour<T>
        {
            if (!inactiveParent)
            {
                inactiveParent = new GameObject("Object-Pool").transform;
                inactiveParent.gameObject.SetActive(false);
                pools = new Dictionary<Type, ObjectPool>();
            }
            if (!pools.TryGetValue(typeof(T), out var pool))
            {
                pool = new ObjectPool();
                pools.Add(typeof(T), pool);
            }
            beh.Deinit();
            beh.transform.SetParent(inactiveParent);
            pool.inactiveInstances.Push(beh);
        }
    }
}
