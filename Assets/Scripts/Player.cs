using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float moveForce = 10f;
    [SerializeField]
    private float jumpForce = 10f;
    [SerializeField]
    private float maxSpeed = 8f;

    private InputAction moveAction;
    private InputAction jumpAction;
    private Rigidbody rigid;
    private Collider coll;
    private Vector2 inputVec;
    private bool jumpPressed = false;

    private const string MoveActionName = "Move";
    private const string JumpActionName = "Jump";

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        coll = GetComponent<Collider>();
    }

    void OnEnable()
    {
        moveAction = InputSystem.actions.FindAction(MoveActionName);
        jumpAction = InputSystem.actions.FindAction(JumpActionName);
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

    private void HandleMoveAction()
    {
        inputVec = moveAction.ReadValue<Vector2>();

        if (inputVec.x == 0 && inputVec.y == 0)
            return;

        rigid.AddForce(inputVec.x * moveForce, 0, inputVec.y * moveForce);
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
            rigid.AddForce(0, jumpForce, 0, ForceMode.Impulse);
        }
    }
}
