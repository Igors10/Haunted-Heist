using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
//using FishNet.Managing.Scened;
using FishNet;

public class Restart : MonoBehaviour
{
    [SerializeField] float restart_time;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(BackToLevel());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator BackToLevel()
    {
        GameData.is_game_over = false;
        GameData.ghost_is_wild = false;


        yield return new WaitForSeconds(restart_time);
        SceneManager.LoadScene("Lvl_Tilemap", LoadSceneMode.Single);
        //SceneLoadData sld = new SceneLoadData("MainMenu");
        //InstanceFinder.SceneManager.LoadGlobalScenes(sld);
        //InstanceFinder.SceneManager.LoadGlobalScenes(sld);
    }
}
