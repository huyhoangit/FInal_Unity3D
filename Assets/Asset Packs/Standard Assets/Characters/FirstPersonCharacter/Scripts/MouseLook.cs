using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Characters.FirstPerson
{
    [Serializable]
    public class MouseLook
    {
        public float XSensitivity = 2f; // Do nhay khi xoay ngang (mouse X)
        public float YSensitivity = 2f; // Do nhay khi xoay doc (mouse Y)
        public bool clampVerticalRotation = true; // Gioi han goc xoay doc (len/xuong)
        public float MinimumX = -90F; // Goc nho nhat camera co the xoay xuong
        public float MaximumX = 90F; // Goc lon nhat camera co the xoay len
        public bool smooth; // Kich hoat xoay muot
        public float smoothTime = 5f; // Toc do xoay muot
        public bool lockCursor = true; // Khoa con tro chuot trong khi choi

        private Quaternion m_CharacterTargetRot; // Huong xoay dich cua nhan vat
        private Quaternion m_CameraTargetRot; // Huong xoay dich cua camera
        private bool m_cursorIsLocked = true; // Trang thai khoa con tro

        public void Init(Transform character, Transform camera)
        {
            // Luu huong xoay hien tai cua nhan vat va camera
            m_CharacterTargetRot = character.localRotation;
            m_CameraTargetRot = camera.localRotation;
        }

        public void LookRotation(Transform character, Transform camera)
        {
            // Lay input tu chuot (chu y: dung CrossPlatformInput de tuong thich tren mobile)
            float yRot = CrossPlatformInputManager.GetAxis("Mouse X") * XSensitivity;
            float xRot = CrossPlatformInputManager.GetAxis("Mouse Y") * YSensitivity;

            // Xoay nhan vat quanh truc Y (trai/phai)
            m_CharacterTargetRot *= Quaternion.Euler(0f, yRot, 0f);

            // Xoay camera quanh truc X (len/xuong)
            m_CameraTargetRot *= Quaternion.Euler(-xRot, 0f, 0f);

            // Neu bat clamp, gioi han goc xoay doc
            if (clampVerticalRotation)
                m_CameraTargetRot = ClampRotationAroundXAxis(m_CameraTargetRot);

            // Ap dung xoay muot hoac truc tiep
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

            // Cap nhat trang thai khoa chuot
            UpdateCursorLock();
        }

        public void SetCursorLock(bool value)
        {
            // Cho phep bat/tat viec khoa chuot
            lockCursor = value;
            if (!lockCursor)
            {
                // Neu khong khoa chuot thi hien con tro
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        public void UpdateCursorLock()
        {
            // Neu nguoi dung muon khoa chuot thi goi InternalLockUpdate
            if (lockCursor)
                InternalLockUpdate();
        }

        private void InternalLockUpdate()
        {
            // Nhan ESC de mo khoa chuot
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                m_cursorIsLocked = false;
            }
            // Click chuot trai de khoa chuot lai
            else if (Input.GetMouseButtonUp(0))
            {
                m_cursorIsLocked = true;
            }

            // Thuc hien khoa hoac mo khoa chuot theo trang thai hien tai
            if (m_cursorIsLocked)
            {
                Cursor.lockState = CursorLockMode.Locked; // Khoa chuot giu o giua man hinh
                Cursor.visible = false; // An con tro chuot
            }
            else
            {
                Cursor.lockState = CursorLockMode.None; // Cho chuot di tu do
                Cursor.visible = true; // Hien con tro chuot
            }
        }

        Quaternion ClampRotationAroundXAxis(Quaternion q)
        {
            // Bien doi quaternion thanh dang co the tinh toan goc X
            q.x /= q.w;
            q.y /= q.w;
            q.z /= q.w;
            q.w = 1.0f;

            // Chuyen quaternion thanh goc X
            float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

            // Gioi han goc X trong khoang cho phep
            angleX = Mathf.Clamp(angleX, MinimumX, MaximumX);

            // Bien doi nguoc lai thanh quaternion
            q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

            return q;
        }
    }
}
