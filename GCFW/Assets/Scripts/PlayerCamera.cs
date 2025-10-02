using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private float lookSpeedX = 20f;
    [SerializeField] private float lookSpeedY = 20f;
    [SerializeField] private float upperLookLimit = -60f;
    [SerializeField] private float lowerLookLimit = 60f;

    private Transform playerBody;
    private float xRotation = 0f;

    private PlayerControls controls;
    private Vector2 lookInput = Vector2.zero;

    private void Awake()
    {
        playerBody = transform.parent;//The parent is the player
        controls = new PlayerControls();
    }

    private void OnEnable()
    {
        //Hear the camera events
        controls.Player.Look.performed += OnLookPerformed;
        controls.Player.Look.canceled += OnLookCanceled;
        controls.Player.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Look.performed -= OnLookPerformed;
        controls.Player.Look.canceled -= OnLookCanceled;
        controls.Player.Disable();
    }

    private void OnLookPerformed(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    private void OnLookCanceled(InputAction.CallbackContext context)
    {
        lookInput = Vector2.zero;
    }

    private void Update()
    {
        HandleLook();
    }

    private void HandleLook()
    {
        float mouseX = lookInput.x * lookSpeedX * Time.deltaTime;
        float mouseY = lookInput.y * lookSpeedY * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, upperLookLimit, lowerLookLimit);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
