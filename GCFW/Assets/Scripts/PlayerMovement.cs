using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInputHandler inputHandler;
    [SerializeField] private Transform cameraTransform;
    [Header("Speeds")]
    [SerializeField] private float walkSpeed;
    [Header("Physics")]
    [SerializeField] private float gravity;
    [SerializeField] private float groundedCheckDistance;
    [Header("Jump")]
    [SerializeField] private float jumpHeight;
    [SerializeField] private float coyoteTime;
    [SerializeField] private float jumpBufferTime;
    [SerializeField] private float fallMultiplier;
    [SerializeField] private float lowJumpMultiplier;
    [Header("Ajustments")]
    [SerializeField] private float inputDeadzone;

    private CharacterController controller;
    private Vector3 currentVelocity = Vector3.zero;
    private Vector3 moveDirection = Vector3.zero;
    private Vector3 horizontalVelocity = Vector3.zero;
    private Vector3 verticalVelocity = Vector3.zero;
    private float coyoteTimer = 0;
    private float jumpBufferTimer = 0;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        if (inputHandler == null) inputHandler = GetComponent<PlayerInputHandler>();
        if (cameraTransform == null && Camera.main != null) cameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        HandleMovement();
        SyncRotationWithCamera();
        HandleJump();
        HandleGravity();
        ApplyMovement();
    }
    private void HandleMovement()
    {
        Vector2 input = inputHandler.MoveInput;
        if (input.magnitude < inputDeadzone)
        {
            horizontalVelocity = Vector3.zero;
            return;
        }

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        moveDirection = (forward * input.y + right * input.x).normalized;
        horizontalVelocity = moveDirection * walkSpeed;
    }
    private void SyncRotationWithCamera()
    {
        Vector3 cameraForward = cameraTransform.forward;
        cameraForward.y = 0;

        if (cameraForward.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(cameraForward);
            transform.rotation = targetRotation;
        }
    }
    private void HandleGravity()
    {
        if (controller.isGrounded)
        {
            coyoteTimer = coyoteTime;
            if (verticalVelocity.y < 0)
                verticalVelocity.y = -2f;
        }
        else
        {
            coyoteTimer -= Time.deltaTime;
        }
        verticalVelocity.y += gravity * Time.deltaTime;

        if (verticalVelocity.y < 0)
            verticalVelocity.y += gravity * (fallMultiplier - 1) * Time.deltaTime;
        else if (verticalVelocity.y > 0 && !inputHandler.IsJumpPressed)
            verticalVelocity.y += gravity * (lowJumpMultiplier - 1) * Time.deltaTime;
    }
    private void HandleJump()
    {
        if (inputHandler.IsJumpPressed) jumpBufferTimer = jumpBufferTime;
        else jumpBufferTimer -= Time.deltaTime;

        if (jumpBufferTimer > 0 && coyoteTimer > 0)
        {
            verticalVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumpBufferTimer = 0;
            coyoteTimer = 0;
        }
    }
    private void ApplyMovement()
    {
        Vector3 totalVelocity = horizontalVelocity + verticalVelocity;
        controller.Move(totalVelocity * Time.deltaTime);
    }
}