using UnityEngine;
using FishNet;
using FishNet.Managing.Scened;
using UnityEngine.SceneManagement;
using System.Linq;

public class RestartScript : MonoBehaviour
{

    bool is_restarting = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Game.Instance.restart_manager = this;
    }

    private void Update()
    {
        RestartInput();
    }

    void RestartInput()
    {
        if (GameData.is_game_over && Input.GetButtonDown("Restart"))
        {
            Restart();
        }
    }

    public void Restart()
    {
        if (is_restarting) return;
        is_restarting = true;

        Debug.Log("RESTART: Loading RestartScene...");

        // Subscribe to OnLoadEnd to detect when intermediate scene finishes loading
        InstanceFinder.SceneManager.OnLoadEnd += OnIntermediateSceneLoaded;

        SceneLoadData sld = new SceneLoadData("RestartScene")
        {
            ReplaceScenes = ReplaceOption.All
        };

        InstanceFinder.SceneManager.LoadGlobalScenes(sld);
    }

    private void OnIntermediateSceneLoaded(SceneLoadEndEventArgs args)
    {
        // Check if IntermediateScene is among the loaded scenes
        bool loadedIntermediate = args.LoadedScenes.Any(scene => scene.name == "RestartScene");
        if (!loadedIntermediate)
            return;

        Debug.Log("RESTART: RestartScene loaded, reloading Lvl_Tilemap");

        // Unsubscribe to avoid firing again
        InstanceFinder.SceneManager.OnLoadEnd -= OnIntermediateSceneLoaded;

        SceneLoadData mainSceneLoad = new SceneLoadData("Lvl_Tilemap")
        {
            ReplaceScenes = ReplaceOption.All
        };

        InstanceFinder.SceneManager.LoadGlobalScenes(mainSceneLoad);
        is_restarting = false;
        GameData.is_game_over = false;
    }
}
