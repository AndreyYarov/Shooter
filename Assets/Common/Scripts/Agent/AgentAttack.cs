using System;
using UnityEngine;
using Shooter.Shell;
using Shooter.Utils.Pool;
using Random = UnityEngine.Random;

namespace Shooter.Agent
{
    public class AgentAttack : MonoBehaviour
    {
        [SerializeField, Min(0.5f)] private float m_MinFireDelay = 4f;
        [SerializeField, Min(0.5f)] private float m_MaxFireDelay = 6f;
        [SerializeField, Range(0.1f, 2f)] private float m_FireY = 1.5f;
        [SerializeField, Range(1f, 89f)] private float m_FireAngle = 45f;
        [SerializeField, Min(2f)] private float m_FireDistance = 10f;
        [SerializeField, Min(1f)] private float m_PlayerHitForce = 5f;
        [SerializeField] private ShellView m_ShellPrefab;

        private Transform _target;
        private float nextFire;
        private Action<Vector3, float> OnPlayerHitCallback;

        public void Init(Transform target, Action<Vector3, float> OnPlayerHit)
        {
            _target = target;
            OnPlayerHitCallback = OnPlayerHit;
            nextFire = Time.time + Random.Range(m_MinFireDelay, m_MaxFireDelay);
        }

        private void Update()
        {
            if (Time.time >= nextFire)
            {
                Vector3 rayDir = (_target.position - transform.position).normalized;
                Vector3 rayOrigin = new Vector3(transform.position.x + rayDir.x * 0.5f, m_FireY, transform.position.z + rayDir.z * 0.5f);
                Ray ray = new Ray(rayOrigin, rayDir);
                if (Physics.Raycast(ray, out var hitInfo, m_FireDistance) && hitInfo.transform == _target)
                {
                    float rad = m_FireAngle * Mathf.Deg2Rad;
                    float sin = Mathf.Sin(rad);
                    float cos = Mathf.Cos(rad);
                    Vector3 dir = new Vector3(rayDir.x * cos, sin, rayDir.z * cos);
                    var shell = ObjectPool.Instantiate(m_ShellPrefab, rayOrigin, Quaternion.identity, null);
                    shell.Init(dir, ray, c => Hit(c, shell.PrevPosition));
                    nextFire = Time.time + Random.Range(m_MinFireDelay, m_MaxFireDelay);
                }
            }
        }

        private bool Hit(Collider c, Vector3 position)
        {
            if (c.gameObject == this || c.transform.IsChildOf(transform))
                return false;
            if (c.transform == _target)
                OnPlayerHitCallback?.Invoke(position, m_PlayerHitForce);
            return true;
        }
    }
}
