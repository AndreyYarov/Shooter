using UnityEngine;

namespace Shooter.Utils {
    public class ParticleHideOnStop : MonoBehaviour
    {
        [SerializeField] private ParticleSystem m_Particle;

        private void OnEnable()
        {
            m_Particle.time = 0f;
            m_Particle.Play();
        }

        private void OnParticleSystemStopped()
        {
            Pool.ObjectPool.Deactivate(m_Particle);
        }
    }
}
