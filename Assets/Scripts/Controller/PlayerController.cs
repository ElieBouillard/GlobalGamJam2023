using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody _rigidbody = null;
    [SerializeField] private Camera _camera = null;

    [Header("Movement")]
    [SerializeField, Min(0f)] private float _movementSpeed = 10f;
    
    [Header("Camera")]
    [SerializeField] private Vector2 _cameraRotationSpeed = new Vector2(45f, 45f);

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

    private void RotateCamera()
    {
        Vector2 speed = _cameraRotationSpeed / 0.016f;
        Vector3 angularVelocity = new Vector3(_mouseInput.x * speed.x, _mouseInput.y * speed.y);
        angularVelocity *= Time.deltaTime;

        Vector3 eulerAngles = _camera.transform.localEulerAngles;
        eulerAngles.y += angularVelocity.x;
        eulerAngles.x -= angularVelocity.y;
        _camera.transform.localEulerAngles = eulerAngles;
    }

    private void Move()
    {
        Vector3 velocity = new Vector3(_movementInput.x, 0f, _movementInput.y);
        velocity = Quaternion.Euler(0f, _camera.transform.localEulerAngles.y, 0f) * velocity;
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
        RotateCamera();
    }
}