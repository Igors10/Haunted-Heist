using FishNet.Object;
using FishNet.Connection;
using UnityEngine;
using FishNet.Managing;
using UnityEngine.UI;
using Unity.VisualScripting;
using System.Collections;
using TMPro;

public class Spawner : NetworkBehaviour
{
    [SerializeField] GameObject robber_prefab;
    [SerializeField] GameObject ghost_prefab;
    [SerializeField] character starting_character;
    GameObject new_player;

    // character select
    [SerializeField] Button ghost_button;
    [SerializeField] Button robber_button;
    [SerializeField] TextMeshProUGUI ghost_taken_text;
    [SerializeField] TextMeshProUGUI robber_taken_text;
    bool choice_made = false;


    //Count down
    [SerializeField] int countdown;
    [SerializeField] float time_between_countdown;
    [SerializeField] TextMeshProUGUI countdown_text;


    public override void OnStartClient()
    {
        base.OnStartClient();

        // Only allow the owner to see and interact with the character selection screen

        if (!base.IsOwner)
        {
            ghost_button.gameObject.SetActive(false);
            robber_button.gameObject.SetActive(false);
            //SelfDestruct();
            //this.gameObject.SetActive(false);
            return;
        }

        if (Game.Instance.loading_screen != null) Game.Instance.loading_screen.SetActive(false);

        Debug.Log("I- Spawner client star");

        ghost_button.onClick.AddListener(() => RequestSpawnGhost());
        robber_button.onClick.AddListener(() => RequestSpawnRobber());
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestSpawnGhost(NetworkConnection sender = null)
    {
        if (Game.Instance.is_ghost_connected || choice_made) return;

        SetChoiceObserverRpc(false);

        StartCoroutine(SpawnPlayer(ghost_prefab, FindSpawnPoint("GhostSpawnpoint"), sender, false));
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestSpawnRobber(NetworkConnection sender = null)
    {
        if (Game.Instance.is_robber_connected || choice_made) return;

        SetChoiceObserverRpc(true);
       
        StartCoroutine(SpawnPlayer(robber_prefab, FindSpawnPoint("RobberSpawnpoint"), sender, true));
    }

    [ObserversRpc]
    void SetChoiceObserverRpc(bool is_robber)
    {
        TextMeshProUGUI text = (is_robber) ? robber_taken_text : ghost_taken_text;
        text.gameObject.SetActive(true);

        if (IsOwner)
        {
            text.text = "You";
            choice_made = true;
        }
        else text.text = "Opponent";
    }
    private Vector3 FindSpawnPoint(string spawnpointName)
    {
        GameObject spawnPoint = GameObject.Find(spawnpointName);
        return (spawnPoint != null) ? spawnPoint.transform.position : Vector3.zero;
    }

    IEnumerator SpawnPlayer(GameObject player_to_spawn, Vector3 position, NetworkConnection owner, bool is_robber)
    {
        UpdatePlayerConnectionObserversRpc(is_robber); // telling the server that a client is connected as robber/ghost

        // Waiting for the other client to connect
        while (Game.Instance.is_robber_connected == false || Game.Instance.is_ghost_connected == false)
        {
            Debug.Log("Waiting for another player");
            yield return new WaitForSeconds(0.1f);
        }

        // Count down
        for (int a = countdown; a > 0; a--)
        {
            if (a > 0 && IsOwner) UpdateCountDownObserversRpc(a.ToString());

            yield return new WaitForSeconds(time_between_countdown);
        }

        // Spawning the character prefab, technically starting the game
        new_player = Instantiate(player_to_spawn, position, Quaternion.identity);
        ServerManager.Spawn(new_player, owner);

        // Assigning player and robber/ghost variables in the game script
        UpdatePlayerObserversRpc(is_robber);

        Debug.Log($"Spawned {(is_robber ? "Robber" : "Ghost")} for connection: {owner.ClientId}"); // verify connection

        SelfDestruct(); // Deactivating the character select screen
    }


    void SelfDestruct()
    {
        NetworkObject networkObject = GetComponent<NetworkObject>();
        networkObject.Despawn();
    }

    IEnumerator UpdateCountDown(string new_countdown)
    {
        countdown_text.text = new_countdown;

        
        float countdown_default_text_size = countdown_text.fontSize;
        float current_text_size = countdown_default_text_size;

        for (float a = 0; a <= time_between_countdown; a += time_between_countdown / 100)
        {
            //current_text_size *= 1.3f;
            
            countdown_text.fontSize = current_text_size + a * 20;
            yield return new WaitForSeconds(time_between_countdown / 100);
        }
        countdown_text.fontSize = current_text_size;
    }

    [ObserversRpc]
    void UpdateCountDownObserversRpc(string new_countdown)
    {
        StartCoroutine(UpdateCountDown(new_countdown));
    }
   
    [ObserversRpc]
    void UpdatePlayerConnectionObserversRpc(bool is_robber)
    {
        if (is_robber)
            Game.Instance.is_robber_connected = true;
        else
            Game.Instance.is_ghost_connected = true;

        //if (IsOwner) Game.Instance.player = new_player.GetComponent<Player>();
    }

    [ObserversRpc]
    void UpdatePlayerObserversRpc(bool is_robber)
    {
        if (is_robber)
            Game.Instance.robber.Value = new_player; 
        else
            Game.Instance.ghost.Value = new_player;

        //if (IsOwner) Game.Instance.player = new_player.GetComponent<Player>();
    }
}
