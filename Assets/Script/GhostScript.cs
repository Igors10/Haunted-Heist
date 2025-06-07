using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    [HideInInspector] public GhostUI ghostUI;
    [SerializeField] float dash_cooldown;
    [SerializeField] float stepvision_cooldown;

    // Dashing Variables
    [HideInInspector] public bool is_aiming;
    [HideInInspector] public bool is_dashing;
    bool is_dash_ready;


    Vector2 mouse_position;
    Vector2 charge_target_position = Vector2.zero;
    float charge_time;
    [SerializeField] float dash_duration;
    [SerializeField] float dash_length;
    [SerializeField] float dash_delay_time;
    Vector3 charge_starting_position;
    Vector3 last_valid_position;

    [SerializeField] float dash_prep_speed;
    [SerializeField] Image full_aiming_arrow;
    Color aiming_arrow_default_color;
    Vector2 last_aiming_position = new Vector2(0f, 0f);

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

    // Input

    [SerializeField] float joystickDeadZone = 0.2f;

    // Ghost's wild
    [HideInInspector] public bool wild_mode_on;
    [SerializeField] float wild_mode_speed_mod;

    private Vector3 dashDirection;

    Animator ghostAnimator;
    Animator ghostAttackingAnimator;

    public override void OnStartClient()
    {
        base.OnStartClient();
        Debug.Log("Ghost OnStartClient");

        // Initializing the boundary layer
        boundaryLayer = LayerMask.GetMask("MapBoundary");

        FindTeleportationPoint(GameObject.Find("teleportation_point_1"));
        FindTeleportationPoint(GameObject.Find("teleportation_point_2"));
        FindTeleportationPoint(GameObject.Find("teleportation_point_3"));
        FindTeleportationPoint(GameObject.Find("teleportation_point_4"));

        ghostAnimator = ghost_hiding.GetComponent<Animator>();
        ghostAttackingAnimator = ghost_attacking.GetComponent<Animator>();

        hiding_sprite = ghost_hiding.GetComponent<SpriteRenderer>();
        hiding_color = hiding_sprite.color;
        aiming_arrow_default_color = full_aiming_arrow.color;

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
            // Different aiming logic depending on if gamepad is used or not
            Vector2 targetPosition = (GameData.is_gamepad_used) ? JoystickAimInput() : MouseAimInput();

            if (is_aiming) AimForCharge(targetPosition);
            if (charge_target_position != Vector2.zero) Charging();

            if (!is_dashing)
            {
                last_valid_position = transform.position;
            }


            if (Input.GetKeyDown(KeyCode.O)) StartCoroutine(WildMode());
        }
    }


    Vector2 JoystickAimInput()
    {
        Vector2 aimInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        Vector2 joystickInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        bool usingJoystick = joystickInput.magnitude > joystickDeadZone;

        Vector2 targetPosition;

        //animation set to aim the idle - flipping through the sprites


        if (usingJoystick)
        {
            // Joystick
            Vector2 direction = joystickInput.normalized;
            targetPosition = (Vector2)transform.position + direction * 3f; // Scalar distance, can be increased.
            last_aiming_position = targetPosition;
        }
        else
        {
            targetPosition = last_aiming_position;
        }

        return targetPosition;
    }

    Vector2 MouseAimInput()
    {
        Vector3 screen_mouse_position = Input.mousePosition;
        Vector2 targetPosition = player.main_camera.ScreenToWorldPoint(screen_mouse_position);

        return targetPosition;
    }

    public IEnumerator WildMode()
    {
        SyncHideServerRpc(false);

        GameData.is_ghost_wild = true;

        yield return new WaitForSeconds(0.3f);

        player.frozen = false;

        default_speed *= wild_mode_speed_mod;
        player.speed *= wild_mode_speed_mod;
    }

    void AimForCharge(Vector2 target_position)
    {
        Vector2 direction = new Vector3(target_position.x, target_position.y, 0) - transform.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        aiming_arrow.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        // Charging up the dash

        //animation set to aim the idle - flipping through the sprites
        ghostAnimator.SetBool("isLoading", true);
        ghostAnimator.SetBool("isWalking", false);

        if (full_aiming_arrow.fillAmount < 1f)
        {
            full_aiming_arrow.fillAmount += Time.deltaTime * dash_prep_speed;
            full_aiming_arrow.color = ghostUI.charged_color_dash;
        }
        else
        {
            // Dash is ready
            //ghostUI.DashReady();
            full_aiming_arrow.fillAmount = 1f;
            is_dash_ready = true;
            full_aiming_arrow.color = aiming_arrow_default_color;
        }

        //full_aiming_arrow.fillAmount = ghostUI.dash_fill.fillAmount;

    }

    void Charging()
    {
        charge_time += Time.deltaTime;
        float progress = ((charge_time / dash_duration) > 1f) ? 1 : charge_time / dash_duration;
        float coolT = Mathf.Pow(progress, 2);

        // Calculating the next position 
        Vector3 nextPosition = Vector3.Lerp(charge_starting_position, charge_target_position, coolT);

        // Before moving, check if the path is clear
        float distanceToMove = Vector3.Distance(transform.position, nextPosition);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dashDirection, distanceToMove, boundaryLayer);

        // Also check for pushers, but only if we want to stop at them
        RaycastHit2D pusherHit = Physics2D.Raycast(transform.position, dashDirection, distanceToMove);
        if (pusherHit.collider != null && pusherHit.collider.CompareTag("Pusher"))
        {
            // Stop just before the pusher
            Vector3 stopPosition = transform.position + dashDirection * (pusherHit.distance - boundaryOffset);
            player.transform.position = stopPosition;
            EndCharge();
            return;
        }

        if (hit.collider != null)
        {
            // If we hit a boundary, stop just before it
            Vector3 stopPosition = transform.position + dashDirection * (hit.distance - boundaryOffset);
            player.transform.position = stopPosition;
            EndCharge();
            return;
        }

        // If the path is clear, move to the next position
        player.transform.position = nextPosition;

        // If we've reached the end of the dash, end it
        if (progress >= 1) EndCharge();
    }

    void CheckAndResolvePusherCollision()
    {
        if (!IsOwner) return;

        // Check if the ghost is inside a pusher
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.2f);
        bool insidePusher = false;

        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Pusher"))
            {
                insidePusher = true;
                break;
            }
        }

        // If inside a pusher, we need to find a safe position
        if (insidePusher)
        {
            // Attempt to find a safe position along the dash path
            FindSafePositionAlongDashPath();
        }
    }

    void FindSafePositionAlongDashPath()
    {
        // Get the dash direction
        Vector3 direction = -dashDirection; // Try going backward along the dash path first

        // Try different distances to find a safe spot
        float[] distances = { 0.5f, 1.0f, 1.5f, 2.0f };

        foreach (float distance in distances)
        {
            Vector3 testPosition = transform.position + direction * distance;

            // Check if this position is safe (not inside a pusher)
            Collider2D[] colliders = Physics2D.OverlapCircleAll(testPosition, 0.1f);
            bool positionSafe = true;

            foreach (Collider2D collider in colliders)
            {
                if (collider.CompareTag("Pusher"))
                {
                    positionSafe = false;
                    break;
                }
            }

            if (positionSafe)
            {
                // Found a safe position, move there
                transform.position = testPosition;
                Debug.Log("Found safe position at distance: " + distance);
                return;
            }
        }
        Vector2[] cardinalDirections = {
        Vector2.up,
        Vector2.right,
        Vector2.down,
        Vector2.left
    };

        foreach (Vector2 dir in cardinalDirections)
        {
            // Try a reasonable escape distance
            Vector3 testPosition = transform.position + new Vector3(dir.x, dir.y, 0) * 0.5f;

            // Check if this position is safe
            Collider2D[] colliders = Physics2D.OverlapCircleAll(testPosition, 0.1f);
            bool positionSafe = true;

            foreach (Collider2D collider in colliders)
            {
                if (collider.CompareTag("Pusher"))
                {
                    positionSafe = false;
                    break;
                }
            }

            if (positionSafe)
            {
                // Found a safe position, move there
                transform.position = testPosition;
                Debug.Log("Found safe position in cardinal direction: " + dir);
                return;
            }
        }

        // As a last resort, use the last valid position
        transform.position = last_valid_position;
        Debug.Log("No safe position found, using last valid position");
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
        //animation set to start the charge
        ghostAttackingAnimator.SetBool("attackEnded", false);

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

        // Calculate initial dash target and save the direction
        dashDirection = aiming_arrow.transform.up;
        Vector3 initialTargetPosition = transform.position + dashDirection * dash_length;

        charge_target_position = initialTargetPosition;
        charge_starting_position = transform.position;
        last_valid_position = charge_starting_position;
        charge_time = 0f;
        is_dashing = true;

        // Cooldown
        StartCoroutine(ghostUI.Cooldown(dash_cooldown, true));
        //ghostUI.DashUsed();

        Debug.Log("Charge Started");
    }


    void EndCharge()
    {
        //animation set to finish the dash
        ghostAttackingAnimator.SetBool("attackEnded", true);

        aiming_arrow.SetActive(false);
        charge_target_position = Vector3.zero;
        is_dashing = false;
        is_dash_ready = false;

        // Check for rt_tutorial
        if (Game.Instance.rt_tutorial != null && Game.Instance.rt_tutorial.ghost_dashed == false) Game.Instance.rt_tutorial.ghost_dashed = true;

        // Check if ghost is inside a pusher object when dash ends
        CheckAndResolvePusherCollision();

        SyncHideServerRpc(true);
        Debug.Log("Charge Ended");
    }


    void HandleTilemapCollision()
    {
        // Cast rays in 8 directions to find the nearest exit
        Vector2[] directions = new Vector2[]
        {
        Vector2.up,
        Vector2.right,
        Vector2.down,
        Vector2.left,
        new Vector2(1, 1).normalized,
        new Vector2(1, -1).normalized,
        new Vector2(-1, -1).normalized,
        new Vector2(-1, 1).normalized
        };

        float maxRayDistance = 2f; // Maximum distance to check
        float shortestDistance = maxRayDistance;
        Vector2 bestDirection = Vector2.zero;
        bool foundExit = false;

        // Try to find the nearest exit point
        foreach (Vector2 direction in directions)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, maxRayDistance, boundaryLayer);
            if (hit.collider == null) // No boundary in this direction
            {
                // Cast another ray but this time looking for pushers
                RaycastHit2D pusherHit = Physics2D.Raycast(transform.position, direction, maxRayDistance);
                if (pusherHit.collider != null && pusherHit.collider.CompareTag("Pusher"))
                {
                    // Found the edge of the pusher
                    if (pusherHit.distance < shortestDistance)
                    {
                        shortestDistance = pusherHit.distance;
                        bestDirection = direction;
                        foundExit = true;
                    }
                }
                else
                {
                    // Open direction with no obstacles
                    float distance = 0.5f; // Default push distance
                    if (distance < shortestDistance)
                    {
                        shortestDistance = distance;
                        bestDirection = direction;
                        foundExit = true;
                    }
                }
            }
        }

        if (foundExit)
        {
            // Push in the best direction with a little extra to ensure we're outside
            float pushDistance = shortestDistance + 0.3f;
            transform.position += new Vector3(bestDirection.x, bestDirection.y, 0) * pushDistance;
            Debug.Log("Pushed out of tilemap in direction: " + bestDirection);
        }
        else
        {
            // If no clear exit found, try teleporting to last valid position
            transform.position = last_valid_position;
            Debug.Log("No clear exit found, teleported to last valid position");
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
        if (is_dashing || ghostUI.dash_fill.fillAmount < 1f) return;

        // updating the indicator
        DashIndicatorServerRpc(is_on);

        // making the ghost slower while aiming
        player.speed = (is_on) ? stepvision_speed : default_speed;
        full_aiming_arrow.fillAmount = 0f;

        is_aiming = is_on;
        aiming_arrow.SetActive(is_aiming);

        // Stopping the aiming animation
        ghostAnimator.SetBool("isLoading", false);
        ghostAnimator.SetBool("isWalking", true);

        if (is_on == false && Vector2.Distance(mouse_position, transform.position) > 0f && is_dash_ready)
        {
            StartCoroutine(StartCharge());
        }
        //else if (is_on == false) ghostUI.DashUsed();
    }

    public void StepVision(bool is_on)
    {
        if (ghostUI.stepvision_fill.fillAmount < 1) return;

        ghostUI.StepVisionFilter(is_on);

        // Cooldown
        if (!is_on) StartCoroutine(ghostUI.Cooldown(stepvision_cooldown, false));

        player.main_camera.GetComponent<CameraBehavior>().SpecialVision(is_on, false);

        player.speed = (is_on) ? stepvision_speed : default_speed;
    }

    // Updating the dash indicatior for the robber over network

    [ServerRpc(RequireOwnership = false)]
    public void DashIndicatorServerRpc(bool is_on)
    {
        DashIndicatorObserversRpc(is_on);

    }

    [ObserversRpc]
    public void DashIndicatorObserversRpc(bool is_on)
    {
        if (Game.Instance.robber.Value != null) Game.Instance.robber.Value.GetComponent<Player>().Indication(is_on);
    }
    // Changing states HIDING - ATTACKING ======================================

    [ServerRpc(RequireOwnership = false)]
    public void CollecteItemCounterUpdateServerRpc()
    {
        CollectedItemCounterUpdateObserversRpc();

    }

    [ObserversRpc]
    void CollectedItemCounterUpdateObserversRpc()
    {
        Game.Instance.item_lottery.gameObject.GetComponent<TextJuice>().UpdateCounterTextWithJuice();
    }

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

        //Flipping the ghost while dashing
        ghost_attacking.GetComponent<SpriteRenderer>().flipX = ghost_hiding.GetComponent<SpriteRenderer>().flipX;

        //if (Game.Instance.robber.Value != null)

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

        //animation
        ghostAttackingAnimator.SetBool("isLaughing", false);

        int chosen_point = 0;

        for (int i = 0; i < teleportation_locations.Count; i++)
        {
            float old_distance = Vector2.Distance(transform.position, teleportation_locations[chosen_point]);
            float new_distance = Vector2.Distance(transform.position, teleportation_locations[i]);
            if (new_distance > old_distance) chosen_point = i;
        }

        if (Game.Instance.rt_tutorial != null) Game.Instance.rt_tutorial.teleported = true;

        return teleportation_locations[chosen_point];
    }

    public IEnumerator Catch()
    {
        Debug.Log("CATCHING: I caught the robber (Observer)");

        // animation
        ghostAnimator.SetBool("isLaughing", true);

        //SFX
        AudioManager.instance.PlaySFX("GhostLaugh");

        is_laughing = true;
        ghost_hiding.layer = LayerMask.NameToLayer("Default");
        if (player != null) player.frozen = true;
        float current_laughing_duration = laughing_duration;
        float current_alpha = hiding_color.a;

        // Blinking collected souls
        if (IsOwner) ghostUI.souls.IncreaseHealth();

        while (current_laughing_duration > 0)
        {
            hiding_sprite.color = new Color(hiding_color.r, hiding_color.g, hiding_color.b, current_alpha);
            current_alpha = current_laughing_duration / laughing_duration;

            current_laughing_duration--;
            yield return new WaitForSeconds(0.05f);
        }

        // stop animation
        ghostAnimator.SetBool("isLaughing", false);

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

        if (collision.gameObject.tag == "Vent")
        {
            // check for rt_tutorial vent_near_ghost
            if (Game.Instance.rt_tutorial != null)
                Game.Instance.rt_tutorial.vent_near_ghost = true;
        }
    }
}