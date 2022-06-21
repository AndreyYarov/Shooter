using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

namespace Shooter.UI
{
    class SimpleView : MonoBehaviour
    {
        [SerializeField] private Button m_Button;
        [SerializeField] private TextMeshProUGUI m_Title;
        [SerializeField] private TextMeshProUGUI m_ButtonLabel;

        public void Init(string title, string buttonText, UnityAction OnButtonClick)
        {
            m_Title.text = title;
            m_ButtonLabel.text = buttonText;
            m_Button.onClick.RemoveAllListeners();
            m_Button.onClick.AddListener(OnButtonClick);
        }
    }
}
