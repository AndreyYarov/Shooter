using UnityEngine;
using UnityEngine.UI;

namespace Shooter.UI
{
    class Aim : MonoBehaviour
    {
        [SerializeField] private Image m_Aim;
        [SerializeField] private Color m_InactiveColor;
        [SerializeField] private Color m_ActiveColor;

        private bool _active = false;
        public bool Active
        {
            get => _active;
            set
            {
                if (_active != value)
                {
                    _active = value;
                    m_Aim.color = _active ? m_ActiveColor : m_InactiveColor;
                }
            }
        }

        private void Start()
        {
            m_Aim.color = _active ? m_ActiveColor : m_InactiveColor;
        }
    }
}
