using UnityEngine;
using UnityEngine.SceneManagement;

public class ChooseTutorialScript : MonoBehaviour
{

    public void RobberTutorial()
    {
        //reset the tutorial stage in static script
        TutorialProgress.part = 1;
        Debug.Log("FRANEK::Twoja stara to z³odziej");
        SceneManager.LoadScene("Lvl_Tutorial_Robber", LoadSceneMode.Single);
    }

    public void GhostTutorial()
    {
        //reset the tutorial stage in static script
        TutorialProgress.part = 1;
        Debug.Log("FRANEK::Twoja stara to duch");
        SceneManager.LoadScene("Lvl_Tutorial_Ghost", LoadSceneMode.Single);
    }


    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }


    // If on Play or Tutorial scene, and "B" or "Circle" is pressed on controller, go back to main menu
    public void CheckForBackToMainMenu()
    {
        if (Input.GetKeyDown(KeyCode.JoystickButton2))
        {
            Debug.Log("Back to main menu");
            SceneManager.LoadScene("MainMenu");
        }
    }


}
