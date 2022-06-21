using System;
using UnityEngine;
using Shooter.Shell;
using Shooter.Utils.Pool;

namespace Shooter.Player
{
    public class PlayerAttack : MonoBehaviour
    {
        [SerializeField] private Transform m_FireSource;
        [SerializeField] private Camera m_MainCamera;
        [SerializeField] private ShellView m_ShellPrefab;

        private Action<bool> SetAimStateCallback;

        public void Init(Action<bool> SetAimState)
        {
            SetAimStateCallback = SetAimState;
        }

        private void Update()
        {
            Ray ray = m_MainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
            if (Physics.Raycast(ray, out var hitInfo) && hitInfo.collider.CompareTag("NPC"))
                SetAimStateCallback?.Invoke(true);
            else
                SetAimStateCallback?.Invoke(false);
            if (Input.GetMouseButtonDown(0))
            {
                var shell = ObjectPool.Instantiate(m_ShellPrefab, m_FireSource.position, Quaternion.identity, null);
                shell.Init(m_FireSource.up, ray, Hit);

                UnityEngine.Debug.Log("Выстрел");
            }
        }

        private bool Hit(Collider c)
        {
            if (c.gameObject == this || c.transform.IsChildOf(transform))
                return false;
            if (c.CompareTag("NPC"))
            {
                ObjectPool.Deactivate(c.gameObject);

                UnityEngine.Debug.Log("Попадание в агента");
            }
            return true;
        }
    }
}
