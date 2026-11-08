using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField]
    private CollisionChecker collisionChecker;
    [SerializeField]
    private float moveForce = 10f;
    [SerializeField]
    private float jumpForce = 10f;
    [SerializeField]
    private float maxSpeed = 8f;
    [SerializeField]
    private Vector3 camaraOffset = new Vector3(0f, 5f, -8f);
    [SerializeField]
    private Camera cameraObj;
    [SerializeField]
    private float cameraDistance = 8f;
    [SerializeField]
    private float cameraSensitivity = 0.2f;
    [SerializeField]
    private float minPitch = 0f;
    [SerializeField]
    private float maxPitch = 80f;

    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction cameraAction;
    private Rigidbody rigid;
    private Vector2 inputVec;
    private bool jumpPressed = false;
    private float yaw = 0f;
    private float pitch = 20f;

    private const string MoveActionName = "Move";
    private const string JumpActionName = "Jump";
    private const string CameraActionName = "Look";

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        moveAction = InputSystem.actions.FindAction(MoveActionName);
        jumpAction = InputSystem.actions.FindAction(JumpActionName);
        cameraAction = InputSystem.actions.FindAction(CameraActionName);
    }

    void Update()
    {
        if (jumpAction.WasPressedThisFrame())
            jumpPressed = true;
    }

    void FixedUpdate()
    {
        HandleMoveAction();
        HandleJumpAction();
    }

    void LateUpdate()
    {
        HandleCameraAction();
    }

    private void HandleMoveAction()
    {
        inputVec = moveAction.ReadValue<Vector2>();

        if (inputVec.x == 0 && inputVec.y == 0)
            return;

        Vector3 dir = Quaternion.Euler(0f, yaw, 0f) * new Vector3(inputVec.x, 0f, inputVec.y);
        rigid.AddForce(dir * moveForce);
        LimitHorizontalSpeed();
    }

    private void LimitHorizontalSpeed()
    {
        Vector3 velocity = rigid.linearVelocity;
        Vector3 horizontal = new Vector3(velocity.x, 0f, velocity.z);

        if (horizontal.magnitude > maxSpeed)
        {
            horizontal = horizontal.normalized * maxSpeed;
            rigid.linearVelocity = new Vector3(horizontal.x, velocity.y, horizontal.z);
        }
    }

    private void HandleJumpAction()
    {
        if (jumpPressed == true)
        {
            jumpPressed = false;
            if (collisionChecker.IsJumpAvaliable == true)
            {
                collisionChecker.WriteLastJumpedTime();
                rigid.AddForce(0, jumpForce, 0, ForceMode.Impulse);
            }
        }
    }

    private void HandleCameraAction()
    {
        if (cameraObj == null) return;

        Vector2 look = cameraAction.ReadValue<Vector2>();
        yaw += look.x * cameraSensitivity;
        pitch -= look.y * cameraSensitivity;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 offset = rotation * new Vector3(0f, 0f, -cameraDistance);

        cameraObj.transform.position = transform.position + offset;
        cameraObj.transform.LookAt(transform.position);
    }
}
