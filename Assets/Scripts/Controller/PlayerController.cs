using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody _rigidbody = null;
    [SerializeField] private CameraController _cameraController = null;

    [Header("Movement")]
    [SerializeField, Min(0f)] private float _movementSpeed = 10f;

    [Header("Dash")]
    [SerializeField, Min(0f)] private float _dashDuration = 0.5f;
    [SerializeField, Min(0f)] private float _dashDistance = 5f;

    private Vector2 _movementInput;
    private Vector2 _mouseInput;
    private bool _attackInput;
    private bool _dashInput;

    private void RegisterInputs()
    {
        _movementInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        _mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        _attackInput = Input.GetMouseButtonDown(0);
        _dashInput = Input.GetKeyDown(KeyCode.LeftShift);
    }

    private void CleanupInputs()
    {
        _movementInput = Vector2.zero;
        _mouseInput = Vector2.zero;
        _attackInput = false;
        _dashInput = false;
    }

    private void Move()
    {
        Vector3 velocity = new Vector3(_movementInput.x, _rigidbody.velocity.y, _movementInput.y);
        _cameraController.ModifyMovementVelocity(ref velocity);
        velocity.Normalize();
        velocity *= _movementSpeed;
        _rigidbody.velocity = velocity;
    }

    private void Update()
    {
        RegisterInputs();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void LateUpdate()
    {
        _cameraController.Rotate(_mouseInput);
    }
}