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

        [Header("Visual effects")]
        [SerializeField] private ParticleSystem m_ExplosionOnGround;
        [SerializeField] private ParticleSystem m_ExplosionUpperGround;

        private Action<Collider> OnHitCallback;

        public void Init(Vector3 velocity, Action<Collider> OnHit)
        {
            m_Rigidbody.velocity = velocity;
            OnHitCallback = OnHit;
        }

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
            OnHitCallback?.Invoke(other);
            OnHitCallback = null;
            if (other.bounds.max.y <= GroundDetectionDistance)
                ObjectPool.Instantiate(m_ExplosionOnGround, new Vector3(transform.position.x, 0f, transform.position.z), Quaternion.identity, null);
            else
                ObjectPool.Instantiate(m_ExplosionUpperGround, transform.position, Quaternion.identity, null);
            ObjectPool.Deactivate(this);
        }
    }
}
