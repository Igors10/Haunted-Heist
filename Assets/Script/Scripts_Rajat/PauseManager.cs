using UnityEngine;

public class PauseManager : MonoBehaviour
{

    private bool isPaused = false;
    public GameObject pausePanel;
    public GameObject settingsPanel;

    void Start()
    {

    }

    void Update()
    {
        // The pause menu code, currently it disables the script for the robber, but it should disable the script of the current player.

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pausePanel.SetActive(!pausePanel.activeSelf);
            Game.Instance.player.GetComponent<InputController>().enabled = (Game.Instance.player.GetComponent<InputController>().enabled) ? false : true;
            isPaused = !isPaused;
        }

        // This means that the updates will stop if game is paused
        //if (isPaused) return;

    }

    public void Resume()
    {
        isPaused = false;
        pausePanel.SetActive(false);
    }

    public void LoadSettingsPanel()
    {
        settingsPanel.SetActive(true);
        pausePanel.SetActive(false);
    }

    public void BackButton()
    {
        settingsPanel.SetActive(false);
        pausePanel.SetActive(true);
    }
}