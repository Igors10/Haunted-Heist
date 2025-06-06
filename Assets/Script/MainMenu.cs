using FishNet.Managing;
using FishNet.Transporting.Tugboat;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
//using FishNet.Managing.Scened;

public class MainMenu : MonoBehaviour
{
    NetworkManager network;
    Tugboat tugboat;
    [SerializeField] TMP_InputField input_field;
    [SerializeField] TMP_Text server_button_text;
    [SerializeField] Color server_text_color;
    [SerializeField] GameObject tutorial_window;
    [SerializeField] GameObject credits_window;
    [SerializeField] GameObject loading_screen;
    [SerializeField] GameObject[] miasma;
    [SerializeField] GameObject[] miasma_border;
    [SerializeField] float miasma_speed;
    [SerializeField] float logo_acceler;
    [SerializeField] bool is_logo_moving;
    float logo_speed = 0;
    [SerializeField] GameObject logo;

    public Button nextButton;
    public GameObject helpPanel;

    bool server_created = false;

    private void Start()
    {
        network = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        tugboat = network.gameObject.GetComponent<Tugboat>();

        if (GameData.is_looping)
        {
            loading_screen.SetActive(true);
            StartCoroutine(Restart(1f, GameData.is_server));
        }
    }

    public void OpenTutorial()
    {
        bool is_tutorial_open = (tutorial_window.activeSelf);

        tutorial_window.SetActive(!is_tutorial_open);
        tutorial_window.GetComponent<Tutorial>().EnableTutorial(!is_tutorial_open);

        EventSystem.current.SetSelectedGameObject(null); // Clear first
        EventSystem.current.SetSelectedGameObject(nextButton.gameObject);

        // closing credits in case they are open
        credits_window.SetActive(false);
    }

    public void RealTutorial()
    {
        // closing credits in case they are open
        credits_window.SetActive(false);

        //creating a dummy server for the functionality
        ServerCreate();
        network.ClientManager.StartConnection();

        //reset the tutorial stage in static script
        TutorialProgress.part = 1;

        //load the choice scene for the tutorial level
        SceneManager.LoadScene("Choose_Tutorial", LoadSceneMode.Single);
    }
    public void OpenCredits()
    {
        bool are_credits_open = (credits_window.activeSelf);
        credits_window.SetActive(!are_credits_open);

        // closing tutorial in case it is open
        tutorial_window.SetActive(false);
    }

    public void ServerCreate()
    {
        if (server_created) return;
        Debug.Log("Server button works");
        network.ServerManager.StartConnection();

        GameData.is_server = true;
        server_created = true;

        if (server_created)
        {
            server_button_text.color = server_text_color;
            server_button_text.text = "Server created";
        }
    }

    public void ClientJoin()
    {
        ChooseIP(); // get the ip from the Input Field

        Debug.Log("Client Button works");
        network.ClientManager.StartConnection();

        // SceneLoadData sld = new SceneLoadData("Lvl_Tilemap");
        // InstanceFinder.SceneManager.LoadGlobalScenes(sld);
        SceneManager.LoadScene("Lvl_Tilemap", LoadSceneMode.Single);
    }

    public void ChooseIP()
    {
        if (input_field.text == "")
        {
            tugboat.SetClientAddress("localhost");
            GameData.current_ip = "localhost"; // saving the ip gamedata
        }
        else
        {
            tugboat.SetClientAddress(input_field.text);
            GameData.current_ip = input_field.text; // saving the ip gamedata
        }

        
        Debug.Log(tugboat.GetClientAddress());
    }

    void Update()
    {
        // "Fast dial" ip adresses 
        if (Input.GetKeyDown(KeyCode.I)) // Igor's
        {
            input_field.text = "192.168.195.190";
        }
        else if (Input.GetKeyDown(KeyCode.J)) // Joel's
        {
            input_field.text = "192.168.195.36";
        }

        //Turning logo on and off
        if (Input.GetKeyDown(KeyCode.L))
        {
            logo.SetActive(!logo.activeSelf);
        }

        CheckForBackToMainMenu();
    }

    private void FixedUpdate()
    {
        MiasmaUpdate();
        LogoUpdate();
    }

    void LogoUpdate() // Logo moving up and down
    {
        if (!is_logo_moving) return;

        logo.transform.Translate(0f, logo_speed, 0f);
        logo_speed += logo_acceler;

        if (logo_speed > 0.01f || logo_speed < -0.01f) logo_acceler *= -1;
    }

    void MiasmaUpdate() // Background miasma moving
    {
        if (miasma[0] == null) return;

        for (int a = 0; a < miasma.Length; a++)
        {
            if (miasma[a].transform.position.x > miasma_border[1].transform.position.x) miasma[a].transform.position = miasma_border[0].transform.position;
            miasma[a].transform.Translate(miasma_speed, 0, 0);
            
        }
    }

    public void LoadPlayScene()
    {
        SceneManager.LoadScene("Play");
    }

    public void LoadMainMenuScene()
    {
        SceneManager.LoadScene("MainMenu");
    }


    // If on Play or Tutorial scene, and "B" or "Circle" is pressed on controller, go back to main menu
    public void CheckForBackToMainMenu()
    {
        if (Input.GetKeyDown(KeyCode.JoystickButton2))
        {
            Debug.Log("Back to main menu");
            SceneManager.LoadScene("MainMenu");
        }
    }

    public void LoadHelpPanel()
    {
        helpPanel.SetActive(!helpPanel.activeSelf);
    }


    //  ***** GGC restarting logic *****

    public IEnumerator Restart(float starting_delay, bool is_server)
    {
        GameData.is_restarting = false;

        Debug.Log("<color=blue>RESTART</color> Reconnection from main menu set on delay");
        yield return new WaitForSeconds(starting_delay);

        Debug.Log("<color=blue>RESTART</color> Reconnection from main menu begins");
        if (is_server)
        {
            Debug.Log("<color=blue>RESTART</color> creating server again");
            ServerCreate();
        }
        else
        {
            Debug.Log("<color=blue>RESTART</color> this one is not the server, wait for a bit");
            yield return new WaitForSeconds(0.2f); // host needs to join first
        }

        // Starting the client
        tugboat.SetClientAddress(GameData.current_ip);
        Debug.Log("<color=blue>RESTART</color> join as client with this ip -> " + GameData.current_ip);
        network.ClientManager.StartConnection();
        SceneManager.LoadScene("Lvl_Tilemap", LoadSceneMode.Single);

    }
}
