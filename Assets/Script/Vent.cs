using FishNet.Object;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


// TO FIX: make it go to the direction it is pointing at

public class Vent : NetworkBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] Vent[] connected_vents;
    Button[] vent_buttons = new Button[3];
    [SerializeField] Color closed_color;
    [SerializeField] Color blocked_color;
    [SerializeField] float vent_moving_speed;
    Color open_color;
    [SerializeField] SpriteRenderer sprite;
    [HideInInspector] public bool blocked;
    GameObject[] key_UI = new GameObject[2];
    VentArrows ventUI;
    bool open = false;
    [SerializeField] Sprite[] vent_sprites;
    [SerializeField] GameObject open_aura;

    void Awake()
    {
        // getting all the vent ui buttons and disabling them
        GameObject[] vent_button_gameObjects = GameObject.FindGameObjectsWithTag("VentButton");
        for (int i = 0; i < vent_button_gameObjects.Length; i++)
        {
            vent_buttons[i] = vent_button_gameObjects[i].GetComponent<Button>();
        }

        key_UI[0] = GameObject.Find("QKeyText");
        key_UI[1] = GameObject.Find("EKeyText");

        ventUI = GameObject.Find("NewVentUI").GetComponent<VentArrows>();
    }

    private void Start()
    {
        // deactivating buttons by default
        if (vent_buttons[0].gameObject.activeSelf == false)
        {
            for (int i = 0; i < vent_buttons.Length; i++)
            {
                vent_buttons[i].gameObject.SetActive(false);
            }
        }
    }

    public void OpenVent(bool is_open)
    {
        //if (blocked) return;
        if (blocked) is_open = false;
        if (CheckToBlock()) BlockVentServerRpc();

        open = is_open;

        // Enabling/disabling arrows
        bool left_arrow_active = (is_open && !connected_vents[0].blocked) ? true : false;
        bool right_arrow_active = (is_open && !connected_vents[1].blocked) ? true : false;
        ventUI.left_arrow.SetActive(left_arrow_active);
        ventUI.right_arrow.SetActive(right_arrow_active);

        ventUI.transform.position = transform.position;
        open_aura.SetActive(is_open);

        Debug.Log("VENT Opened:" + open);

        /*            OLD CODE FOR DYNAMIC BUTTON ALLOCATION
        for (int a = 0; a < connected_vents.Length; a++)
        {
            if (a >= vent_buttons.Length) continue;

            vent_buttons[a].gameObject.SetActive(is_open);

            if (!is_open) continue; // we dont need to go further if we simply close vents

            // if the target vent is blocked there's no reason to make an arrow for it
            else if (connected_vents[a].blocked)
            {
                vent_buttons[a].gameObject.SetActive(false);
                continue;
            }

            // setting positions and rotations of the buttons
            vent_buttons[a].transform.position = transform.position;

            Vector3 vent_direction = (connected_vents[a].transform.position - transform.position).normalized;
            vent_buttons[a].transform.rotation = Quaternion.FromToRotation(Vector3.up, vent_direction);

            vent_buttons[a].transform.position += vent_buttons[a].transform.up * 1.5f;
            key_UI[a].transform.position = vent_buttons[a].transform.position;


            // Adding functionality to buttons
            vent_buttons[a].onClick.RemoveAllListeners();
            Vent target_vent = connected_vents[a];
            vent_buttons[a].onClick.AddListener(() => StartCoroutine(MoveToVent(target_vent))); 
        }*/
    }

    bool CheckToBlock()
    {
        for (int a = 0; a < connected_vents.Length; a++)
        {
            if (connected_vents[a].blocked == false) return false;
        }

        return true;
    }

    void Update()
    {
        VentsInput();
    }

    void VentsInput()
    {
        if (!open || blocked) return;
        if (Input.GetButtonUp("VentLeft") && connected_vents[0] != null) StartCoroutine(MoveToVent(connected_vents[0]));
        else if (Input.GetButtonUp("VentRight") && connected_vents[1] != null) StartCoroutine(MoveToVent(connected_vents[1]));
    }
    IEnumerator MoveToVent(Vent vent_to_move_to)
    {
        // Darkness disappers when you use vents 

        OpenVent(false); 

        //SFX
        AudioManager.instance.PlaySFX("Vent");

        // Moving the character to the vent
        Vector3[] moving_positions = new Vector3[3];

        Debug.Log("VENTS: Moving_starts");

        // Finding position that the camera will move through simulating robbers vent movement =========================
        if (Mathf.Abs(transform.position.x - vent_to_move_to.transform.position.x) 
            > Mathf.Abs(transform.position.y - vent_to_move_to.transform.position.y))
        {
            float first_position_x = Random.Range(1f, transform.position.x - vent_to_move_to.transform.position.x);
            moving_positions[0] = new Vector3(first_position_x, transform.position.y, transform.position.z);
            moving_positions[1] = new Vector3(moving_positions[0].x, vent_to_move_to.transform.position.y, transform.position.z);
        }
        else
        {
            float first_position_y = Random.Range(1f, transform.position.y - vent_to_move_to.transform.position.y);
            moving_positions[0] = new Vector3(transform.position.x, first_position_y, transform.position.z);
            moving_positions[1] = new Vector3(vent_to_move_to.transform.position.x, moving_positions[0].y, transform.position.z);
        }

        moving_positions[2] = vent_to_move_to.transform.position;

        // ==============================================================================================================

        //Game.Instance.robber.Value.SetActive(false); // Deactivating the robber so that its invisible and theres no input
        Game.Instance.robber.Value.GetComponent<RobberScript>().EnableServerRpc(false);
        Game.Instance.robber.Value.GetComponent<RobberScript>().flashlight.SetActive(false);

        // Moving the robber through 
        int current_target_position = 0;

        while (Vector3.Distance(Game.Instance.robber.Value.transform.position, moving_positions[2]) > 0.1f)
        {
            Vector3 robber_position = Game.Instance.robber.Value.transform.position;
            if (Vector3.Distance(robber_position, moving_positions[current_target_position]) < 0.1f)
            {
                //SFX
                AudioManager.instance.PlaySFX("VentAmbience");
                current_target_position++;
                Debug.Log("VENT: switched the position");
            }

            Game.Instance.robber.Value.transform.position = Vector3.MoveTowards(robber_position, moving_positions[current_target_position], 0.1f);

            //Updating camera position;
            Game.Instance.robber.Value.GetComponent<Player>().main_camera.GetComponent<CameraBehavior>().to_follow = robber_position;

            yield return new WaitForSeconds(vent_moving_speed);
        }

        //SFX
        AudioManager.instance.PlaySFX("Vent");

        //Game.Instance.robber.Value.SetActive(true); // Reactivating the robber
        
        // block the vent you just moved to
        vent_to_move_to.BlockVentServerRpc();

        // we need to block this vent if both vents it goes to were blocked
        if (CheckToBlock()) BlockVentServerRpc();

        Game.Instance.robber.Value.GetComponent<RobberScript>().EnableServerRpc(true);
        Debug.Log("VENT: finished_moving");

        // to do: the above can happen when you interact with another vent so make sure to account for that
    }

    [ServerRpc(RequireOwnership = false)]
    void BlockVentServerRpc()
    {
        BlockVentObserverRpc();
    }

    [ObserversRpc]
    public void BlockVentObserverRpc()
    {
        blocked = true;
        //this.gameObject.tag = "Untagged";
        sprite.sprite = vent_sprites[1];
    }
}
