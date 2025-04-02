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
    public TextMeshProUGUI client_wait_text;
    public Button restart_button;
    public Button disconnect_button;
    private void Start()
    {
        Game.game_over = this;

        //restart_button.onClick.AddListener(() => Restart());
        disconnect_button.onClick.AddListener(() => DisconnectClient());
    }
    public void GameOverUIOn(bool won, bool is_host)
    {
        GameData.is_game_over = true;
        // disconnect button
        //disconnect_button.gameObject.SetActive(true);         should always be somewhere and available

        // win lose text
        if (won) win_text.gameObject.SetActive(true);
        else lose_text.gameObject.SetActive(true);

        // wait for host / restart
        if (is_host) restart_button.gameObject.SetActive(true);
        else client_wait_text.gameObject.SetActive(true);
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
        //Game.Instance.network_manager.SceneManager.LoadConnectionScenes(Game.Instance.player.LocalConnection, new SceneLoadData("MainMenu"));
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