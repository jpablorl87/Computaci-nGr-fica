using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
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
    [Header("Animations")]
    [SerializeField] private Animator animator;
    [Header("Ajustments")]
    [SerializeField] private float inputDeadzone;

    private CharacterController controller;
    private Vector3 currentVelocity = Vector3.zero;
    private Vector3 moveDirection = Vector3.zero;
    private Vector3 horizontalVelocity = Vector3.zero;
    private Vector3 verticalVelocity = Vector3.zero;
    private float coyoteTimer = 0;
    private float jumpBufferTimer = 0;
    private bool isCurrentlyJumping = false;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        if (inputHandler == null) inputHandler = GetComponent<PlayerInputHandler>();
        if (cameraTransform == null && Camera.main != null) cameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        HandleMovement();
        UpdateAnimator();
        SyncRotationWithCamera();
        HandleJump();
        HandleGravity();
        ApplyMovement();
    }
    /*private void HandleMovement()
    {
        Vector2 input = inputHandler.MoveInput;
        if (input.magnitude < inputDeadzone)
        {
            horizontalVelocity = Vector3.zero;
            moveDirection = Vector3.zero;
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
    }*/
    private void HandleMovement()
    {
        Vector2 input = inputHandler.MoveInput;

        bool noInput = input.magnitude < inputDeadzone;

        if (noInput)
        {
            horizontalVelocity = Vector3.zero;

            if (!isCurrentlyJumping)
                moveDirection = Vector3.zero;

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
            if (isCurrentlyJumping)
            {
                isCurrentlyJumping = false;
                animator.SetBool("IsJumping", false);
            }
            if (verticalVelocity.y < 0)
                verticalVelocity.y = -2f;

            coyoteTimer = coyoteTime;
        }
        else
        {
            coyoteTimer -= Time.deltaTime;
        }
        verticalVelocity.y += gravity * Time.deltaTime;
        if (verticalVelocity.y < 0)
        {
            verticalVelocity.y += gravity * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (verticalVelocity.y > 0 && !inputHandler.IsJumpPressed)
        {
            verticalVelocity.y += gravity * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
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

            isCurrentlyJumping = true;
            animator.SetBool("IsJumping", true);
        }
        if (controller.isGrounded && isCurrentlyJumping)
        {
            //isCurrentlyJumping = false;
            StartCoroutine(ResetJumpingFlag());
        }
    }
    private void ApplyMovement()
    {
        Vector3 totalVelocity = horizontalVelocity + verticalVelocity;
        controller.Move(totalVelocity * Time.deltaTime);
    }
    private void UpdateAnimator()
    {
        Vector3 localMove = transform.InverseTransformDirection(moveDirection);
        float moveX = localMove.x;
        float moveZ = localMove.z;
        if (Mathf.Abs(moveX) < 0.1f) moveX = 0f;
        if (Mathf.Abs(moveZ) < 0.1f) moveZ = 0f;
        Debug.Log($"MoveX: {moveX}, MoveY: {moveZ}, IsJumping: {isCurrentlyJumping}");
        animator.SetFloat("MoveX", moveX, 0.1f, Time.deltaTime);
        animator.SetFloat("MoveZ", moveZ, 0.1f, Time.deltaTime);
    }
    private IEnumerator ResetJumpingFlag()
    {
        yield return new WaitForSeconds(0.1f);
        isCurrentlyJumping = false;
        animator.SetBool("IsJumping", false);
    }
}