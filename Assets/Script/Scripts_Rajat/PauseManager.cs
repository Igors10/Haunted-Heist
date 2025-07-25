using UnityEngine;
using UnityEngine.EventSystems; // Required for EventSystem
using UnityEngine.UI; // Required for Button

public class PauseManager : MonoBehaviour
{
    private bool isPaused = false;
    public GameObject pausePanel;
    public GameObject settingsPanel;
    public GameObject hints;

    // Reference to control if we should check for controller input
    [Header("Input Settings")]
    [SerializeField] private bool checkForControllerInput = true;
    [SerializeField] private KeyCode startButtonKeyCode = KeyCode.Joystick1Button7;

    [Header("UI Navigation")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button backButton;

    private EventSystem eventSystem;

    void Start()
    {
        if (pausePanel) pausePanel.SetActive(false);
        if (settingsPanel) settingsPanel.SetActive(false);

        eventSystem = EventSystem.current;
        if (eventSystem == null)
        {
            Debug.LogWarning("No EventSystem found in the scene.");
        }
    }

    void Update()
    {
        // Escape or Start
        if (Input.GetKeyDown(KeyCode.Escape) ||
            (checkForControllerInput && Input.GetKeyDown(startButtonKeyCode)))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        // Toggle pause panel
        pausePanel.SetActive(isPaused);
        hints.SetActive(isPaused);

        // Select the resume button when pause menu is opened
        if (isPaused && resumeButton != null && eventSystem != null)
        {
            eventSystem.SetSelectedGameObject(resumeButton.gameObject);
        }

        // Toggle player controls
        if (Game.Instance != null && Game.Instance.player != null)
        {
            InputController playerInput = Game.Instance.player.GetComponent<InputController>();
            if (playerInput != null)
            {
                playerInput.enabled = !isPaused;
            }
        }
    }

    public void Resume()
    {
        if (isPaused)
        {
            // Re-enable player controls
            if (Game.Instance != null && Game.Instance.player != null)
            {
                InputController playerInput = Game.Instance.player.GetComponent<InputController>();
                if (playerInput != null)
                {
                    playerInput.enabled = true;
                }
            }

            // Reset pause state
            isPaused = false;
            pausePanel.SetActive(false);
        }
    }

    public void LoadSettingsPanel()
    {
        settingsPanel.SetActive(true);
        pausePanel.SetActive(false);

        // Select the back button when settings panel is opened
        if (backButton != null && eventSystem != null)
        {
            eventSystem.SetSelectedGameObject(backButton.gameObject);
        }
    }

    public void BackButton()
    {
        settingsPanel.SetActive(false);
        pausePanel.SetActive(true);

        // Re-select the resume button when going back to pause menu
        if (resumeButton != null && eventSystem != null)
        {
            eventSystem.SetSelectedGameObject(resumeButton.gameObject);
        }
    }
}