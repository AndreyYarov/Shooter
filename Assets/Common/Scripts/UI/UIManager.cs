using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Shooter.UI
{
    public enum GameState
    {
        Playing,
        Finish,
        Fail
    }

    public class UIManager : MonoBehaviour
    {
        [SerializeField] private SimpleView m_SimpleViewPrefab;
        [SerializeField] private Aim m_Aim;

        private SimpleView _finishView;
        private SimpleView _failView;

        private void Awake()
        {
            _finishView = Instantiate(m_SimpleViewPrefab, transform);
            _finishView.name = "Finish-View";
            _finishView.gameObject.SetActive(false);
            _failView = Instantiate(m_SimpleViewPrefab, transform);
            _failView.name = "Fail-View";
            _failView.gameObject.SetActive(false);
        }

        public void Init(UnityAction OnRestartButtonClick)
        {
            _finishView.Init("Выиграл", "Начать с начала", OnRestartButtonClick);
            _failView.Init("Проиграл", "Начать с начала", OnRestartButtonClick);
        }

        public void SetGameState(GameState state)
        {
            _finishView.gameObject.SetActive(state == GameState.Finish);
            _failView.gameObject.SetActive(state == GameState.Fail);
            m_Aim.gameObject.SetActive(state == GameState.Playing);
        }

        public void SetAimState(bool active) => m_Aim.Active = active;
    }
}
