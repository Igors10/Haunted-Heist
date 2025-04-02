using FishNet.Managing.Scened;
using FishNet;
using UnityEngine;

public class RestartButton : MonoBehaviour
{
    public void Restart()
    {
        SceneLoadData sld = new SceneLoadData("RestartScene");
        InstanceFinder.SceneManager.LoadGlobalScenes(sld);
        //SceneManager.LoadScene("RestartScene", LoadSceneMode.Single);
    }

}
