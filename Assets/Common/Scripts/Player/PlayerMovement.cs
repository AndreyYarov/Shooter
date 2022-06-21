using System;
using UnityEngine;

namespace Shooter.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private float m_Speed = 5f;
        [SerializeField] private float m_RotationSpeed = 180f;
        [SerializeField] private Transform m_CameraContainer;
        [SerializeField] private Rigidbody m_Rigidbody;
        [SerializeField] private float m_FailY = -1f;

        private float cameraAngle = 0f;

        private Action OnFailCallback;
        private Action OnFinishCallback;

        private void OnEnable()
        {
            m_Rigidbody.isKinematic = false;
        }

        private void OnDisable()
        {
            m_Rigidbody.isKinematic = true;
        }

        private void FixedUpdate()
        {
            Vector3 movement = transform.forward * Input.GetAxis("Vertical") + transform.right * Input.GetAxis("Horizontal");
            if (movement != Vector3.zero)
                transform.position += movement.normalized * m_Speed * Time.fixedDeltaTime;
            
            if (transform.position.y <= m_FailY)
            {
                OnFailCallback?.Invoke();
                OnFailCallback = null;
                OnFinishCallback = null;
            }
        }

        private void Update()
        {
            float rotationY = Input.GetAxis("Mouse X");
            if (rotationY != 0f)
                transform.Rotate(Vector3.up, rotationY * m_RotationSpeed * Time.deltaTime);

            float newCameraAngle = Mathf.Clamp(cameraAngle - Input.GetAxis("Mouse Y") * m_RotationSpeed * Time.deltaTime, -90f, 90f);
            if (newCameraAngle != cameraAngle)
            {
                cameraAngle = newCameraAngle;
                m_CameraContainer.localRotation = Quaternion.Euler(cameraAngle, 0f, 0f);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Finish"))
            {
                OnFinishCallback?.Invoke();
                OnFailCallback = null;
                OnFinishCallback = null;
            }
        }

        public void HitPlayer(Vector3 position, float force)
        {
            Vector3 dir = transform.position - position;
            dir.y = 0f;
            m_Rigidbody.AddForce(dir.normalized * force, ForceMode.Impulse);
        }

        public void Init(Action OnFinish, Action OnFail)
        {
            OnFinishCallback = OnFinish;
            OnFailCallback = OnFail;
        }
    }
}
