using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
//using FishNet.Managing.Scened;
using FishNet;

public class Restart : MonoBehaviour
{
    [SerializeField] float restart_time;

    IEnumerator BackToLevel() // previously this was just called on start
    {
        GameData.is_game_over = false;
        GameData.is_ghost_wild = false;


        yield return new WaitForSeconds(restart_time);
        SceneManager.LoadScene("Lvl_Tilemap", LoadSceneMode.Single);
        //SceneLoadData sld = new SceneLoadData("MainMenu");
        //InstanceFinder.SceneManager.LoadGlobalScenes(sld);
        //InstanceFinder.SceneManager.LoadGlobalScenes(sld);
    }

    
}
