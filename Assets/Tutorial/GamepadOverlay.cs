using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GamepadOverlay : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI[] overlays;
    [SerializeField] string[] keyboard_controls;
    [SerializeField] string[] gamepad_controls;
    [SerializeField] GameObject[] input_ui; 
    /*
     * 0- left mouse ability robber
     * 1- right mouse ability robber
     * 2- left mouse ability ghost
     * 3- right mouse ability ghost
     * 4- picking up items
     * 
     * Tutorials have shortened versions
     */

    [SerializeField] Sprite[] gamepad_ui_sprites;
    [SerializeField] Sprite[] keyboard_ui_sprites;
 
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
        else if (Input.GetKeyDown(KeyCode.Joystick1Button0)) joystick_is_connected = true; // this part doesnt actually trigger

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
            InitControlVisuals(gamepad_ui_sprites);
        }
        else if (CheckKeyInput())
        {
            // If no controller connected use the keyboard input text
            if (debug_log) Debug.Log("No controller connected.");
            InitControlsText(keyboard_controls);
            InitControlVisuals(keyboard_ui_sprites);
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

    void InitControlVisuals(Sprite[] control_visuals)
    {
        for (int a = 0; a < input_ui.Length; a++)
        {
            if (input_ui[a].TryGetComponent(out SpriteRenderer sprite)) sprite.sprite = control_visuals[a];
            else if (input_ui[a].TryGetComponent(out Image image)) image.sprite = control_visuals[a];
        }
    }
}
