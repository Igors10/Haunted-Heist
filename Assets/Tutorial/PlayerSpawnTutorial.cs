using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;
using FishNet.Connection;
using FishNet.Object;
using System.Collections;
using FishNet;

public class PlayerSpawnTutorial : MonoBehaviour
{
    public GameObject character_prefab;
    public Transform characterSpawn;


    private void Start()
    {
        Debug.Log("FRANEK::Starting Corutione");
        StartCoroutine(Delay());
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(2f);

        Debug.Log("FRANEK::Starting checking foreach loop");
        foreach (NetworkConnection conn in InstanceFinder.ServerManager.Clients.Values)
        {
            Debug.Log("FRANEK::Spawning character");
            SpawnCharacter(conn);
        }

    }
    void SpawnCharacter(NetworkConnection conn)
    {
        Debug.Log("FRANEK::Spawn");

        GameObject character = Instantiate(character_prefab, characterSpawn.position, Quaternion.identity);
        Game.Instance.network_manager.ServerManager.Spawn(character, conn);

        NetworkObject network_obj = character.GetComponent<NetworkObject>();
        Game.Instance.network_manager.SceneManager.AddOwnerToDefaultScene(network_obj);

        if (character.TryGetComponent(out RobberScript robber))
            Game.Instance.robber.Value = character;
        else
            Game.Instance.ghost.Value = character;

        //OnSpawned?.Invoke(network_obj);
    }
}
