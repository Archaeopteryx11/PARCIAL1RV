using UnityEngine;
using UnityEngine.InputSystem;

public class SphereCameraLook : MonoBehaviour
{
    public float mouseSensitivity = 0.2f;
    public float gamepadSensitivity = 120f;
    public float verticalClamp = 80f;

    private Vector2 lookInput;
    private float xRotation = 0f;
    private bool isGamepad;

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
        isGamepad = context.control?.device is Gamepad;
    }

    void Update()
    {
        float sens = isGamepad ? gamepadSensitivity * Time.deltaTime : mouseSensitivity * 100f * Time.deltaTime;

        float x = lookInput.x * sens;
        float y = lookInput.y * sens;

        transform.Rotate(Vector3.up * x, Space.World);

        xRotation -= y;
        xRotation = Mathf.Clamp(xRotation, -verticalClamp, verticalClamp);

        transform.localRotation = Quaternion.Euler(xRotation, transform.localEulerAngles.y, 0);
    }
}