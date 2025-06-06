using FishNet.Object;
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
    public ArrowPointer indication;
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

    private bool lastFlipXState;

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
                TutorialProgress.Robber = true;
            }
            else if (TryGetComponent(out GhostScript ghost))
            {
                ghost.player = GetComponent<Player>();
                ghost.default_speed = speed;
                wide_dark_filter.SetActive(true);
                main_camera.GetComponent<CameraBehavior>().robber_filter.GetComponent<Image>().color = ghost.stepvision_color;
                main_camera.GetComponent<CameraBehavior>().CameraMode(camera_mode.GHOST);
                if (Game.Instance.item_lottery != null) Game.Instance.item_lottery.ClearLocations();
                TutorialProgress.Ghost = true;
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

        UpdateIndication();
    }

    public void Indication(bool is_on)
    {
        if (IsOwner) indication.gameObject.SetActive(is_on);
    }

    void UpdateIndication()
    {
        if (TryGetComponent(out RobberScript robber))
        {
            if (Game.Instance.ghost.Value != null) indication.target = Game.Instance.ghost.Value.transform.position;
        }
        else if (TryGetComponent(out GhostScript ghost))
        {
            if (Game.Instance.robber.Value != null) indication.target = Game.Instance.robber.Value.transform.position;
        }
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

    void LateUpdate()
    {
        // Flip state 
        if (sprite.flipX != lastFlipXState)
        {
            SyncFlipXServerRpc(sprite.flipX);
            lastFlipXState = sprite.flipX;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void SyncFlipXServerRpc(bool flipX)
    {
        SyncFlipXObserversRpc(flipX);
    }

    [ObserversRpc]
    void SyncFlipXObserversRpc(bool flipX)
    {
        sprite.flipX = flipX;
        if (aura_sprite != null) aura_sprite.flipX = flipX;
    }
}

