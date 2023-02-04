using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody _rigidbody = null;
    [SerializeField] private CameraController _cameraController = null;

    [Header("Movement")]
    [SerializeField, Min(0f)] private float _movementSpeed = 10f;
    [SerializeField, Min(0f)] private LayerMask _groundLayerMask = 0;
    [SerializeField, Min(10f)] private float _groundSnapDistance = 100f;

    [Header("Slide")]
    [SerializeField, Min(0f)] private float _slideDuration = 0.5f;
    [SerializeField, Min(0f)] private float _slideSpeed = 30f;
    [SerializeField, Min(0f)] private float _slideCooldown = 0.5f;
    [SerializeField, Range(0f, 1f)] private float _slideTrauma = 0.5f;
    [SerializeField, Min(0f)] private float _slideSpeedDecrementDuration = 3f;
    [SerializeField] private AnimationCurve _slideSpeedDecrementCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    [SerializeField] private float _slideFOV = 120f;

    [Header("Attack")]
    [SerializeField] private Weapon _startingWeapon = null;
    [SerializeField] private bool _canSlideAttack = true;

    // Inputs.
    private Vector2 _movementInput;
    private Vector2 _mouseInput;
    private bool _attackInput;
    private bool _slideInput;

    // Slide.
    private Vector3? _slideDirection;
    private float _slideCooldownTimer;
    private float _slideSpeedBuffer;
    private Coroutine _slideSpeedBufferDecrementCoroutine;

    // Attack.
    private PlayerAttackModule _attackModule;

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
        Vector3 direction = Vector3.Magnitude(_movementInput) > 0.1f ? _movementInput : new Vector3(0f, 1f);

        StartCoroutine(SlideCoroutine(direction));

        if (direction.y > direction.x)
            _cameraController.SetFOV(_slideFOV, _slideDuration * 0.5f);
    
        _cameraController.SetTrauma(_slideTrauma);
        // TODO: slide audio.
    }

    private IEnumerator SlideCoroutine(Vector3 direction)
    {
        _slideDirection = new Vector3(direction.x, 0f, direction.y);
        yield return new WaitForSeconds(_slideDuration);
        _slideDirection = null;

        _slideSpeedBuffer = _slideSpeed;
        _slideSpeedBufferDecrementCoroutine = StartCoroutine(DecrementSlideSpeedBufferCoroutine());

        _cameraController.ResetFOV(0.15f);
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

    private IEnumerator DecrementSlideSpeedBufferCoroutine()
    {
        float initialBuffer = _slideSpeedBuffer;

        for (float t = 0f; t <= 1f; t += Time.deltaTime / _slideSpeedDecrementDuration)
        {
            _slideSpeedBuffer = Mathf.Lerp(0f, initialBuffer, _slideSpeedDecrementCurve.Evaluate(t));
            yield return null;
        }

        _slideSpeedBuffer = 0f;
    }
    #endregion // Slide

    #region Attack
    private void Attack()
    {
        if (!CanAttack())
            return;

        _attackModule.Attack(shookOnAttack: _cameraController);
    }

    private bool CanAttack()
    {
        return (_slideDirection == null || _canSlideAttack)
               && _attackModule?.CanAttack() == true;
    }
    #endregion Attack

    private Vector3 GetMovementDirection()
    {
        return _slideDirection ?? new Vector3(_movementInput.x, _rigidbody.velocity.y, _movementInput.y);
    }

    private float GetMovementSpeed()
    {
        if (_slideDirection != null)
            return _slideSpeed;

        if (_movementInput.y == 0f || _movementInput.x != 0f)
        {
            if (_slideSpeedBufferDecrementCoroutine != null)
            {
                StopCoroutine(_slideSpeedBufferDecrementCoroutine);
                _slideSpeedBufferDecrementCoroutine = null;
            }
        
            _slideSpeedBuffer = 0f;
        }

        return _movementSpeed + _slideSpeedBuffer;
    }

    private void Move()
    {
        Vector3 velocity = GetMovementDirection();
        _cameraController.ModifyMovementVelocity(ref velocity);
        velocity.Normalize();
        velocity *= GetMovementSpeed();
        _rigidbody.velocity = velocity;
    }

    private void SnapToGround()
    {
        Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.down, out RaycastHit hit, _groundSnapDistance, _groundLayerMask);

        if (hit.collider == null)
            return;

        transform.position = new Vector3(transform.position.x, hit.point.y + 0.05f, transform.position.z);
    }

    private void Start()
    {
        _attackModule = new PlayerAttackModule();
        _attackModule.SetWeapon(_startingWeapon);
    }

    private void Update()
    {
        RegisterInputs();
        _attackModule.Update();

        if (_slideInput)
            Slide();

        if (_attackInput)
            Attack();

        UpdateSlideCooldown();
    }

    private void FixedUpdate()
    {
        Move();
        SnapToGround();
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