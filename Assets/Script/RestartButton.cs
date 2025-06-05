using FishNet.Managing.Scened;
using FishNet;
using UnityEngine;

public class RestartButton : MonoBehaviour
{
    public void Restart()
    {
        // prev functionality
        //SceneLoadData sld = new SceneLoadData("RestartScene");
        //InstanceFinder.SceneManager.LoadGlobalScenes(sld);
        //SceneManager.LoadScene("RestartScene", LoadSceneMode.Single);

        GameData.is_looping = true;

        SceneLoadData sld = new SceneLoadData("MainMenu");
        InstanceFinder.SceneManager.LoadGlobalScenes(sld);
        

    }

}
