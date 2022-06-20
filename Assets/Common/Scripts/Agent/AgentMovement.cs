using UnityEngine;
using UnityEngine.AI;

namespace Shooter.Agent
{
    public class AgentMovement : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float m_Speed;
        [SerializeField] private NavMeshAgent m_Agent;

        private Vector3 right;
        private Vector3 left;

        void OnEnable()
        {
            if (m_Speed > 0f && m_Agent && NavMesh.SamplePosition(transform.position, out var hit, 1f, NavMesh.AllAreas))
            {
                Vector3 samplePos = hit.position;
                transform.position = samplePos;

                right = samplePos + Vector3.right * 100f;
                if (NavMesh.Raycast(samplePos, right, out hit, NavMesh.AllAreas))
                    right = hit.position;

                left = samplePos + Vector3.left * 100f;
                if (NavMesh.Raycast(samplePos, left, out hit, NavMesh.AllAreas))
                    left = hit.position;

                m_Agent.speed = m_Speed;
                m_Agent.SetDestination(Random.Range(0f, 1f) < 0.5f ? left : right);
            }
        }

        private void Update()
        {
            if (m_Agent.remainingDistance <= m_Agent.stoppingDistance)
                m_Agent.SetDestination(m_Agent.destination == left ? right : left);
        }
    }
}
