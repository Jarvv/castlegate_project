using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;

namespace UnityStandardAssets.Characters.FirstPerson
{
    [RequireComponent(typeof (CharacterController))]
    public class FirstPersonController : MonoBehaviour
    {
        [SerializeField] private bool m_IsWalking;
        [SerializeField] private float m_WalkSpeed;
        [SerializeField] private float m_RunSpeed;
        [SerializeField] [Range(0f, 1f)] private float m_RunstepLenghten;
        [SerializeField] private float m_StickToGroundForce;
        [SerializeField] private float m_GravityMultiplier;
        [SerializeField] private MouseLook m_MouseLook;
        [SerializeField] private bool m_UseFovKick;
        [SerializeField] private FOVKick m_FovKick = new FOVKick();
        [SerializeField] private bool m_UseHeadBob;
        [SerializeField] private CurveControlledBob m_HeadBob = new CurveControlledBob();
        [SerializeField] private LerpControlledBob m_JumpBob = new LerpControlledBob();
        [SerializeField] private float m_StepInterval;

        public MobileMovement mobMovRight;
        public MobileMovement mobMovLeft;
        public BuildControl buildControl;
        public InterfaceController interfaceCont;
        public MouseControl mouseCont;
        
        private Camera m_Camera;
        private bool m_Jump;
        private float m_YRotation;
        private Vector2 m_Input;
        private Vector3 m_MoveDir = Vector3.zero;
        private CharacterController m_CharacterController;
        private CollisionFlags m_CollisionFlags;
        private bool m_PreviouslyGrounded;
        private Vector3 m_OriginalCameraPosition;
        private float yaw;
        private float pitch;

        void Start()
        {
            m_CharacterController = GetComponent<CharacterController>();
            m_Camera = Camera.main;
            m_OriginalCameraPosition = m_Camera.transform.localPosition;
            m_FovKick.Setup(m_Camera);
            m_HeadBob.Setup(m_Camera, m_StepInterval);
			m_MouseLook.Init(transform , m_Camera.transform);
            yaw = transform.eulerAngles.y;
            pitch = transform.eulerAngles.x;
        }

        void Update(){
            RotateView();

            // Check for cursorlock, update the UI accordingly
            if (m_MouseLook.m_cursorIsLocked)
            {
                interfaceCont.WalkingInterfaceControl(true);
                mouseCont.MouseLocked(true);
            }
            else
            {
                interfaceCont.WalkingInterfaceControl(false);
                mouseCont.MouseLocked(false);
            }
        }

        private void FixedUpdate(){
            float speed;
            GetInput(out speed);

            Vector3 desiredMove = Vector3.zero;
            // Desktop
            if (!buildControl.onMobile)
            {
                m_Input = Vector3.zero;

                // Only allow movement using arrow keys
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    m_Input.y = 1;
                }
                if (Input.GetKey(KeyCode.DownArrow))
                {
                    m_Input.y = -1;
                }
                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    m_Input.x = -1;
                }
                if (Input.GetKey(KeyCode.RightArrow))
                {
                    m_Input.x = 1;
                }

                desiredMove = transform.forward * m_Input.y + transform.right * m_Input.x;
                RaycastHit hitInfo;
                Physics.SphereCast(transform.position, m_CharacterController.radius, Vector3.down, out hitInfo,
                                   m_CharacterController.height / 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
                desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

            }
            // Mobile
            else
            {
                // Movement
                desiredMove = mobMovLeft.Direction();
                desiredMove.z = desiredMove.y / 100;
                desiredMove.x = desiredMove.x / 100;
                desiredMove = transform.TransformDirection(desiredMove);
            }

            // Update move direction
            m_MoveDir.x = desiredMove.x*speed;
            m_MoveDir.z = desiredMove.z*speed;
            m_MoveDir.y = -m_StickToGroundForce;

            m_CollisionFlags = m_CharacterController.Move(m_MoveDir*Time.fixedDeltaTime);

            UpdateCameraPosition(speed);
            
        }

        private void UpdateCameraPosition(float speed){
            Vector3 newCameraPosition;
            if (!m_UseHeadBob){
                return;
            }
            if (m_CharacterController.velocity.magnitude > 0 && m_CharacterController.isGrounded){
                m_Camera.transform.localPosition =
                    m_HeadBob.DoHeadBob(m_CharacterController.velocity.magnitude +
                                      (speed*(m_IsWalking ? 1f : m_RunstepLenghten)));
                newCameraPosition = m_Camera.transform.localPosition;
                newCameraPosition.y = m_Camera.transform.localPosition.y - m_JumpBob.Offset();
            }
            else{
                newCameraPosition = m_Camera.transform.localPosition;
                newCameraPosition.y = m_OriginalCameraPosition.y - m_JumpBob.Offset();
            }
            m_Camera.transform.localPosition = newCameraPosition;
        }


        private void GetInput(out float speed){
            // Read input
            float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
            float vertical = CrossPlatformInputManager.GetAxis("Vertical");

            bool waswalking = m_IsWalking;

#if !MOBILE_INPUT
            // On standalone builds, walk/run speed is modified by a key press.
            // keep track of whether or not the character is walking or running
            m_IsWalking = !Input.GetKey(KeyCode.RightShift);
#endif
            // set the desired speed to be walking or running
            speed = m_IsWalking ? m_WalkSpeed : m_RunSpeed;
            m_Input = new Vector2(horizontal, vertical);

            // normalize input if it exceeds 1 in combined length:
            if (m_Input.sqrMagnitude > 1){
                m_Input.Normalize();
            }

            // handle speed change to give an fov kick
            // only if the player is going to a run, is running and the fovkick is to be used
            if (m_IsWalking != waswalking && m_UseFovKick && m_CharacterController.velocity.sqrMagnitude > 0){
                StopAllCoroutines();
                StartCoroutine(!m_IsWalking ? m_FovKick.FOVKickUp() : m_FovKick.FOVKickDown());
            }
        }

        public void SetCameraPos(Transform tran)
        {
            m_MouseLook.SetRotation(transform, m_Camera.transform, tran);
        }


        private void RotateView()
        {
            // Desktop       
            if (!buildControl.onMobile)
            {
                m_MouseLook.LookRotation(transform, m_Camera.transform);
            }
            // Mobile
            else
            {
                // Rotation
                Vector3 moveDirection = mobMovRight.Direction();
                yaw += moveDirection.x / 50;
                pitch -= moveDirection.y / 50;
                transform.eulerAngles = new Vector3(pitch, yaw, 0f);
            }     
        }


        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            Rigidbody body = hit.collider.attachedRigidbody;
            //dont move the rigidbody if the character is on top of it
            if (m_CollisionFlags == CollisionFlags.Below){
                return;
            }

            if (body == null || body.isKinematic){
                return;
            }

            body.AddForceAtPosition(m_CharacterController.velocity*0.1f, hit.point, ForceMode.Impulse);
        }

        public void EnableFirstPersonControl()
        {
            // Desktop       
            if (!buildControl.onMobile)
            {
               // m_MouseLook.SetCursorLock(true);
               // m_MouseLook.UpdateCursorLock();
            }  
        }

        public void DisableFirstPersonControl()
        {
            // Desktop       
            if (!buildControl.onMobile)
            {
                m_MouseLook.allowLooking = false;
                m_MouseLook.SetCursorLock(false);
                m_MouseLook.UpdateCursorLock();
                m_MouseLook.m_cursorIsLocked = false;
                m_MouseLook.lockCursor = false;
            }              
        }
    }
}