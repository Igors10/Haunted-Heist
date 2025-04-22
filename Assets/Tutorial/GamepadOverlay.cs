using UnityEngine;
using TMPro;

public class GamepadOverlay : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI[] overlays;
    [SerializeField] string[] keyboard_controls;
    [SerializeField] string[] gamepad_controls;

    bool is_controller_input = false;
    [SerializeField] bool debug_log = false;

    /*
     Ghost
    
    0- movement
    1- dash
    2- stepvision

     Robber

    0- movement
    1- flashlight
    2- item_pickup
    3- nightvision
    4- vents

    */

    void Start()
    {
        string[] control_text = (GameData.is_gamepad_used) ? gamepad_controls : keyboard_controls;
        InitControlsText(control_text);
    }

    private void Update()
    {
        InputCheck();
    }

    bool CheckJoystickAxes()
    {
        bool joystick_is_connected = false;

        if (Input.GetAxis("HorizontalJoystick") != 0f)
        {
            joystick_is_connected = true;
            if (debug_log) Debug.Log(Input.GetAxis("HorizontalJoystick"));
        }
        else if (Input.GetAxis("VerticalJoystick") != 0f)
        {
            joystick_is_connected = true;
            if (debug_log) Debug.Log(Input.GetAxis("VerticalJoystick"));
        }
        else if (Input.GetKeyDown(KeyCode.Joystick1Button0)) joystick_is_connected = true;

        return joystick_is_connected;
    }

    bool CheckKeyInput()
    {
        if (Input.GetKeyDown(KeyCode.A)) return true;
        if (Input.GetKeyDown(KeyCode.W)) return true;
        if (Input.GetKeyDown(KeyCode.S)) return true;
        if (Input.GetKeyDown(KeyCode.D)) return true;
        if (Input.GetKeyDown(KeyCode.Space)) return true;
        if (Input.GetKeyDown(KeyCode.Q)) return true;
        if (Input.GetKeyDown(KeyCode.E)) return true;
        if (Input.GetKeyDown(KeyCode.Mouse0)) return true;
        if (Input.GetKeyDown(KeyCode.Mouse1)) return true;
        if (Input.GetKeyDown(KeyCode.Mouse2)) return true;

        return false;
    }

    void InputCheck() 
    {
        // Updates the control scheme text real time based on the input
        
        if (CheckJoystickAxes())
        {
            // If controller is connected use the controller input text
            if (debug_log) Debug.Log("Controller connected");
            InitControlsText(gamepad_controls);
        }
        else if (CheckKeyInput())
        {
            // If no controller connected use the keyboard input text
            if (debug_log) Debug.Log("No controller connected.");
            InitControlsText(keyboard_controls);
        }
    }
    
    void InitControlsText(string[] control_text)
    {
        // setting text back to default
        for (int a = 0; a < control_text.Length; a++)
        {
            overlays[a].text = overlays[a].text.Replace(keyboard_controls[a], "{key}");
            overlays[a].text = overlays[a].text.Replace(gamepad_controls[a], "{key}");
        }

        // initialize new buttons into text
        for (int a = 0; a < control_text.Length; a++)
        {
            overlays[a].text = overlays[a].text.Replace("{key}", control_text[a]);
        }
    }
}
