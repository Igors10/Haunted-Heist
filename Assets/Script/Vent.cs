using FishNet.Object;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


// TO FIX: script is being disabled when you start the game (because its network behaviour)

public class Vent : NetworkBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] Vent[] connected_vents;
    GameObject[] all_vents;
    Button[] vent_buttons = new Button[3];
    [SerializeField] Color closed_color;
    [SerializeField] Color blocked_color;
    [SerializeField] float vent_moving_speed;
    Color open_color;
    [SerializeField] SpriteRenderer sprite;
    [HideInInspector] public bool blocked;
    VentArrows ventUI;
    bool open = false;
    [SerializeField] Sprite[] vent_sprites;
    [SerializeField] GameObject open_aura;
    public float use_distance;
    [SerializeField] float full_block_timer;
    float current_block_timer = 0;
    [SerializeField] Image cooldown_overlay;
    [SerializeField] TextMeshProUGUI cooldown_text;

    public override void OnStartClient()
    {
        base.OnStartClient();
        this.enabled = true; // Re-enable the script when the client joins the scene.
    }
    void Awake()
    {
        // getting all the vent ui buttons and disabling them
        GameObject[] vent_button_gameObjects = GameObject.FindGameObjectsWithTag("VentButton");
        for (int i = 0; i < vent_button_gameObjects.Length; i++)
        {
            vent_buttons[i] = vent_button_gameObjects[i].GetComponent<Button>();
        }

        ventUI = GameObject.Find("NewVentUI").GetComponent<VentArrows>(); // Add this object with arrows into the scene
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

        all_vents = GameObject.FindGameObjectsWithTag("Vent");
    }

    public void OpenVent(bool is_open)
    {
        Debug.Log("VENTS: vent collision 222 ");
        //if (blocked) return;
        if (blocked) is_open = false;
        if (CheckToBlock()) BlockVentServerRpc();

        open = is_open;

        //  * Enabling/disabling arrows *
        // Setting the correct input icons
        bool is_lb_icon_on = (GameData.is_gamepad_used) ? true : false;
        ventUI.lb_icon.SetActive(is_lb_icon_on);
        bool is_rb_icon_on = (GameData.is_gamepad_used) ? true : false;
        ventUI.rb_icon.SetActive(is_rb_icon_on);

        bool left_arrow_active = (is_open && !connected_vents[0].blocked) ? true : false;
        bool right_arrow_active = ((connected_vents.Length > 1) && is_open && !connected_vents[1].blocked) ? true : false;
        ventUI.left_arrow.SetActive(left_arrow_active);
        ventUI.right_arrow.SetActive(right_arrow_active);

        ventUI.transform.position = transform.position;
        open_aura.SetActive(is_open);

        Debug.Log("VENT Opened:" + open);

        // Check for rt_tutorial
        Game.Instance.rt_tutorial.vent_near = is_open;
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

        // block all vents for some time
        BlockAllVents();

        // we need to block this vent if both vents it goes to were blocked
        //if (CheckToBlock()) BlockVentServerRpc();

        Game.Instance.robber.Value.GetComponent<RobberScript>().EnableServerRpc(true);
        Debug.Log("VENT: finished_moving");

        // to do: the above can happen when you interact with another vent so make sure to account for that
    }

    void BlockAllVents()
    {
        for (int a = 0; a < all_vents.Length; a++)
        {
            all_vents[a].GetComponent<Vent>().BlockVentServerRpc();
        }
    }

    public IEnumerator Block()
    {
        blocked = true;
        //this.gameObject.tag = "Untagged";
        sprite.sprite = vent_sprites[1];

        // Check for rt_tutorial
        Game.Instance.rt_tutorial.robber_vented = true;

        while (current_block_timer < full_block_timer) {
            current_block_timer++;

            // setting the timer text
            float time_to_showcase = full_block_timer - current_block_timer;
            cooldown_text.text = time_to_showcase.ToString();

            // setting the overlay look
            float cooldown_fillAmount = (full_block_timer - current_block_timer) / full_block_timer;
            cooldown_overlay.fillAmount = cooldown_fillAmount;

            yield return new WaitForSeconds(1f);
        }

        

        current_block_timer = 0;
        cooldown_overlay.fillAmount = 0;
        cooldown_text.text = "";
        sprite.sprite = vent_sprites[0];
        blocked = false;
    }

    [ServerRpc(RequireOwnership = false)]
    void BlockVentServerRpc()
    {
        BlockVentObserverRpc();
    }

    [ObserversRpc]
    public void BlockVentObserverRpc()
    {
        if (blocked == false) StartCoroutine(Block());
    }
}
