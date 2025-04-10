using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerVibrationTest : MonoBehaviour
{
    void Start()
    {
        if (Gamepad.current != null)
        {
            Debug.Log("Gamepad detected: " + Gamepad.current.name);
            Gamepad.current.SetMotorSpeeds(0.5f, 1.0f); // Constant vibration
            Debug.Log("Controller should be vibrating");
        }
        else
        {
            Debug.LogWarning("No gamepad connected.");
        }
    }

    void OnDisable()
    {
        if (Gamepad.current != null)
            Gamepad.current.SetMotorSpeeds(0f, 0f); // Stop vibration
    }
}
