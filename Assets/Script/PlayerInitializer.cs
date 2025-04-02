using FishNet;
using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

public class PlayerInitializer : MonoBehaviour
{
    [SerializeField] GameObject character_select_prefab;

    private void Start()
    {
        if (Game.Instance.network_manager != null)
        {
            Game.Instance.network_manager.SceneManager.OnClientLoadedStartScenes += OnClientLoadedStartScenes;
            if (GameData.is_game_over == true) RespawnExistingPlayers();
        }
        else Debug.Log("Network manager not assigned");
    }

    private void OnClientLoadedStartScenes(NetworkConnection conn, bool asServer) // Made with the help from ChatGPT
    {
        // Ensure this runs only on the server.
        if (!asServer) return;

        SpawnCharacterSelect(conn);
    }

    void SpawnCharacterSelect(NetworkConnection conn)
    {
        GameObject character_select = Instantiate(character_select_prefab);
        Game.Instance.network_manager.ServerManager.Spawn(character_select, conn);

        NetworkObject network_obj = character_select.GetComponent<NetworkObject>();
        Game.Instance.network_manager.SceneManager.AddOwnerToDefaultScene(network_obj);

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
        if (Game.Instance.network_manager != null)
        {
            Game.Instance.network_manager.SceneManager.OnClientLoadedStartScenes -= OnClientLoadedStartScenes;
        }
    }
}
