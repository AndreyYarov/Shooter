using System;
using UnityEngine;
using Shooter.Utils.Pool;

namespace Shooter.Shell
{
    public class ShellView : MonoBehaviour
    {
        private const float GroundDetectionDistance = 0.01f;

        [SerializeField] private Rigidbody m_Rigidbody;
        [SerializeField] private float m_DestroyY = -10f;
        [SerializeField] private float m_AimDistance = 10f;

        [Header("Visual effects")]
        [SerializeField] private ParticleSystem m_ExplosionOnGround;
        [SerializeField] private ParticleSystem m_ExplosionUpperGround;

        private Func<Collider, bool> OnHitCallback;

        public void Init(Vector3 dir, Ray targetRay, Func<Collider, bool> OnHit)
        {
            float p = Mathf.Sqrt(dir.x * dir.x + dir.z * dir.z);
            if (p == 0)
            {
                float v0 = Mathf.Sqrt(-2f * Physics.gravity.y * m_AimDistance);
                m_Rigidbody.velocity = dir * v0;
            }
            else
            {
                float vt = 10 / p;
                Vector3 dst = targetRay.origin + targetRay.direction * vt;
                float t = Mathf.Sqrt((transform.position.y - dst.y + dir.y * vt) * (-2f / Physics.gravity.y));
                float v0 = vt / t;
                m_Rigidbody.velocity = dir * v0;
            }

            OnHitCallback = OnHit;
        }

        public Vector3 PrevPosition => transform.position - m_Rigidbody.velocity * Time.fixedDeltaTime;

        private void Update()
        {
            if (transform.position.y <= m_DestroyY)
            {
                ObjectPool.Instantiate(m_ExplosionUpperGround, transform.position, Quaternion.identity, null);
                ObjectPool.Deactivate(this);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (OnHitCallback == null || OnHitCallback(other))
            {
                OnHitCallback = null;
                if (other.bounds.max.y <= GroundDetectionDistance)
                    ObjectPool.Instantiate(m_ExplosionOnGround, new Vector3(transform.position.x, 0f, transform.position.z), Quaternion.identity, null);
                else
                    ObjectPool.Instantiate(m_ExplosionUpperGround, PrevPosition, Quaternion.identity, null);
                ObjectPool.Deactivate(this);
            }
        }
    }
}
