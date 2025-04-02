using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;

public class PlayerController : NetworkBehaviour
{
    [Header("Base setup")]
    public float moveSpeed = 7.5f; // Movement speed
    private Rigidbody2D rb;       // 2D Rigidbody for movement
    private Vector2 movement;     // Player's input direction

    [HideInInspector]
    public bool canMove = true; // Allow movement toggle

    [SerializeField]
    private float cameraYOffset = 10.0f; // Camera height above the player
    private Camera playerCamera;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (base.IsOwner)
        {
            // Set up camera
            playerCamera = Camera.main;
            playerCamera.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - cameraYOffset);
            playerCamera.transform.SetParent(transform); // Camera follows player
        }
        else
        {
            gameObject.GetComponent<PlayerController>().enabled = false;
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!base.IsOwner || !canMove)
            return;

        // Get input from WASD or arrow keys
        movement.x = Input.GetAxisRaw("Horizontal"); // Left/Right
        movement.y = Input.GetAxisRaw("Vertical");   // Up/Down
    }

    [System.Obsolete]
    void FixedUpdate()
    {
        if (!canMove)
            return;

        // Apply movement using Rigidbody2D
        rb.velocity = movement.normalized * moveSpeed;
    }
}
