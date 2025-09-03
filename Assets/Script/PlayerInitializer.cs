using FishNet;
using FishNet.Connection;
using FishNet.Managing.Scened;
using FishNet.Managing;
using FishNet.Object;
using UnityEngine;

public class PlayerInitializer : MonoBehaviour
{
    NetworkManager network_manager;
    [SerializeField] GameObject character_select_prefab;

    private void Start()
    {
        network_manager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();

        if (network_manager != null)
        {
            network_manager.SceneManager.OnClientLoadedStartScenes += OnClientLoadedStartScenes;
            if (GameData.is_looping) RespawnExistingPlayers();
        }
        else Debug.Log("Network manager not assigned");
    }

    private void OnClientLoadedStartScenes(NetworkConnection conn, bool asServer) 
    {
        // Ensure this runs only on the server.
        if (!asServer) return;

        SpawnCharacterSelect(conn);
    }

    void SpawnCharacterSelect(NetworkConnection conn)
    {
        GameObject character_select = Instantiate(character_select_prefab);
        network_manager.ServerManager.Spawn(character_select, conn);

        NetworkObject network_obj = character_select.GetComponent<NetworkObject>();
        network_manager.SceneManager.AddOwnerToDefaultScene(network_obj);

        //OnSpawned?.Invoke(network_obj);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) RespawnExistingPlayers();
    }

    private void RespawnExistingPlayers()
    {
        foreach (NetworkConnection conn in InstanceFinder.ServerManager.Clients.Values)
        {
            if (conn.IsActive)
            {
                SpawnCharacterSelect(conn);
            }
        }
    }

    private void OnDestroy()
    {
        if (network_manager != null)
        {
            network_manager.SceneManager.OnClientLoadedStartScenes -= OnClientLoadedStartScenes;
        }
    }
}


