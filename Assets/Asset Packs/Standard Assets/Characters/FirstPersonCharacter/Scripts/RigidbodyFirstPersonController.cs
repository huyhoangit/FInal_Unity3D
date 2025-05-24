using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Characters.FirstPerson
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class RigidbodyFirstPersonController : MonoBehaviour
    {
        [Serializable]
        public class MovementSettings
        {
            // Các thông số tốc độ di chuyển
            public float ForwardSpeed = 8.0f;   // Tốc độ đi tới
            public float BackwardSpeed = 4.0f;  // Tốc độ lùi
            public float StrafeSpeed = 4.0f;    // Tốc độ đi ngang
            public float RunMultiplier = 2.0f;   // Hệ số tăng tốc khi chạy
            public KeyCode RunKey = KeyCode.LeftShift; // Phím chạy
            public float JumpForce = 50f; // Lực nhảy
            // Đường cong điều chỉnh tốc độ khi leo dốc
            public AnimationCurve SlopeCurveModifier = new AnimationCurve(new Keyframe(-90.0f, 1.0f), new Keyframe(0.0f, 1.0f), new Keyframe(90.0f, 0.0f));
            [HideInInspector] public float CurrentTargetSpeed = 8f; // Tốc độ mục tiêu hiện tại

#if !MOBILE_INPUT
            private bool m_Running; // Trạng thái đang chạy
#endif

            // Cập nhật tốc độ mục tiêu dựa trên input
            public void UpdateDesiredTargetSpeed(Vector2 input)
            {
                if (input == Vector2.zero) return;

                // Xử lý tốc độ khi đi ngang
                if (input.x > 0 || input.x < 0)
                {
                    CurrentTargetSpeed = StrafeSpeed;
                }

                // Xử lý tốc độ khi lùi
                if (input.y < 0)
                {
                    CurrentTargetSpeed = BackwardSpeed;
                }

                // Xử lý tốc độ khi tiến (ưu tiên cao nhất)
                if (input.y > 0)
                {
                    CurrentTargetSpeed = ForwardSpeed;
                }

#if !MOBILE_INPUT
                // Xử lý tăng tốc khi chạy
                if (Input.GetKey(RunKey))
                {
                    CurrentTargetSpeed *= RunMultiplier;
                    m_Running = true;
                }
                else
                {
                    m_Running = false;
                }
#endif
            }

#if !MOBILE_INPUT
            public bool Running
            {
                get { return m_Running; }
            }
#endif
        }

        [Serializable]
        public class AdvancedSettings
        {
            // Các thiết lập nâng cao
            public float groundCheckDistance = 0.01f; // Khoảng cách kiểm tra mặt đất
            public float stickToGroundHelperDistance = 0.5f; // Khoảng cách bám đất
            public float slowDownRate = 20f; // Tốc độ giảm tốc khi không có input
            public bool airControl; // Cho phép điều khiển khi ở trên không
            [Tooltip("set it to 0.1 or more if you get stuck in wall")]
            public float shellOffset; // Độ lệch để tránh kẹt vào tường
        }

        // Các thành phần cần thiết
        public Camera cam; // Camera góc nhìn thứ nhất
        public MovementSettings movementSettings = new MovementSettings(); // Cài đặt di chuyển
        public MouseLook mouseLook = new MouseLook(); // Điều khiển chuột
        public AdvancedSettings advancedSettings = new AdvancedSettings(); // Cài đặt nâng cao

        // Các biến nội bộ
        private Rigidbody m_RigidBody; // Rigidbody của nhân vật
        private CapsuleCollider m_Capsule; // Collider dạng viên nang
        private float m_YRotation; // Góc xoay theo trục Y
        private Vector3 m_GroundContactNormal; // Vector pháp tuyến mặt đất
        private bool m_Jump, m_PreviouslyGrounded, m_Jumping, m_IsGrounded; // Các trạng thái nhảy/đứng

        // Các thuộc tính công khai
        public Vector3 Velocity
        {
            get { return m_RigidBody.linearVelocity; } // Vận tốc hiện tại
        }

        public bool Grounded
        {
            get { return m_IsGrounded; } // Có đang đứng trên mặt đất
        }

        public bool Jumping
        {
            get { return m_Jumping; } // Có đang nhảy
        }

        public bool Running
        {
            get
            {
#if !MOBILE_INPUT
                return movementSettings.Running; // Có đang chạy
#else
                return false;
#endif
            }
        }

        private void Start()
        {
            // Khởi tạo các thành phần
            m_RigidBody = GetComponent<Rigidbody>();
            m_Capsule = GetComponent<CapsuleCollider>();
            mouseLook.Init(transform, cam.transform); // Khởi tạo điều khiển chuột
        }

        private void Update()
        {
            // Xử lý xoay camera mỗi frame
            RotateView();

            // Xử lý input nhảy
            if (CrossPlatformInputManager.GetButtonDown("Jump") && !m_Jump)
            {
                m_Jump = true;
            }
        }

        private void FixedUpdate()
        {
            // Kiểm tra mặt đất và xử lý vật lý
            GroundCheck();
            Vector2 input = GetInput(); // Lấy input từ người chơi

            // Xử lý di chuyển khi có input và (đang đứng hoặc cho phép điều khiển trên không)
            if ((Mathf.Abs(input.x) > float.Epsilon || Mathf.Abs(input.y) > float.Epsilon) && (advancedSettings.airControl || m_IsGrounded))
            {
                // Tính hướng di chuyển dựa trên hướng camera
                Vector3 desiredMove = cam.transform.forward * input.y + cam.transform.right * input.x;
                desiredMove = Vector3.ProjectOnPlane(desiredMove, m_GroundContactNormal).normalized;

                // Áp dụng tốc độ
                desiredMove.x = desiredMove.x * movementSettings.CurrentTargetSpeed;
                desiredMove.z = desiredMove.z * movementSettings.CurrentTargetSpeed;
                desiredMove.y = desiredMove.y * movementSettings.CurrentTargetSpeed;

                // Thêm lực nếu vận tốc hiện tại nhỏ hơn tốc độ mục tiêu
                if (m_RigidBody.linearVelocity.sqrMagnitude <
                    (movementSettings.CurrentTargetSpeed * movementSettings.CurrentTargetSpeed))
                {
                    m_RigidBody.AddForce(desiredMove * SlopeMultiplier(), ForceMode.Impulse);
                }
            }

            // Xử lý khi đang đứng trên mặt đất
            if (m_IsGrounded)
            {
                m_RigidBody.linearDamping = 5f; // Tăng ma sát để giảm tốc nhanh

                // Xử lý nhảy
                if (m_Jump)
                {
                    m_RigidBody.linearDamping = 0f;
                    m_RigidBody.linearVelocity = new Vector3(m_RigidBody.linearVelocity.x, 0f, m_RigidBody.linearVelocity.z);
                    m_RigidBody.AddForce(new Vector3(0f, movementSettings.JumpForce, 0f), ForceMode.Impulse);
                    m_Jumping = true;
                }

                // Tối ưu: Cho rigidbody "ngủ" khi không di chuyển
                if (!m_Jumping && Mathf.Abs(input.x) < float.Epsilon && Mathf.Abs(input.y) < float.Epsilon && m_RigidBody.linearVelocity.magnitude < 1f)
                {
                    m_RigidBody.Sleep();
                }
            }
            else // Khi ở trên không
            {
                m_RigidBody.linearDamping = 0f; // Giảm ma sát

                // Bám đất khi vừa rời khỏi mặt đất
                if (m_PreviouslyGrounded && !m_Jumping)
                {
                    StickToGroundHelper();
                }
            }
            m_Jump = false; // Reset trạng thái nhảy
        }

        // Tính hệ số độ dốc
        private float SlopeMultiplier()
        {
            float angle = Vector3.Angle(m_GroundContactNormal, Vector3.up);
            return movementSettings.SlopeCurveModifier.Evaluate(angle);
        }

        // Hỗ trợ bám đất
        private void StickToGroundHelper()
        {
            RaycastHit hitInfo;
            if (Physics.SphereCast(transform.position, m_Capsule.radius * (1.0f - advancedSettings.shellOffset),
                Vector3.down, out hitInfo,
                ((m_Capsule.height / 2f) - m_Capsule.radius) + advancedSettings.stickToGroundHelperDistance,
                Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                // Chỉ bám đất nếu mặt đất không quá dốc
                if (Mathf.Abs(Vector3.Angle(hitInfo.normal, Vector3.up)) < 85f)
                {
                    m_RigidBody.linearVelocity = Vector3.ProjectOnPlane(m_RigidBody.linearVelocity, hitInfo.normal);
                }
            }
        }

        // Lấy input từ người chơi
        private Vector2 GetInput()
        {
            Vector2 input = new Vector2
            {
                x = CrossPlatformInputManager.GetAxis("Horizontal"), // Input ngang
                y = CrossPlatformInputManager.GetAxis("Vertical") // Input dọc
            };
            movementSettings.UpdateDesiredTargetSpeed(input); // Cập nhật tốc độ mục tiêu
            return input;
        }

        // Xoay camera và nhân vật
        private void RotateView()
        {
            // Bỏ qua nếu game đang tạm dừng
            if (Mathf.Abs(Time.timeScale) < float.Epsilon) return;

            float oldYRotation = transform.eulerAngles.y; // Lưu góc xoay cũ

            mouseLook.LookRotation(transform, cam.transform); // Xoay theo chuột

            // Xoay vận tốc để phù hợp với hướng nhìn mới
            if (m_IsGrounded || advancedSettings.airControl)
            {
                Quaternion velRotation = Quaternion.AngleAxis(transform.eulerAngles.y - oldYRotation, Vector3.up);
                m_RigidBody.linearVelocity = velRotation * m_RigidBody.linearVelocity;
            }
        }

        // Kiểm tra có đang đứng trên mặt đất không
        private void GroundCheck()
        {
            m_PreviouslyGrounded = m_IsGrounded;
            RaycastHit hitInfo;

            // Dùng SphereCast để kiểm tra va chạm với mặt đất
            if (Physics.SphereCast(transform.position, m_Capsule.radius * (1.0f - advancedSettings.shellOffset),
                Vector3.down, out hitInfo,
                ((m_Capsule.height / 2f) - m_Capsule.radius) + advancedSettings.groundCheckDistance,
                Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                m_IsGrounded = true;
                m_GroundContactNormal = hitInfo.normal; // Lưu pháp tuyến mặt đất
            }
            else
            {
                m_IsGrounded = false;
                m_GroundContactNormal = Vector3.up; // Mặc định là hướng lên
            }

            // Kết thúc nhảy khi chạm đất
            if (!m_PreviouslyGrounded && m_IsGrounded && m_Jumping)
            {
                m_Jumping = false;
            }
        }
    }
}