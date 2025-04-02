using FishNet.Managing;
using FishNet.Managing.Server;
using FishNet.Object;
using FishNet.Transporting.Tugboat;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
//using FishNet.Managing.Scened;
using FishNet;
using Unity.Mathematics;

public class MainMenu : MonoBehaviour
{
    NetworkManager network;
    Tugboat tugboat;
    [SerializeField] TMP_InputField input_field;
    [SerializeField] TMP_Text server_button_text;
    [SerializeField] Color server_text_color;
    [SerializeField] GameObject tutorial_window;
    [SerializeField] GameObject credits_window;
    bool server_created = false;

    private void Start()
    {
        network = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        tugboat = network.gameObject.GetComponent<Tugboat>();
    }

    public void OpenTutorial()
    {
        bool is_tutorial_open = (tutorial_window.activeSelf);

        tutorial_window.SetActive(!is_tutorial_open);
        tutorial_window.GetComponent<Tutorial>().EnableTutorial(!is_tutorial_open);

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
        }
        else
        {
            tugboat.SetClientAddress(input_field.text);
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
    }
}
