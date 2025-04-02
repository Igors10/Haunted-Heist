using FishNet.Object;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Disconnect : MonoBehaviour
{
    public void DisconnectClient()
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
    }
}
