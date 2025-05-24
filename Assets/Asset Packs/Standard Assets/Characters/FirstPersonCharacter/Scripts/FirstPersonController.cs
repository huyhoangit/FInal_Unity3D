
using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput; 
using UnityStandardAssets.Utility; 
using Random = UnityEngine.Random;

#pragma warning disable 618, 649 

namespace UnityStandardAssets.Characters.FirstPerson
{
    // Bắt buộc phải có 2 thành phần: CharacterController và AudioSource
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(AudioSource))]
    public class FirstPersonController : MonoBehaviour
    {
        // Các thiết lập di chuyển và âm thanh
        [SerializeField] private bool m_IsWalking; // Đang đi bộ hay không
        [SerializeField] private float m_WalkSpeed;
        [SerializeField] private float m_RunSpeed;
        [SerializeField][Range(0f, 1f)] private float m_RunstepLenghten;
        [SerializeField] private float m_JumpSpeed;
        [SerializeField] private float m_StickToGroundForce; // Lực giữ nhân vật dính xuống đất
        [SerializeField] private float m_GravityMultiplier;
        [SerializeField] private MouseLook m_MouseLook;
        [SerializeField] private bool m_UseFovKick; // Dùng hiệu ứng zoom ra khi chạy
        [SerializeField] private FOVKick m_FovKick = new FOVKick();
        [SerializeField] private bool m_UseHeadBob; // Dùng hiệu ứng lắc đầu khi đi
        [SerializeField] private CurveControlledBob m_HeadBob = new CurveControlledBob();
        [SerializeField] private LerpControlledBob m_JumpBob = new LerpControlledBob();
        [SerializeField] private float m_StepInterval;
        [SerializeField] private AudioClip[] m_FootstepSounds; // Âm thanh bước chân
        [SerializeField] private AudioClip m_JumpSound;
        [SerializeField] private AudioClip m_LandSound;

        // Biến xử lý nội bộ
        private Camera m_Camera;
        private bool m_Jump;
        private float m_YRotation;
        private Vector2 m_Input;
        private Vector3 m_MoveDir = Vector3.zero;
        private CharacterController m_CharacterController;
        private CollisionFlags m_CollisionFlags;
        private bool m_PreviouslyGrounded;
        private Vector3 m_OriginalCameraPosition;
        private float m_StepCycle;
        private float m_NextStep;
        private bool m_Jumping;
        private AudioSource m_AudioSource;

        // Hàm khởi tạo
        private void Start()
        {
            m_CharacterController = GetComponent<CharacterController>();
            m_Camera = Camera.main;
            m_OriginalCameraPosition = m_Camera.transform.localPosition;

            // Setup các hiệu ứng chuyển động
            m_FovKick.Setup(m_Camera);
            m_HeadBob.Setup(m_Camera, m_StepInterval);

            m_StepCycle = 0f;
            m_NextStep = m_StepCycle / 2f;
            m_Jumping = false;
            m_AudioSource = GetComponent<AudioSource>();

            // Khởi tạo điều khiển chuột
            m_MouseLook.Init(transform, m_Camera.transform);
        }

        // Hàm Update chạy mỗi frame
        private void Update()
        {
            RotateView(); // Điều khiển góc nhìn bằng chuột

            // Nếu nhấn phím nhảy, gán m_Jump = true
            if (!m_Jump)
            {
                m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
            }

            // Nếu vừa chạm đất thì reset lại trạng thái nhảy
            if (!m_PreviouslyGrounded && m_CharacterController.isGrounded)
            {
                StartCoroutine(m_JumpBob.DoBobCycle());
                PlayLandingSound(); // Phát âm thanh tiếp đất
                m_MoveDir.y = 0f;
                m_Jumping = false;
            }

            // Nếu vừa rơi xuống (vừa mất tiếp đất)
            if (!m_CharacterController.isGrounded && !m_Jumping && m_PreviouslyGrounded)
            {
                m_MoveDir.y = 0f;
            }

            m_PreviouslyGrounded = m_CharacterController.isGrounded;
        }

        // Phát âm thanh tiếp đất
        private void PlayLandingSound()
        {
            m_AudioSource.clip = m_LandSound;
            m_AudioSource.Play();
            m_NextStep = m_StepCycle + .5f;
        }

        // FixedUpdate dùng cho xử lý vật lý
        private void FixedUpdate()
        {
            float speed;
            GetInput(out speed); // Đọc input từ người chơi

            // Tính hướng di chuyển theo hướng camera đang nhìn
            Vector3 desiredMove = transform.forward * m_Input.y + transform.right * m_Input.x;

            // Tính mặt phẳng bề mặt dưới chân (cho đi mượt)
            RaycastHit hitInfo;
            Physics.SphereCast(transform.position, m_CharacterController.radius, Vector3.down, out hitInfo,
                               m_CharacterController.height / 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
            desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

            // Gán hướng di chuyển XZ
            m_MoveDir.x = desiredMove.x * speed;
            m_MoveDir.z = desiredMove.z * speed;

            if (m_CharacterController.isGrounded)
            {
                m_MoveDir.y = -m_StickToGroundForce;

                // Nếu nhảy, áp lực nhảy lên
                if (m_Jump)
                {
                    m_MoveDir.y = m_JumpSpeed;
                    PlayJumpSound();
                    m_Jump = false;
                    m_Jumping = true;
                }
            }
            else
            {
                // Nếu đang trên không thì áp lực trọng lực nhiều hơn
                m_MoveDir += Physics.gravity * m_GravityMultiplier * Time.fixedDeltaTime;
            }

            // Di chuyển nhân vật
            m_CollisionFlags = m_CharacterController.Move(m_MoveDir * Time.fixedDeltaTime);

            // Tính chu kỳ bước chân và vị trí camera
            ProgressStepCycle(speed);
            UpdateCameraPosition(speed);

            // Khóa hoặc mở khóa con trỏ chuột
            m_MouseLook.UpdateCursorLock();
        }

        // Phát âm thanh nhảy
        private void PlayJumpSound()
        {
            m_AudioSource.clip = m_JumpSound;
            m_AudioSource.Play();
        }

        // Tính toán bước chân để phát âm thanh
        private void ProgressStepCycle(float speed)
        {
            if (m_CharacterController.velocity.sqrMagnitude > 0 && (m_Input.x != 0 || m_Input.y != 0))
            {
                m_StepCycle += (m_CharacterController.velocity.magnitude + (speed * (m_IsWalking ? 1f : m_RunstepLenghten))) *
                               Time.fixedDeltaTime;
            }

            if (!(m_StepCycle > m_NextStep))
            {
                return;
            }

            m_NextStep = m_StepCycle + m_StepInterval;

            PlayFootStepAudio();
        }

        // Phát âm thanh bước chân ngẫu nhiên
        private void PlayFootStepAudio()
        {
            if (!m_CharacterController.isGrounded)
            {
                return;
            }

            int n = Random.Range(1, m_FootstepSounds.Length); // chọn ngẫu nhiên âm thanh (trừ vị trí 0)
            m_AudioSource.clip = m_FootstepSounds[n];
            m_AudioSource.PlayOneShot(m_AudioSource.clip);

            // Đổi âm thanh vừa phát về vị trí đầu để lần sau không phát lại
            m_FootstepSounds[n] = m_FootstepSounds[0];
            m_FootstepSounds[0] = m_AudioSource.clip;
        }

        // Điều chỉnh vị trí camera cho hiệu ứng lắc đầu, nhảy
        private void UpdateCameraPosition(float speed)
        {
            Vector3 newCameraPosition;
            if (!m_UseHeadBob)
            {
                return;
            }

            if (m_CharacterController.velocity.magnitude > 0 && m_CharacterController.isGrounded)
            {
                m_Camera.transform.localPosition =
                    m_HeadBob.DoHeadBob(m_CharacterController.velocity.magnitude +
                                       (speed * (m_IsWalking ? 1f : m_RunstepLenghten)));
                newCameraPosition = m_Camera.transform.localPosition;
                newCameraPosition.y = m_Camera.transform.localPosition.y - m_JumpBob.Offset();
            }
            else
            {
                newCameraPosition = m_Camera.transform.localPosition;
                newCameraPosition.y = m_OriginalCameraPosition.y - m_JumpBob.Offset();
            }
            m_Camera.transform.localPosition = newCameraPosition;
        }

        // Lấy input từ người chơi
        private void GetInput(out float speed)
        {
            float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
            float vertical = CrossPlatformInputManager.GetAxis("Vertical");

            bool waswalking = m_IsWalking;

#if !MOBILE_INPUT
            // Nếu không phải thiết bị di động thì nhấn shift để chạy
            m_IsWalking = !Input.GetKey(KeyCode.LeftShift);
#endif
            speed = m_IsWalking ? m_WalkSpeed : m_RunSpeed;
            m_Input = new Vector2(horizontal, vertical);

            // Chuẩn hóa input
            if (m_Input.sqrMagnitude > 1)
            {
                m_Input.Normalize();
            }

            // Hiệu ứng thay đổi góc nhìn khi chuyển từ đi sang chạy
            if (m_IsWalking != waswalking && m_UseFovKick && m_CharacterController.velocity.sqrMagnitude > 0)
            {
                StopAllCoroutines();
                StartCoroutine(!m_IsWalking ? m_FovKick.FOVKickUp() : m_FovKick.FOVKickDown());
            }
        }

        // Điều khiển góc nhìn bằng chuột
        private void RotateView()
        {
            m_MouseLook.LookRotation(transform, m_Camera.transform);
        }

        // Tác động lên vật thể có Rigidbody khi va chạm
        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            Rigidbody body = hit.collider.attachedRigidbody;

            if (m_CollisionFlags == CollisionFlags.Below) return;
            if (body == null || body.isKinematic) return;

            body.AddForceAtPosition(m_CharacterController.velocity * 0.1f, hit.point, ForceMode.Impulse);
        }
    }
}
