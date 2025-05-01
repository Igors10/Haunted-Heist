using FishNet.Object;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class RobberScript : NetworkBehaviour
{
    [HideInInspector]
    public Player player;
    public GameObject flashlight;
    public GameObject natural_light;
    [HideInInspector] public bool items_collected;
    public GameObject item_pick_up_aura;
    RobberUI robberUI;
    public ArrowPointer[] exit_pointer;

    // Shake variables
    [SerializeField] public float radar_range;
    [SerializeField] public float white_noise_range;
    [SerializeField] public float shake_intensity;
    [SerializeField] public float white_noise_volume;
    GameObject level_light;

    // Beign caught variables 
    [SerializeField] float jumpscare_duration;
    bool is_caught = false;
    [SerializeField] GameObject jumpscare;

    // Item radar
    public float time_before_item_help;
    float item_radar_timer;
    public ArrowPointer item_arrow;
    public bool custom_arrow_pointer = false;
    public Vector3 custom_arrow_pointer_target;

    // *** How to use custom arrow pointer ***

    // set ArrowPointer.gameObject to be active
    // set custom_arrow_pointer to true
    // set custom_arrow_poinetr_target to transform.position of desired target
    // set custom_arrow_pointer back to false when you need the arrow to point at items again

    // also you can set time_before_item_help to 0 so that item arrow appears right after you have collected an item

    // Night vision jump scare variables
    bool jumpscared;
    float jumpscare_cooldown = 20f;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (IsOwner)
        {
            robberUI = GameObject.Find("RobberUI").GetComponent<RobberUI>();
            if (robberUI == null) Debug.Log("Couldnt find ghost UI");
            else robberUI.EnableUI();
        }
    }
    private void Start()
    {
        item_radar_timer = time_before_item_help;
        level_light = GameObject.Find("Candels");
        exit_pointer[0].target = GameObject.Find("FrontDoor_Close").transform.position;
        exit_pointer[1].target = GameObject.Find("BackDoor_Close").transform.position;
    }
    public void Flashlight(bool is_on)
    {
        if (player.is_special_vision_on) return;
        flashlight.SetActive(is_on);
        SyncFlashlightServerRpc(is_on);
        if (IsOwner) player.narrow_dark_filter.SetActive(!is_on);

        if (is_on)
        {
            //SFX
            AudioManager.instance.PlaySFX("Flashlight");

            Debug.Log("FlashlightOn");
        }
        else
        {
            //SFX
            //AudioManager.instance.PlaySFX("Flashlight");

            Debug.Log("FlashlightOff");
        }
    }

    [ServerRpc]
    public void EnableServerRpc(bool is_enabled)
    {
        EnableObseverRpc(is_enabled);
    }

    [ObserversRpc]
    public void EnableObseverRpc(bool is_enabled)
    {
        // prevent the bug when robber has light on when venting, and also has night vision on when venting
        SyncFlashlightServerRpc(false);
        NightVision(false);

        item_pick_up_aura.SetActive(is_enabled);
        GetComponent<SpriteRenderer>().enabled = is_enabled;
        GetComponent<CapsuleCollider2D>().enabled = is_enabled;
        if (IsOwner) GetComponent<InputController>().enabled = is_enabled;
        else natural_light.SetActive(is_enabled);
    }
    public void NightVision(bool is_on)
    {
        // Energy bar UI code (if its too low it wont work)
        if (!robberUI.UseEnergy(is_on)) is_on = false;

        player.main_camera.GetComponent<CameraBehavior>().SpecialVision(is_on, true);
        player.is_special_vision_on = is_on;

        // Disabling robbers natural light, so it doesnt look weird with night vision
        natural_light.SetActive(!is_on);

        if (IsOwner) player.narrow_dark_filter.SetActive(!is_on);

        if (is_on) NightVisionOn();
    }

    void NightVisionOn()
    {
        // Turn off the flashlight in case its on
        SyncFlashlightObserversRpc(false);

        // Play the jumpscare sound if the ghost is close
        if (Game.Instance.ghost.Value != null && jumpscared == false &&
            Vector2.Distance(Game.Instance.ghost.Value.transform.position, transform.position) < 9f)
        {
            StartCoroutine(Jumpscare());
        }
    }

    IEnumerator Jumpscare()
    {
        // Play the jumpscare sound
        jumpscared = true;
        AudioManager.instance.PlaySFX("JumpscareLight");

        // Sound will not repeat in the next set amount of seconds
        yield return new WaitForSeconds(jumpscare_cooldown);

        jumpscared = false;
    }

    private void Update()
    {
        GhostRadar();
        ItemRadar();
    }

    public void ResetItemRadar()
    {
        item_radar_timer = time_before_item_help;
        item_arrow.gameObject.SetActive(false);
    }
    void ItemRadar()
    {
        if (item_arrow.gameObject.activeSelf || IsOwner == false || items_collected) return;

        item_radar_timer -= Time.deltaTime;

        if (item_radar_timer < 0)
        {
            ActivateItemRadar();
        }
    }

    public void ActivateItemRadar()
    {
        // activate the arrow
        item_arrow.gameObject.SetActive(true);
        item_arrow.target = (custom_arrow_pointer) ? custom_arrow_pointer_target : Game.Instance.item_lottery.GetRandomItem().transform.position;
    }

    void GhostRadar()
    {
        if (Game.Instance.ghost.Value == null || !IsOwner || player == null) return;

        float distance_to_ghost = Vector2.Distance(Game.Instance.ghost.Value.transform.position, transform.position);

        // Shaking effect
        Vector2 shake_vector = ShakeEffect(distance_to_ghost);
        natural_light.transform.localPosition = new Vector3(shake_vector.x, shake_vector.y, natural_light.transform.position.z);
        flashlight.transform.localPosition = new Vector3(shake_vector.x, shake_vector.y, flashlight.transform.position.z);

        // White noise audio effect
        AudioSource white_noise = GetComponent<AudioSource>();
        if (distance_to_ghost > white_noise_range)
            white_noise.volume = 0;
        else
            white_noise.volume = (white_noise_range - distance_to_ghost) * white_noise_volume / white_noise_range;

        // Controller rumble
        HandleVibration(distance_to_ghost);
    }

    void HandleVibration(float distance)
    {
        if (Gamepad.current == null)
            return;

        // No rumble outside of white noise range
        if (distance > white_noise_range)
        {
            Gamepad.current.SetMotorSpeeds(0f, 0f);
            return;
        }

        // Intensity
        float proximity = (white_noise_range - distance) / white_noise_range;
        float intensityScale = 0.1f; // Overall

        float lowFreq = proximity * 0.3f * intensityScale;
        float highFreq = proximity * 0.6f * intensityScale;

        Gamepad.current.SetMotorSpeeds(lowFreq, highFreq);
    }



    Vector2 ShakeEffect(float distance_to_ghost)
    {
        // No shake at all if ghost is too far away
        if (distance_to_ghost > radar_range) return Vector2.zero;

        float distance_modifier = (radar_range - distance_to_ghost);
        float magnitude = (distance_modifier * shake_intensity / 50f);

        float x = Random.Range(-1f, 1f) * magnitude;
        float y = Random.Range(-1f, 1f) * magnitude;

        return new Vector2(x, y);

    }

    private void OnDisable()
    {
        if (Gamepad.current != null)
            Gamepad.current.SetMotorSpeeds(0f, 0f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Ghost collision
        if (IsServer && collision.gameObject.tag == "Ghost" && collision.transform.parent.GetComponent<GhostScript>().is_dashing
            && is_caught == false)
        {
            SyncCatchRobberServerRpc();
        }

        // Mansion escape
        if (collision.gameObject.tag == "EscapeZone" && items_collected)
        {
            player.won = true;
            player.GameOverServerRpc(true);
            if (Game.Instance.ghost.Value != null) Game.Instance.ghost.Value.GetComponent<Player>().GameOverServerRpc(false);
        }

        // Vents
        if (collision.gameObject.tag == "Vent")
        {
            Debug.Log("VENTS: vent collision");
            // Experimental code where you press Q and E to use vents

            Vent vent = collision.GetComponent<Vent>();
            if (vent == null)
            {
                Debug.LogError("VENTS: Vent component is missing from the collided object!");
            }
            else // if (Vector2.Distance(transform.position, vent.gameObject.transform.position) < vent.use_distance)
            {
                vent.OpenVent(true);
            }
        }
    }

    // Synchronizing using the flashlight ===========================================

    [ServerRpc(RequireOwnership = false)]
    void SyncFlashlightServerRpc(bool is_on)
    {
        //flashlight.SetActive(is_on); this is unnecessary
        SyncFlashlightObserversRpc(is_on);
    }

    [ObserversRpc]
    void SyncFlashlightObserversRpc(bool is_on)
    {
        flashlight.SetActive(is_on);
        if (Game.Instance.ghost.Value != null) Game.Instance.ghost.Value.GetComponent<Player>().Indication(is_on);
    }

    // ===============================================================================

    [ServerRpc(RequireOwnership = false)]
    void SyncCatchRobberServerRpc()
    {
        SyncCatchRobberObserverRpc();
    }

    [ObserversRpc]
    void SyncCatchRobberObserverRpc()
    {
        CaughthRobber();

    }
    void CaughthRobber()
    {
        Debug.Log("CATCHING: I was caught (Observer)");
        //if (IsOwner) GameObject.Find("Ghost(Clone)").GetComponent<GhostScript>().SyncCatchServerRpc();
        Game.Instance.ghost.Value.GetComponent<GhostScript>().SyncCatchServerRpc();

        if (IsOwner) StartCoroutine(GetSpooked());
    }

    IEnumerator GetSpooked()
    {
        is_caught = true;
        float alpha = 1f;
        float current_jumpscare_duration = jumpscare_duration;
        SpriteRenderer sprite = jumpscare.GetComponent<SpriteRenderer>();
        jumpscare.SetActive(true);

        AudioManager.instance.PlaySFX("Damage");

        //HP blinking
        if (IsOwner) robberUI.hp.DecreaseHealth();

        // Check if there are lives left
        if (robberUI.hp.currentHealth < 1)
        {
            player.GameOverServerRpc(false);
            if (Game.Instance.ghost.Value != null) Game.Instance.ghost.Value.GetComponent<Player>().GameOverServerRpc(true);
        }

        while (current_jumpscare_duration > 0)
        {
            alpha = current_jumpscare_duration / jumpscare_duration;
            sprite.color = new Color(1, 1, 1, alpha);

            current_jumpscare_duration--;
            yield return new WaitForSeconds(0.1f);
        }

        jumpscare.SetActive(false);
        is_caught = false;
    }
}
