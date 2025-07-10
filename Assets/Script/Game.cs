using FishNet.Managing;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
public enum character
{
    ROBBER,
    GHOST,
}
public class Game : NetworkBehaviour
{
    public static Game Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }


        Instance = this;

        // activating loading screen
        if (loading_screen != null) loading_screen.SetActive(true);

        //connection fails
        network_manager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();

    }
    //public static GameObject robber;
    public readonly SyncVar<GameObject> robber = new SyncVar<GameObject>();
    public readonly SyncVar<GameObject> ghost = new SyncVar<GameObject>();
    //public static GameObject ghost;
    public Player player;
    public static Level level;
    public static GameOverUI game_over;
    public NetworkManager network_manager;
    public RestartScript restart_manager;
    public GameObject loading_screen;
    public ItemLottery item_lottery;
    public RuntimeTutorialManager rt_tutorial;
    public TimerUI timer;
    public GameObject opponent_text;

    public bool is_robber_connected = false;
    public bool is_ghost_connected = false;

    public int players_ready_to_restart = 0;

    void Start()
    {
        // Turning off the music for server
        if (!IsServer) AudioManager.instance.musicSource.gameObject.SetActive(false);
    }

    public bool IsRobber()
    {
        bool is_robber = (player == robber.Value) ? true : false;

        return is_robber;
    }

    public bool IsGhost()
    {
        bool is_ghost = (player == ghost.Value) ? true : false;
        return is_ghost;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton7))
        {
            is_robber_connected = true;
            is_ghost_connected = true;
        }
        if (Input.GetKeyDown(KeyCode.O)) // shortcut key for making stopping the client be considered a server in gamedata
        {
            GameData.is_server = false;
        }
        if (Input.GetKeyDown(KeyCode.R) && GameData.is_restarting == false) 
        {
            PlayerReadyToRestartServerRPC(true);
        }
        /*
        if (Input.GetButtonDown("Restart") && GameData.is_restarting == false && GameData.is_game_over)
        {
            PlayerReadyToRestartServerRPC(true);
        }*/

        // Debug to see if robber and ghost variables are in ===================

        if (robber.Value == null)
        {
            Debug.Log("robber is not assigned");
        }
        else Debug.Log("robber is assigned");

        if (ghost.Value == null)
        {
            Debug.Log("ghost is not assigned");
        }
        else Debug.Log("ghost is assigned");

        // ======================================================================
    }

    // Restarting GGC logic

    [ServerRpc(RequireOwnership = false)]

    public void PlayerReadyToRestartServerRPC(bool force_restart)
    {
        bool should_restart = (players_ready_to_restart + 1 >= 0) || force_restart ? true : false;

        PlayerReadyToRestartObserverRPC(should_restart);
    }

    [ObserversRpc]
    public void PlayerReadyToRestartObserverRPC(bool should_restart)
    {
        players_ready_to_restart++;

        //game_over.Restart(); << uncomment this for old restart function
        restart_manager.Restart();
    }

}
