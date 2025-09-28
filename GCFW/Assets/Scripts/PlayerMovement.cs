using UnityEngine;
[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInputHandler inputHandler;
    [SerializeField] private Transform cameraTransform;
    [Header("Speeds")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float rotationSpeed;
    [Header("Asustments")]
    [SerializeField] private float inputDeadzone;

    private CharacterController controller;
    private Vector3 currentVelocity = Vector3.zero;
    private Vector3 moveDirection = Vector3.zero;
    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        if (inputHandler == null) inputHandler = GetComponent<PlayerInputHandler>();
        if (cameraTransform == null && Camera.main != null) cameraTransform = Camera.main.transform;
    }
    private void Update()
    {
        HandleMovement();
        handleRotation();
    }
    private void HandleMovement()
    {
        Vector2 input = inputHandler.MoveInput;
        if (input.magnitude < inputDeadzone)
        {
            moveDirection = Vector3.zero;
            return;
        }
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();
        moveDirection = forward * input.y + right * input.x;
        moveDirection.Normalize();
        controller.Move(moveDirection * walkSpeed * Time.deltaTime);
    }
    private void handleRotation()
    {
        if (moveDirection.magnitude > 0.001)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
