using UnityEngine;
using Shooter.Shell;
using Shooter.Utils.Pool;

namespace Shooter.Player
{
    public class PlayerAttack : MonoBehaviour
    {
        [SerializeField] private UI.Aim m_Aim;
        [SerializeField] private Transform m_FireSource;
        [SerializeField] private Camera m_MainCamera;
        [SerializeField] private ShellView m_ShellPrefab;

        private void Update()
        {
            Ray ray = m_MainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
            if (Physics.Raycast(ray, out var hitInfo) && hitInfo.collider.CompareTag("NPC"))
                m_Aim.Active = true;
            else
                m_Aim.Active = false;
            if (Input.GetMouseButtonDown(0))
            {
                var shell = ObjectPool.Instantiate(m_ShellPrefab, m_FireSource.position, Quaternion.identity, null);
                shell.Init(m_FireSource.up, ray, Hit);
            }
        }

        private bool Hit(Collider c)
        {
            if (c.gameObject == this || c.transform.IsChildOf(transform))
                return false;
            if (c.CompareTag("NPC"))
                ObjectPool.Deactivate(c.gameObject);
            return true;
        }
    }
}
