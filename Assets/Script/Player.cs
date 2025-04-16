using FishNet.Object;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : NetworkBehaviour
{
    [HideInInspector]
    public bool frozen;
    [HideInInspector]
    public bool won;
    [HideInInspector]
    public bool is_special_vision_on;
    public float speed;
    [HideInInspector]
    public Camera main_camera;
    public GameObject narrow_dark_filter;
    public GameObject wide_dark_filter;
    public SpriteRenderer sprite;
    public SpriteRenderer aura_sprite; // temporary
    public Color color;

    // indication varialbes
    [SerializeField] GameObject indication;
    [SerializeField] SpriteRenderer indication_sprite;
    [SerializeField] float indication_hide_distance;

    // lives/collected souls variables (hardcoded 3 for now)
    [SerializeField] GameObject hp_bar;
    [SerializeField] SpriteRenderer[] lives = new SpriteRenderer[3];
    [SerializeField] float blinking_interval;
    [SerializeField] float blinking_duration;
    int current_hp = 3;
    [SerializeField] Color wasted_life_color;
    [HideInInspector] public bool is_blinking;

    // Text variables
    [SerializeField] TextMeshProUGUI w_text;
    [SerializeField] TextMeshProUGUI l_text;
    [SerializeField] string win_text;
    [SerializeField] string lose_text;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (base.IsOwner)
        {
            // Set up camera
            main_camera = Camera.main;
            Debug.Log("Camera_supposed_to_be_assigned");
            if (main_camera == null) Debug.Log("Camera_was_not_assigned");
            Game.Instance.player = this;

            SetUpUI();

            if (TryGetComponent(out RobberScript robber))
            {
                robber.player = GetComponent<Player>();
                narrow_dark_filter.SetActive(true);
                wide_dark_filter.SetActive(true);
                main_camera.GetComponent<CameraBehavior>().CameraMode(camera_mode.ROBBER);
                if (IsOwner) GetComponent<FootstepManager>().enabled = false;
            }
            else if (TryGetComponent(out GhostScript ghost))
            {
                ghost.player = GetComponent<Player>();
                ghost.default_speed = speed;
                wide_dark_filter.SetActive(true);
                main_camera.GetComponent<CameraBehavior>().robber_filter.GetComponent<Image>().color = ghost.stepvision_color;
                main_camera.GetComponent<CameraBehavior>().CameraMode(camera_mode.GHOST);
                if (Game.Instance.item_lottery != null) Game.Instance.item_lottery.ClearLocations();
            }

            // for testing game over screen
            //if (IsHost) GameOverServerRpc(false);
            //else GameOverServerRpc(true);
        }
        else
        {
            GetComponent<InputController>().enabled = false;
            if (TryGetComponent(out AudioSource white_noise)) white_noise.enabled = false;
        }
    }

    private void Update()
    {
        if (main_camera != null && this.gameObject.activeSelf) main_camera.GetComponent<CameraBehavior>().to_follow = transform.position;
    }

    private void FixedUpdate()
    {
        IndicationRotation();
    }

    void IndicationRotation()
    {
        // indication
        if (indication.activeSelf && TryGetComponent(out GhostScript ghost))
        {
            Vector3 robber_direction = (Game.Instance.robber.Value.transform.position - transform.position).normalized;
            indication.transform.rotation = Quaternion.FromToRotation(Vector3.up, robber_direction);

            if (Vector2.Distance(Game.Instance.robber.Value.transform.position, transform.position) < indication_hide_distance)
            {
                indication_sprite.enabled = false;
            }
            else indication_sprite.enabled = true;
        }
        else
        {
            if (indication.activeSelf && TryGetComponent(out RobberScript robber))
            {
                Vector3 robber_direction = (Game.Instance.ghost.Value.transform.position - transform.position).normalized;
                indication.transform.rotation = Quaternion.FromToRotation(Vector3.up, robber_direction);

                if (Vector2.Distance(Game.Instance.ghost.Value.transform.position, transform.position) < indication_hide_distance)
                {
                    indication_sprite.enabled = false;
                }
                else indication_sprite.enabled = true;
            }
        }
    } // Optimize this
    public void Indication(bool is_on)
    {
        if (IsOwner) indication.SetActive(is_on);
    }

    public IEnumerator BlinkingLives()
    {
        hp_bar.SetActive(true);
        float timer = blinking_duration;
        is_blinking = true;

        current_hp--;
        lives[current_hp].color = wasted_life_color;

        //check for game over
        if (current_hp == 0 && TryGetComponent(out RobberScript robber))
        {
            GameOverServerRpc(false);
            if (Game.Instance.ghost.Value != null) Game.Instance.ghost.Value.GetComponent<Player>().GameOverServerRpc(true);
        }

        while (timer > 0)
        {
            timer -= blinking_interval;

            hp_bar.SetActive(!hp_bar.activeSelf);

            yield return new WaitForSeconds(blinking_interval);
        }

        is_blinking = false;
        hp_bar.SetActive(false);
    }

    void SetUpUI()
    {
        w_text = Game.game_over.win_text;
        l_text = Game.game_over.lose_text;

        w_text.text = win_text;
        l_text.text = lose_text;
    }

    [ServerRpc(RequireOwnership = false)]
    public void GameOverServerRpc(bool won)
    {
        GameOverObserverRpc(won);
    }

    [ObserversRpc]
    public void GameOverObserverRpc(bool won)
    {
        if (IsOwner)
        {
            Debug.Log("GAME OVER: Player " + won);
            frozen = true;

            if (IsHost) Game.game_over.GameOverUIOn(won, true);
            else Game.game_over.GameOverUIOn(won, false);
        }
    }

}

