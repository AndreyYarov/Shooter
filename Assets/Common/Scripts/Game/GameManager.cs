using UnityEngine;
using Shooter.Player;
using Shooter.UI;

namespace Shooter.Game
{
    class GameManager : MonoBehaviour
    {
        [SerializeField] private EnemiesManager m_EnemiesManager;
        [SerializeField] private PlayerMovement m_Player;
        [SerializeField] private UIManager m_UI;

        private Vector3 _playerSpawnPosition;
        private PlayerAttack _playerAttack;

        private void Start()
        {
            _playerSpawnPosition = m_Player.transform.position;
            _playerAttack = m_Player.GetComponent<PlayerAttack>();
            _playerAttack.Init(m_UI.SetAimState);
            m_UI.Init(StartGame);
            StartGame();
        }

        public void StartGame()
        {
            m_UI.SetGameState(GameState.Playing);
            ActivatePlayer();
            m_Player.transform.SetPositionAndRotation(_playerSpawnPosition, Quaternion.identity);
            m_Player.Init(Finish, Fail);
            m_EnemiesManager.PlaceAgents(m_Player.gameObject, m_Player.HitPlayer);
        }

        private void Finish()
        {
            m_UI.SetGameState(GameState.Finish);
            DeactivatePlayer();
        }

        private void Fail()
        {
            m_UI.SetGameState(GameState.Fail);
            DeactivatePlayer();
        }

        private void DeactivatePlayer()
        {
            m_Player.enabled = false;
            _playerAttack.enabled = false;
        }

        private void ActivatePlayer()
        {
            m_Player.enabled = true;
            _playerAttack.enabled = true;
        }
    }
}
