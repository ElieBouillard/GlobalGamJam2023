using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Vector2 _cameraRotationSpeed = new Vector2(45f, 45f);
    [SerializeField] private Vector2 _cameraPitchLimit = new Vector2(-60f, 60f);

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

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
