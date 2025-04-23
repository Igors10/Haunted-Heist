using UnityEngine;

public class JoystickButtonChecker : MonoBehaviour
{
    void Update()
    {
        // Check the first 20 joystick buttons
        for (int i = 0; i < 20; i++)
        {
            if (Input.GetKeyDown("joystick button " + i))
            {
                Debug.Log("Joystick button " + i + " pressed");
            }
        }

        // Check the RT axis value
        float rtValue = Input.GetAxis("RT");
        Debug.Log("RT axis value: " + rtValue);

    }
}
