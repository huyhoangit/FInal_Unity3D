using System;
using UnityEngine;
using UnityStandardAssets.Utility;

namespace UnityStandardAssets.Characters.FirstPerson
{
    public class HeadBob : MonoBehaviour
    {
        // Các thành phần cần thiết
        public Camera Camera; // Camera sẽ áp dụng hiệu ứng
        public CurveControlledBob motionBob = new CurveControlledBob(); // Xử lý lắc đầu khi di chuyển
        public LerpControlledBob jumpAndLandingBob = new LerpControlledBob(); // Xử lý lắc đầu khi nhảy/tiếp đất
        public RigidbodyFirstPersonController rigidbodyFirstPersonController; // Controller điều khiển nhân vật
        public float StrideInterval; // Khoảng thời gian giữa các bước chân
        [Range(0f, 1f)] public float RunningStrideLengthen; // Độ dài bước chân khi chạy (từ 0-1)

        private bool m_PreviouslyGrounded; // Trạng thái mặt đất ở frame trước
        private Vector3 m_OriginalCameraPosition; // Vị trí ban đầu của camera

        private void Start()
        {
            // Thiết lập ban đầu
            motionBob.Setup(Camera, StrideInterval); // Khởi tạo hiệu ứng di chuyển
            m_OriginalCameraPosition = Camera.transform.localPosition; // Lưu vị trí gốc của camera
        }

        private void Update()
        {
            Vector3 newCameraPosition;

            // Kiểm tra nếu nhân vật đang di chuyển và đứng trên mặt đất
            if (rigidbodyFirstPersonController.Velocity.magnitude > 0 && rigidbodyFirstPersonController.Grounded)
            {
                // Áp dụng hiệu ứng lắc đầu theo tốc độ di chuyển
                float speedFactor = rigidbodyFirstPersonController.Velocity.magnitude *
                                  (rigidbodyFirstPersonController.Running ? RunningStrideLengthen : 1f);

                Camera.transform.localPosition = motionBob.DoHeadBob(speedFactor);
                newCameraPosition = Camera.transform.localPosition;

                // Điều chỉnh vị trí Y nếu có hiệu ứng nhảy/tiếp đất
                newCameraPosition.y = Camera.transform.localPosition.y - jumpAndLandingBob.Offset();
            }
            else
            {
                // Giữ nguyên vị trí camera nếu không di chuyển
                newCameraPosition = Camera.transform.localPosition;
                newCameraPosition.y = m_OriginalCameraPosition.y - jumpAndLandingBob.Offset();
            }

            Camera.transform.localPosition = newCameraPosition;

            // Xử lý hiệu ứng khi tiếp đất
            if (!m_PreviouslyGrounded && rigidbodyFirstPersonController.Grounded)
            {
                StartCoroutine(jumpAndLandingBob.DoBobCycle());
            }

            // Cập nhật trạng thái mặt đất
            m_PreviouslyGrounded = rigidbodyFirstPersonController.Grounded;
        }
    }
}