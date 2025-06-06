using FishNet;
using FishNet.Managing;
using FishNet.Managing.Client;
//using FishNet.Managing.Scened;
using FishNet.Object;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    public TextMeshProUGUI win_text;
    public TextMeshProUGUI lose_text;
    public TextMeshProUGUI restart_text;
    public Button restart_button;
    public Button disconnect_button;
    bool restart_pressed = false;
    private void Start()
    {
        Game.game_over = this;

        //restart_button.onClick.AddListener(() => Restart());
        disconnect_button.onClick.AddListener(() => DisconnectClient());
        restart_button.onClick.AddListener(() => RestartButton());
    }

    private void Update()
    {
        RestartInput();
    }
    
    void RestartInput()
    {
        if (restart_text.gameObject.activeSelf && Input.GetButtonDown("Fire1") 
            && restart_pressed == false && GameData.is_server == false) RestartButton(); 
    }

    public void GameOverUIOn(bool won, bool is_host)
    {
        GameData.is_game_over = true;
        //GameData.are_hints_on = false; we need to showcase the hints for the other role too
        if (Game.Instance.rt_tutorial != null) Game.Instance.rt_tutorial.WipeTutorial(); // resetting the tutorial
        // disconnect button
        //disconnect_button.gameObject.SetActive(true);         should always be somewhere and available

        // win lose text
        if (won) win_text.gameObject.SetActive(true);
        else lose_text.gameObject.SetActive(true);

        // Removing current hint
        Game.Instance.rt_tutorial.overlay.SetActive(false);

        // displaying restart text
        restart_text.gameObject.SetActive(true);
        string restart_input_text = (GameData.is_gamepad_used) ? "Press <color=yellow>A</color> to restart the game" : "Click to restart the game";
        restart_text.text = (GameData.is_server) ? "Waiting for the host to restart" : restart_input_text;
    }

    public void DisconnectClient() // when the host disconnects the clients should also disconnect
    {
        if (Game.Instance.network_manager != null)
        {
            
            // Stop the client connection if this instance is a client.
            if (Game.Instance.network_manager.ClientManager.Started)
            {
                Debug.Log("Stopping client connection...");
                Game.Instance.network_manager.ClientManager.StopConnection();
            }

            // If this instance is a server, stop the server connection.
            if (Game.Instance.network_manager.ServerManager.Started)
            {
                Debug.Log("Stopping server connection...");

                // Despawn all spawned objects.
                List<NetworkObject> spawnedObjects = new List<NetworkObject>(Game.Instance.network_manager.ServerManager.Objects.Spawned.Values);
                foreach (NetworkObject obj in spawnedObjects)
                {
                    if (obj.IsSpawned) // Ensure the object is still spawned before despawning.
                    {
                        obj.Despawn();
                    }
                }

                // Stop the server.
                Game.Instance.network_manager.ServerManager.StopConnection(true);
            }
        }
        else
        {
            Debug.LogWarning("NetworkManager not found! Unable to disconnect.");
        }

        // Reset scene state by loading MainMenu.
        Debug.Log("Loading MainMenu...");

        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);

        GameData.is_game_over = false;
        //GameData.is_looping = false;

        Debug.Log("<color=blue>RESTART</color> Finished Disconnecting");
        //Game.Instance.network_manager.SceneManager.LoadConnectionScenes(Game.Instance.player.LocalConnection, new SceneLoadData("MainMenu"));
    }

    public void RestartButton()
    {
        Game.Instance.PlayerReadyToRestartServerRPC(false);

        restart_text.text = "Waiting for the other player";
        restart_pressed = true;

        Debug.Log("<color=blue>RESTART</color> Restart button pressed");
    }

    public void Restart()
    {
        Debug.Log("<color=blue>RESTART</color> Restart activated");
        GameData.is_looping = true;
        GameData.is_restarting = true;

        DisconnectClient();
    }
}



/*
 * if (Game.Instance.network_manager != null)
        {
            // both client and server can shut down the server upon pressing disconnect
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
            Game.Instance.network_manager.ClientManager.StopConnection();

            // Despawn all network objects.
            foreach (NetworkObject obj in Game.Instance.network_manager.ServerManager.Spawned)
            {
                obj.Despawn();
            }

            Game.Instance.network_manager.ServerManager.StopConnection(true);
            
        }
        else Debug.Log("Network manager was not found");
 */