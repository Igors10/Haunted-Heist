using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostScript : NetworkBehaviour
{
    [HideInInspector] public Player player;

    [SerializeField] GameObject aiming_arrow;
    [SerializeField] GameObject ghost_hiding;
    [SerializeField] GameObject ghost_attacking;
    public Color stepvision_color;
    [SerializeField] float stepvision_speed;
    [HideInInspector] public float default_speed;

    // GhostUI
    GhostUI ghostUI;
    [SerializeField] float dash_cooldown;
    [SerializeField] float stepvision_cooldown;

    // Dashing Variables
    bool is_aiming;
    bool is_dashing;
    Vector2 mouse_position;
    Vector2 charge_target_position = Vector2.zero;
    float charge_time;
    [SerializeField] float dash_duration;
    [SerializeField] float dash_length;
    [SerializeField] float dash_delay_time;
    Vector3 charge_starting_position;
    Vector3 last_valid_position;

    // Boundary checking
    [SerializeField] LayerMask boundaryLayer;
    float boundaryCheckRadius = 0.1f;
    float boundaryOffset = 0.05f;

    // Catching Variables
    [SerializeField] float laughing_duration;
    bool is_laughing = false;
    List<Vector3> teleportation_locations = new List<Vector3>();
    SpriteRenderer hiding_sprite;
    Color hiding_color;

    // Ghost particle effects
    public ParticleSystem ghostParticleZoomIn;
    public ParticleSystem ghostParticleZoomOut;

    public override void OnStartClient()
    {
        base.OnStartClient();
        Debug.Log("Ghost OnStartClient");

        // Initializing the boundary layer
        boundaryLayer = LayerMask.GetMask("MapBoundary");

        if (IsOwner) AudioManager.instance.musicSource.gameObject.SetActive(false);

        FindTeleportationPoint(GameObject.Find("teleportation_point_1"));
        FindTeleportationPoint(GameObject.Find("teleportation_point_2"));
        FindTeleportationPoint(GameObject.Find("teleportation_point_3"));
        FindTeleportationPoint(GameObject.Find("teleportation_point_4"));

        hiding_sprite = ghost_hiding.GetComponent<SpriteRenderer>();
        hiding_color = hiding_sprite.color;

        if (IsOwner)
        {
            ghostUI = GameObject.Find("GhostUI").GetComponent<GhostUI>();
            if (ghostUI == null) Debug.Log("Couldnt find ghost UI");
            else ghostUI.EnableUI();
        }
    }

    void FindTeleportationPoint(GameObject teleport_point)
    {
        if (teleport_point == null) return;

        teleportation_locations.Add(teleport_point.transform.position);
        Debug.Log("Teleportation pont added at " + teleport_point.transform.position);
    }

    private void FixedUpdate()
    {
        if (IsOwner)
        {
            Vector3 screen_mouse_position = Input.mousePosition;

            mouse_position = player.main_camera.ScreenToWorldPoint(screen_mouse_position);

            if (is_aiming) AimForCharge(mouse_position);
            if (charge_target_position != Vector2.zero) Charging();

            // If not dashing, continuously update the last valid position to the current position
            if (!is_dashing)
            {
                last_valid_position = transform.position;
            }
        }
    }

    void AimForCharge(Vector2 target_position)
    {
        Vector2 direction = new Vector3(target_position.x, target_position.y, 0) - transform.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        aiming_arrow.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    void Charging()
    {
        charge_time += Time.deltaTime;
        float progress = ((charge_time / dash_duration) > 1f) ? 1 : charge_time / dash_duration;
        float coolT = Mathf.Pow(progress, 2);

        // Calculating the next position 
        Vector3 nextPosition = Vector3.Lerp(charge_starting_position, charge_target_position, coolT);

        // Check if the next position would be inside a map boundary
        if (IsPositionValid(nextPosition))
        {
            // If it's valid, move to that position
            player.transform.position = nextPosition;
        }
        else
        {
            // If it's not valid, find the closest valid position
            Vector3 direction = (nextPosition - charge_starting_position).normalized;
            float distance = Vector3.Distance(charge_starting_position, nextPosition);

            // Searching for the closest valid position
            float minDistance = 0;
            float maxDistance = distance;
            float currentDistance = maxDistance;
            Vector3 validPosition = charge_starting_position;

            for (int i = 0; i < 10; i++)
            {
                currentDistance = (minDistance + maxDistance) / 2;
                Vector3 testPosition = charge_starting_position + direction * currentDistance;

                if (IsPositionValid(testPosition))
                {
                    validPosition = testPosition;
                    minDistance = currentDistance;
                }
                else
                {
                    maxDistance = currentDistance;
                }
            }

            // Move to the valid position
            player.transform.position = validPosition;

            // End the dash since we hit map boundary
            EndCharge();
            return;
        }

        if (progress >= 1) EndCharge();
    }

    // Check if a position is valid (not inside a boundary)
    bool IsPositionValid(Vector3 position)
    {
        // Check for map boundaries
        Collider2D[] boundaryColliders = Physics2D.OverlapCircleAll(position, boundaryCheckRadius, boundaryLayer);
        if (boundaryColliders.Length > 0)
        {
            return false;
        }

        // During dashing, we ignore pusher colliders
        if (is_dashing)
        {
            return true;
        }

        // When not dashing, check for pusher colliders
        Collider2D[] pusherColliders = Physics2D.OverlapCircleAll(position, boundaryCheckRadius);
        foreach (Collider2D collider in pusherColliders)
        {
            if (collider.CompareTag("Pusher"))
            {
                return false;
            }
        }

        return true;
    }

    IEnumerator StartCharge()
    {
        //SFX
        AudioManager.instance.PlaySFXGlobal("GhostWarp");


        SyncHideServerRpc(false);
        is_aiming = false;

        float dash_delay_timer = dash_delay_time;

        while (dash_delay_timer > 0)
        {
            dash_delay_timer -= Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        // Dash start soundeffect
        AudioManager.instance.PlaySFXGlobal("Dash");

        // Calculate initial dash target
        Vector3 dashDirection = aiming_arrow.transform.up;
        Vector3 initialTargetPosition = transform.position + dashDirection * dash_length;

        charge_target_position = initialTargetPosition;
        charge_starting_position = transform.position;
        last_valid_position = charge_starting_position;
        charge_time = 0f;
        is_dashing = true;

        // Cooldown
        StartCoroutine(ghostUI.Cooldown(dash_cooldown, true));

        Debug.Log("Charge Started");
    }

    void EndCharge()
    {
        aiming_arrow.SetActive(false);
        charge_target_position = Vector3.zero;
        is_dashing = false;


        // Check if ghost is inside a pusher object when dash ends
        CheckAndResolvePusherCollision();

        SyncHideServerRpc(true);
        Debug.Log("Charge Ended");
    }

    void CheckAndResolvePusherCollision()
    {
        if (!IsOwner) return;

        // Get all colliders at the current position
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.1f);

        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Pusher"))
            {
                // Calculate the nearest edge point
                Vector2 direction = GetNearestExitDirection(collider);

                // Push the ghost out in that direction
                float pushDistance = 0.5f;
                transform.position += new Vector3(direction.x, direction.y, 0) * pushDistance;

                // Stop the loop after the first pusher is found
                break;
            }
        }
    }

    Vector2 GetNearestExitDirection(Collider2D pusherCollider)
    {
        Vector2 pusherCenter = pusherCollider.bounds.center;
        Vector2 direction = (Vector2)transform.position - pusherCenter;

        if (pusherCollider is CircleCollider2D)
        {
            return direction.normalized;
        }

        if (pusherCollider is BoxCollider2D)
        {
            BoxCollider2D boxCollider = pusherCollider as BoxCollider2D;
            Vector2 extents = boxCollider.bounds.extents;

            // Calculate the ratio
            float xRatio = direction.x / extents.x;
            float yRatio = direction.y / extents.y;

            // Need to check which side is closer
            if (Mathf.Abs(xRatio) > Mathf.Abs(yRatio))
            {
                return new Vector2(Mathf.Sign(direction.x), 0);
            }
            else
            {
                return new Vector2(0, Mathf.Sign(direction.y));
            }
        }

        return direction.normalized;
    }

    public void ChargeAttack(bool is_on)
    {
        if (ghostUI.dash_fill.fillAmount < 1 || is_dashing) return;

        is_aiming = is_on;
        aiming_arrow.SetActive(is_aiming);

        if (is_on == false && Vector2.Distance(mouse_position, transform.position) > 0f)
        {
            StartCoroutine(StartCharge());
        }
    }

    public void StepVision(bool is_on)
    {
        if (ghostUI.stepvision_fill.fillAmount < 1) return;

        // Cooldown
        if (!is_on) StartCoroutine(ghostUI.Cooldown(stepvision_cooldown, false));

        player.main_camera.GetComponent<CameraBehavior>().SpecialVision(is_on, false);

        player.speed = (is_on) ? stepvision_speed : default_speed;
    }

    // Changing states HIDING - ATTACKING ======================================

    [ServerRpc(RequireOwnership = false)]
    public void SyncHideServerRpc(bool is_hiding)
    {
        SyncHideObserversRpc(is_hiding);

    }

    [ObserversRpc]
    public void SyncHideObserversRpc(bool is_hiding)
    {
        if (IsOwner) player.frozen = !is_hiding;
        ghost_hiding.SetActive(is_hiding);
        ghost_attacking.SetActive(!is_hiding);
        is_dashing = !is_hiding;
        Game.Instance.robber.Value.GetComponent<Player>().Indication(!is_hiding);

        //ghost particle effects for charging
        if (is_hiding)
        {
            if (ghostParticleZoomIn.isEmitting)
            {
                ghostParticleZoomIn.Clear();
                Debug.Log("ZoomIn cleared");
            }

            ghostParticleZoomOut.Stop();
            if (ghostParticleZoomOut.isStopped)
            {
                ghostParticleZoomOut.Play();
                Debug.Log("ZoomOut activated");
            }
        }
        else
        {
            ghostParticleZoomIn.Stop();
            if (ghostParticleZoomIn.isStopped)
            {
                ghostParticleZoomIn.Play();
                Debug.Log("ZoomIn activated");
            }
        }

    }

    // ==========================================================================

    [ServerRpc(RequireOwnership = false)]
    public void SyncCatchServerRpc()
    {
        SyncCatchObserverRpc();
    }

    [ObserversRpc]
    public void SyncCatchObserverRpc()
    {
        if (is_laughing) return;
        StartCoroutine(Catch());
    }

    Vector2 TeleportAway()
    {
        Debug.Log("TeleportsAway");

        int chosen_point = 0;

        for (int i = 0; i < teleportation_locations.Count; i++)
        {
            float old_distance = Vector2.Distance(transform.position, teleportation_locations[chosen_point]);
            float new_distance = Vector2.Distance(transform.position, teleportation_locations[i]);
            if (new_distance > old_distance) chosen_point = i;
        }

        return teleportation_locations[chosen_point];
    }

    IEnumerator Catch()
    {
        Debug.Log("CATCHING: I caught the robber (Observer)");

        //SFX
        AudioManager.instance.PlaySFX("GhostLaugh");

        is_laughing = true;
        ghost_hiding.layer = LayerMask.NameToLayer("Default");
        if (player != null) player.frozen = true;
        float current_laughing_duration = laughing_duration;
        float current_alpha = hiding_color.a;

        // Blinking collected souls
        if (IsOwner) StartCoroutine(player.BlinkingLives());

        while (current_laughing_duration > 0)
        {
            hiding_sprite.color = new Color(hiding_color.r, hiding_color.g, hiding_color.b, current_alpha);
            current_alpha = current_laughing_duration / laughing_duration;

            current_laughing_duration--;
            yield return new WaitForSeconds(0.1f);
        }

        hiding_sprite.color = hiding_color;
        if (player != null) player.frozen = false;
        ghost_hiding.layer = LayerMask.NameToLayer("Ghost");
        is_laughing = false;

        transform.position = TeleportAway();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Pusher") && ghost_hiding.activeSelf)
        {
            // Push the ghost away from this object
            Vector2 direction = transform.position - collision.transform.position;
            direction.Normalize();

            transform.position += new Vector3(direction.x, direction.y, 0) * 0.3f;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Pusher"))
        {
            // Push the ghost away from this object
            Vector2 direction = transform.position - collision.transform.position;
            direction.Normalize();

            transform.position += new Vector3(direction.x, direction.y, 0) * 0.3f;
        }
    }
}