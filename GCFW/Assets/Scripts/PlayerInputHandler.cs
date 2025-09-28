using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerControls controls;
    public Vector2 MoveInput { get; private set; } = Vector2.zero;
    public Vector2 LookInput { get; private set; } = Vector2.zero;
    public bool IsJumpPressed { get; private set; } = false;
    public bool IsSprintPressed { get; private set; } = false;
    public bool IsCrouchPressed { get; private set; } = false;
    private void Awake()
    {
        controls = new PlayerControls();
    }
    private void OnEnable()
    {
        if (controls != null) controls = new PlayerControls();
        controls.Player.Move.performed += OnMovePerformed;
        controls.Player.Move.canceled += OnMoveCanceled;
        controls.Player.Look.performed += OnLookPerformed;
        controls.Player.Look.canceled += OnLookCanceled;
        controls.Player.Jump.performed += OnJumpPerformed;
        controls.Player.Jump.canceled += OnJumpCanceled;
        controls.Player.Sprint.performed += OnSprintPerformed;
        controls.Player.Sprint.canceled += OnSprintCanceled;
        controls.Player.Crouch.performed += OnCrouchPerformed;
        controls.Player.Crouch.canceled += OnCrouchCanceled;

        controls.Player.Enable();
        controls.UI.Enable();
    }
    private void OnDisable()
    {
        controls.Player.Move.performed -= OnMovePerformed;
        controls.Player.Move.canceled -= OnMoveCanceled;
        controls.Player.Look.performed -= OnLookPerformed;
        controls.Player.Look.canceled -= OnLookCanceled;
        controls.Player.Jump.performed -= OnJumpPerformed;
        controls.Player.Jump.canceled -= OnJumpCanceled;
        controls.Player.Sprint.performed -= OnSprintPerformed;
        controls.Player.Sprint.canceled -= OnSprintCanceled;
        controls.Player.Crouch.performed -= OnCrouchPerformed;
        controls.Player.Crouch.canceled -= OnCrouchCanceled;

        controls.Player.Disable();
        controls.UI.Disable();
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();
    }
    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        MoveInput = Vector2.zero;
    }
    private void OnLookPerformed(InputAction.CallbackContext context)
    {
        LookInput = context.ReadValue<Vector2>();
    }
    private void OnLookCanceled(InputAction.CallbackContext context)
    {
        LookInput = Vector2.zero;
    }
    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        IsJumpPressed = true;
    }
    private void OnJumpCanceled(InputAction.CallbackContext context)
    {
        IsJumpPressed = false;
    }
    private void OnSprintPerformed(InputAction.CallbackContext context)
    {
        IsSprintPressed = true;
    }
    private void OnSprintCanceled(InputAction.CallbackContext context)
    {
        IsSprintPressed = false;
    }
    private void OnCrouchPerformed(InputAction.CallbackContext context)
    {
        IsCrouchPressed = true;
    }
    private void OnCrouchCanceled(InputAction.CallbackContext context)
    {
        IsCrouchPressed = false;
    }
    private void OnDestroy()
    {
        if (controls != null)
        {
            controls.Dispose();
            controls = null;
        }
    }
}
