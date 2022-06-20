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

        /// <summary>
        /// Key - origin or null,
        /// Value - stack of inactive objects
        /// </summary>
        private Dictionary<Object, Stack<Object>> inactive = new Dictionary<Object, Stack<Object>>();
        /// <summary>
        /// Key - instance,
        /// Value - original prefab
        /// </summary>
        private Dictionary<Object, Object> origins = new Dictionary<Object, Object>();
        private Stack<Object> inactiveWithoutOrigins = new Stack<Object>();

        private static ObjectPool GetPool(Type type)
        {
            ObjectPool pool;
            if (!inactiveParent)
            {
                inactiveParent = new GameObject("Object-Pool").transform;
                inactiveParent.gameObject.SetActive(false);
                pool = new ObjectPool();
                pools = new Dictionary<Type, ObjectPool> { { type, pool } };
            }
            else if (!pools.TryGetValue(type, out pool))
            {
                pool = new ObjectPool();
                pools.Add(type, pool);
            }
            return pool;
        }

        public static T Instantiate<T>(T origin, Vector3 position, Quaternion rotation, Transform parent) where T : Object
        {
            var pool = GetPool(typeof(T));
            Stack<Object> stack;
            if (origin)
                pool.inactive.TryGetValue(origin, out stack);
            else
                stack = pool.inactiveWithoutOrigins;
            if (stack != null && stack.TryPop(out var obj) && obj is T instance)
            {
                if (instance is not GameObject go)
                    go = (instance as Component).gameObject;
                go.transform.SetParent(parent);
                go.transform.SetPositionAndRotation(position, rotation);
            }
            else
            {
                instance = Object.Instantiate(origin, position, rotation, parent);
                pool.origins.Add(instance, origin);
            }
            return instance;
        }

        public static void Deactivate<T>(T instance) where T : Object
        {
            var pool = GetPool(typeof(T));
            if (instance is not GameObject go)
                go = (instance as Component).gameObject;
            go.transform.SetParent(inactiveParent);
            pool.origins.TryGetValue(instance, out var origin);
            Stack<Object> stack;
            if (!origin)
                stack = pool.inactiveWithoutOrigins;
            else if (!pool.inactive.TryGetValue(origin, out stack))
            {
                stack = new Stack<Object>();
                pool.inactive.Add(origin, stack);
            }
            stack.Push(instance);
        }
    }
}
