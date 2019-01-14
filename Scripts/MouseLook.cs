using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.EventSystems;

namespace UnityStandardAssets.Characters.FirstPerson
{
    [Serializable]
    public class MouseLook
    {
        public float XSensitivity = 2f;
        public float YSensitivity = 2f;
        public bool clampVerticalRotation = true;
        public float MinimumX = -90F;
        public float MaximumX = 90F;
        public bool smooth;
        public float smoothTime = 5f;
        public bool lockCursor = true;
        public bool allowLooking = true;

        private Quaternion m_CharacterTargetRot;
        private Quaternion m_CameraTargetRot;

        public bool m_cursorIsLocked = true;
        
        public void Init(Transform character, Transform camera)
        {
            m_CharacterTargetRot = character.localRotation;
            m_CameraTargetRot = camera.localRotation;
        }

        public void SetRotation(Transform character, Transform camera, Transform targetTran)
        {

            m_CharacterTargetRot = Quaternion.Euler(0f, targetTran.rotation.eulerAngles.y, 0f);
            m_CameraTargetRot = Quaternion.Euler(targetTran.rotation.eulerAngles.x, 0f, 0f);

            character.localRotation = m_CharacterTargetRot;
            camera.localRotation = m_CameraTargetRot;
        }


        public void LookRotation(Transform character, Transform camera)
        {
            UpdateCursorLock();

            if (allowLooking && m_cursorIsLocked)
            {
                float yRot = CrossPlatformInputManager.GetAxis("Mouse X") * XSensitivity;
                float xRot = CrossPlatformInputManager.GetAxis("Mouse Y") * YSensitivity;

                m_CharacterTargetRot *= Quaternion.Euler(0f, yRot, 0f);
                m_CameraTargetRot *= Quaternion.Euler(-xRot, 0f, 0f);

                if (clampVerticalRotation)
                    m_CameraTargetRot = ClampRotationAroundXAxis(m_CameraTargetRot);

                if (smooth)
                {
                    character.localRotation = Quaternion.Slerp(character.localRotation, m_CharacterTargetRot,
                        smoothTime * Time.deltaTime);
                    camera.localRotation = Quaternion.Slerp(camera.localRotation, m_CameraTargetRot,
                        smoothTime * Time.deltaTime);
                }
                else
                {
                    character.localRotation = m_CharacterTargetRot;
                    camera.localRotation = m_CameraTargetRot;
                }
            }
            else
            {
                //Look around with Left Mouse
                if (Input.GetMouseButton(0))
                {
                    allowLooking = true;
                    float yRot = CrossPlatformInputManager.GetAxis("Mouse X") * XSensitivity * 2;
                    float xRot = CrossPlatformInputManager.GetAxis("Mouse Y") * YSensitivity * 2;

                    m_CharacterTargetRot *= Quaternion.Euler(0f, yRot, 0f);
                    m_CameraTargetRot *= Quaternion.Euler(-xRot, 0f, 0f);

                    character.localRotation = m_CharacterTargetRot;
                    camera.localRotation = m_CameraTargetRot;

                }
                else
                {
                    allowLooking = false;
                }
            }
        }

        public void SetCursorLock(bool value)
        {
            lockCursor = value;
            if(!lockCursor)
            {//we force unlock the cursor if the user disable the cursor locking helper
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        public void UpdateCursorLock()
        {
            //if the user set "lockCursor" we check & properly lock the cursos
            if (lockCursor)
                InternalLockUpdate();
        }

        private void InternalLockUpdate()
        {
            // Dont allow cursor locking at this time
            if(false)
            {
                m_cursorIsLocked = !m_cursorIsLocked;
                allowLooking = !allowLooking;
            }

            if (m_cursorIsLocked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else if (!m_cursorIsLocked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        Quaternion ClampRotationAroundXAxis(Quaternion q)
        {
            q.x /= q.w;
            q.y /= q.w;
            q.z /= q.w;
            q.w = 1.0f;

            float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan (q.x);

            angleX = Mathf.Clamp (angleX, MinimumX, MaximumX);

            q.x = Mathf.Tan (0.5f * Mathf.Deg2Rad * angleX);

            return q;
        }

    }
}
