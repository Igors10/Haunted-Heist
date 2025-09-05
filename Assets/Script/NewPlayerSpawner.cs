using UnityEngine;
using FishNet.Object;
using FishNet;
using FishNet.Connection;
using FishNet.Managing;
using System.Collections;

public class NewPlayerSpawner : NetworkBehaviour
{
    [SerializeField] GameObject robber_prefab;
    [SerializeField] GameObject ghost_prefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void OnStartClient()
    {
        base.OnStartClient();

        if (IsOwner) StartCoroutine(RequestSpawnPlayer());
    }

    IEnumerator RequestSpawnPlayer()
    {
        yield return new WaitForSeconds(0.1f);

        SpawnPlayer(GameData.character_selected);
    }

    Vector3 FindSpawnPoint(string spawnpoint_name)
    {
        GameObject spawnPoint = GameObject.Find(spawnpoint_name);
        return (spawnPoint != null) ? spawnPoint.transform.position : Vector3.zero;
    }

    [ServerRpc(RequireOwnership = true)]
    void SpawnPlayer(character character_to_spawn, NetworkConnection player_connection = null)
    {
        Vector3 spawn_pos = new Vector3();
        GameObject player_prefab;

        player_prefab = (character_to_spawn == character.ROBBER) ? robber_prefab : ghost_prefab;
        spawn_pos = (character_to_spawn == character.ROBBER) ? FindSpawnPoint("RobberSpawnpoint") : FindSpawnPoint("GhostSpawnpoint");

        GameObject spawned_player = Instantiate(player_prefab, spawn_pos, Quaternion.identity);
        InstanceFinder.ServerManager.Spawn(spawned_player, player_connection);

        NetworkObject network_obj = spawned_player.GetComponent<NetworkObject>();
        InstanceFinder.SceneManager.AddOwnerToDefaultScene(network_obj);

        if (character_to_spawn == character.ROBBER) Game.Instance.robber.Value = spawned_player;
        if (character_to_spawn == character.GHOST) Game.Instance.ghost.Value = spawned_player;


        // remove this spawner everywhere
        base.NetworkObject.Despawn();
    }


}
