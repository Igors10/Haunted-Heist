using UnityEngine;
using FishNet.Object;
using UnityEngine.UIElements;
using FishNet.Connection;
using System.Collections;
using UnityEngine.SceneManagement;

public class ConnectionManager : MonoBehaviour 
{
    private void Start()
    {
        //StartCoroutine(CheckForConnection());
    }

    IEnumerator CheckForConnection() // whoever clicked play on the tile scene will be transported to main menu
    {
        yield return new WaitForSeconds(5.0f);

        if (Game.Instance.network_manager == null) SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        else
        {
            Debug.Log("Connection manager: network manager is valid");

            // create a character select for each client
        }
    }
}


/*
    [SerializeField] GameObject character_select;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    override public void OnStartServer()
    {
        base.OnStartServer();
        StartCoroutine(EnableCharacterSelectScreen());
        // this whole thing only works on servers side !!! 
    }

    // I wait for a little here, so that the scene is fully loaded before I proceed with spawning anything
    IEnumerator EnableCharacterSelectScreen()
    {
        yield return new WaitForSeconds(1.0f);

        Debug.Log("I- Trying to spawn a player");
        //GameObject new_player = Instantiate(character_select);
        //ServerManager.Spawn(new_player, LocalConnection);
        EnableCharacterSelectScreenServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void EnableCharacterSelectScreenServerRpc(NetworkConnection sender = null)
    {
        // Spawn the character selection screen for the specific client
        if (sender == null)
        {
            Debug.LogError("ServerRpc received no valid sender connection.");
            return;
        }

        GameObject new_player = Instantiate(character_select);
        ServerManager.Spawn(new_player, sender);

        Debug.Log($"Spawned character select screen for client: {sender.ClientId}");
    }*/