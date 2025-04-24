using UnityEngine;
using UnityEngine.SceneManagement;

public class Help : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        CheckForBackToMainMenu();
    }

    public void GoBack()
    {
        SceneManager.LoadScene("Play");
    }

    public void CheckForBackToMainMenu()
    {
        if (Input.GetKeyDown(KeyCode.JoystickButton2))
        {
            Debug.Log("Back to main menu");
            SceneManager.LoadScene("MainMenu");
        }
    }
}
