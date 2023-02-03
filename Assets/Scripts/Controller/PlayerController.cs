using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody _rigidbody = null;
    [SerializeField] private CameraController _cameraController = null;

    [Header("Movement")]
    [SerializeField, Min(0f)] private float _movementSpeed = 10f;

    [Header("Slide")]
    [SerializeField, Min(0f)] private float _slideDuration = 0.5f;
    [SerializeField, Min(0f)] private float _slideSpeed = 30f;
    [SerializeField, Min(0f)] private float _slideCooldown = 0.5f;
    [SerializeField, Range(0f, 1f)] private float _slideTrauma = 0.5f;

    private Vector2 _movementInput;
    private Vector2 _mouseInput;
    private bool _attackInput;
    private bool _slideInput;

    private Vector3? _slideDirection;
    private float _slideCooldownTimer;

    #region Inputs
    private void RegisterInputs()
    {
        _movementInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        _mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        _attackInput = Input.GetMouseButtonDown(0);
        _slideInput = Input.GetKeyDown(KeyCode.LeftShift);
    }

    private void CleanupInputs()
    {
        _movementInput = Vector2.zero;
        _mouseInput = Vector2.zero;
        _attackInput = false;
        _slideInput = false;
    }
    #endregion // Inputs

    #region Slide
    private void Slide()
    {
        if (!CanSlide())
            return;

        _slideCooldownTimer = _slideCooldown;
        StartCoroutine(SlideCoroutine(_movementInput));
        _cameraController.SetTrauma(_slideTrauma);
    }

    private IEnumerator SlideCoroutine(Vector3 direction)
    {
        _slideDirection = new Vector3(direction.x, 0f, direction.y);
        yield return new WaitForSeconds(_slideDuration);
        _slideDirection = null;
    }

    private bool CanSlide()
    {
        return _slideDirection == null && _slideCooldownTimer <= 0f;
    }

    private void UpdateSlideCooldown()
    {
        if (_slideDirection == null)
            _slideCooldownTimer -= Time.deltaTime;
    }
    #endregion // Slide

    private Vector3 GetMovementDirection()
    {
        return _slideDirection ?? new Vector3(_movementInput.x, _rigidbody.velocity.y, _movementInput.y);
    }

    private float GetMovementSpeed()
    {
        return _slideDirection != null ? _slideSpeed : _movementSpeed;
    }

    private void Move()
    {
        Vector3 velocity = GetMovementDirection();
        _cameraController.ModifyMovementVelocity(ref velocity);
        velocity.Normalize();
        velocity *= GetMovementSpeed();
        _rigidbody.velocity = velocity;
    }

    private void Update()
    {
        RegisterInputs();
        
        if (_slideInput)
            Slide();

        UpdateSlideCooldown();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void LateUpdate()
    {
        _cameraController.UpdateCamera(_mouseInput);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        if (_slideDirection != null)
            Gizmos.DrawLine(transform.position, transform.position + _slideDirection.Value);
    }
}