using UnityEngine;

namespace SUBR.Player
{
    /// <summary>
    /// Basic TPS locomotion. Animator optional.
    /// Tum Editor mein CharacterController + model child assign karo.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerController : MonoBehaviour
    {
        [Header("Move")]
        [SerializeField] float walkSpeed = 4.5f;
        [SerializeField] float sprintSpeed = 7f;
        [SerializeField] float gravity = -20f;
        [SerializeField] float jumpHeight = 1.2f;
        [SerializeField] Transform cameraPivot;

        [Header("Look")]
        [SerializeField] float mouseSensitivity = 1.4f;
        [SerializeField] float minPitch = -40f;
        [SerializeField] float maxPitch = 70f;
        [SerializeField] bool lockCursor = true;

        CharacterController _cc;
        float _verticalVel;
        float _pitch;
        float _yaw;
        public bool InputEnabled { get; set; } = true;
        public bool IsGrounded => _cc != null && _cc.isGrounded;
        public Vector3 PlanarVelocity { get; private set; }

        void Awake()
        {
            _cc = GetComponent<CharacterController>();
            _yaw = transform.eulerAngles.y;
            if (cameraPivot == null)
            {
                var t = transform.Find("CameraPivot");
                if (t) cameraPivot = t;
            }
        }

        void Start()
        {
            if (lockCursor)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        void Update()
        {
            if (!InputEnabled) return;
            Look();
            Move();
        }

        void Look()
        {
            float mx = Input.GetAxisRaw("Mouse X") * mouseSensitivity;
            float my = Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
            _yaw += mx;
            _pitch = Mathf.Clamp(_pitch - my, minPitch, maxPitch);
            transform.rotation = Quaternion.Euler(0f, _yaw, 0f);
            if (cameraPivot) cameraPivot.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
        }

        void Move()
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            Vector3 input = new Vector3(h, 0f, v).normalized;
            bool sprint = Input.GetKey(KeyCode.LeftShift);
            float speed = sprint ? sprintSpeed : walkSpeed;

            Vector3 world = transform.TransformDirection(input) * speed;
            PlanarVelocity = world;

            if (_cc.isGrounded && _verticalVel < 0f) _verticalVel = -2f;
            if (_cc.isGrounded && Input.GetButtonDown("Jump"))
                _verticalVel = Mathf.Sqrt(jumpHeight * -2f * gravity);

            _verticalVel += gravity * Time.deltaTime;
            world.y = _verticalVel;
            _cc.Move(world * Time.deltaTime);
        }

        public void Teleport(Vector3 pos)
        {
            _cc.enabled = false;
            transform.position = pos;
            _cc.enabled = true;
            _verticalVel = 0f;
        }
    }
}
