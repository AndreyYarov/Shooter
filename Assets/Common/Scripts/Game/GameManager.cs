using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shooter.Player;

namespace Shooter.Game
{
    class GameManager : MonoBehaviour
    {
        [SerializeField] private EnemiesManager m_EnemiesManager;
        [SerializeField] private PlayerMovement m_Player;

        private void Start()
        {
            m_EnemiesManager.PlaceAgents(m_Player.gameObject, m_Player.HitPlayer);
        }
    }
}
