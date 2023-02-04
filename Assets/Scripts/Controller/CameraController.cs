using UnityEngine;

public class CameraController : MonoBehaviour, IShakable
{
    [Header("General")]
    [SerializeField] private Vector2 _cameraRotationSpeed = new Vector2(45f, 45f);
    [SerializeField] private Vector2 _cameraPitchLimit = new Vector2(-60f, 60f);

    [Header("Shake")]
    [SerializeField] private Transform _cameraShaker = null;
    [SerializeField] private Shake.ShakeSettings _shakeSettings = Shake.ShakeSettings.Default;

    private Shake _shake;

    public void UpdateCamera(Vector2 mouseInput)
    {
        Rotate(mouseInput);

        (Vector3 pos, Quaternion rot)? shakeData = _shake.Evaluate(_cameraShaker);
        _cameraShaker.localPosition = shakeData?.pos ?? Vector3.zero;
    }

    public void AddTrauma(float trauma)
    {
        _shake?.AddTrauma(trauma);
    }

    public void SetTrauma(float trauma)
    {
        _shake?.SetTrauma(trauma);
    }

    public void ToggleCursor(bool state)
    {
        Cursor.visible = state;
        Cursor.lockState = state ? CursorLockMode.None : CursorLockMode.Locked;
    }

    public void Rotate(Vector2 mouseInput)
    {
        Vector2 speed = _cameraRotationSpeed / 0.016f;
        Vector3 angularVelocity = new Vector3(mouseInput.x * speed.x, mouseInput.y * speed.y);
        angularVelocity *= Time.deltaTime;

        Vector3 eulerAngles = transform.localEulerAngles;
        eulerAngles.y += angularVelocity.x;
        eulerAngles.x -= angularVelocity.y;

        bool lookingUp = transform.forward.y > 0f;
        eulerAngles.x = lookingUp
                        ? Mathf.Max(eulerAngles.x - 360f, _cameraPitchLimit.x) + 360f
                        : Mathf.Min(eulerAngles.x, _cameraPitchLimit.y);

        transform.localEulerAngles = eulerAngles;
    }

    public void ModifyMovementVelocity(ref Vector3 velocity)
    {
        velocity = Quaternion.Euler(0f, transform.localEulerAngles.y, 0f) * velocity;
    }

    private System.Collections.IEnumerator Start()
    {
        ToggleCursor(false);
        _shake = new Shake(_shakeSettings);

        yield return new WaitForEndOfFrame();
        transform.localEulerAngles = Vector3.zero;
    }
}
