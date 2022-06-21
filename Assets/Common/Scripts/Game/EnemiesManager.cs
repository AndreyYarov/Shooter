using System;
using UnityEngine;
using Shooter.Agent;
using Shooter.Utils.Pool;

namespace Shooter.Game
{
    [Serializable]
    class EnemiesManager : MonoBehaviour
    {
        [SerializeField] private AgentAttack m_AgentPrefab;
        [SerializeField] private Vector3[] m_EnemyPositions = new Vector3[0];

        private GameObject[] enemies;

        public bool HaveActiveAgents()
        {
            foreach (var go in enemies)
                if (go.activeInHierarchy)
                    return true;
            return false;
        }
        
        public void PlaceAgents(GameObject player, Action<Vector3, float> OnPlayerHitCallback)
        {
            if (enemies != null)
                foreach (var go in enemies)
                    if (go.activeInHierarchy)
                        ObjectPool.Deactivate(go);

            enemies = new GameObject[m_EnemyPositions.Length];
            for (int i = 0; i < enemies.Length; i++)
            {
                enemies[i] = ObjectPool.Instantiate(m_AgentPrefab.gameObject, m_EnemyPositions[i], Quaternion.identity, transform);
                enemies[i].GetComponent<AgentAttack>().Init(player.transform, OnPlayerHitCallback);
            }    
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            foreach (var pos in m_EnemyPositions)
                Gizmos.DrawSphere(pos, 0.25f);
        }
#endif
    }
}
