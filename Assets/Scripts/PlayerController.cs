using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionAsset inputActions;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float acceleration = 50f;
    [SerializeField] private float deceleration = 70f;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 18f;
    [SerializeField] private float dashDuration = 0.15f;

    [Header("Grounding")]
    [SerializeField] private LayerMask groundLayer = ~0;

    [Header("Shooting")]
    [SerializeField] private GameObject projectile;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private AudioClip shootSound;
    [SerializeField] private AudioClip bellSound;

    private Rigidbody2D playerRigidbody;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction dashAction;
    private InputAction shootAction;
    private Vector2 moveInput;
    private bool jumpRequested;
    private bool dashRequested;
    private bool isGrounded;
    private bool hasAirDash = true;
    private bool isDashing;
    private float dashTimeRemaining;
    private int facingDirection = 1;

    private void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();

        if (inputActions == null)
        {
            Debug.LogError("Assign the PlayerInputActions asset to PlayerController.", this);
            return;
        }

        InputActionMap playerMap = inputActions.FindActionMap("Player", true);
        moveAction = playerMap.FindAction("Move", true);
        jumpAction = playerMap.FindAction("Jump", true);
        dashAction = playerMap.FindAction("Dash", true);
        shootAction = playerMap.FindAction("Shoot", true);
    }

    private void OnEnable()
    {
        // Enable the Player action map when the script is enabled
        if (inputActions != null)
        {
            inputActions.FindActionMap("Player").Enable();
        }
    }

    private void OnDisable()
    {
        // Disable the Player action map when the script is disabled
        if (inputActions != null)
        {
            inputActions.FindActionMap("Player").Disable();
        }
    }

    private void Update()
    {
        if (moveAction == null)
        {
            return;
        }

        // Read input values and check for jump and dash requests
        moveInput = moveAction.ReadValue<Vector2>();
        jumpRequested |= jumpAction.WasPressedThisFrame();
        dashRequested |= dashAction.WasPressedThisFrame();

        // Update the facing direction based on horizontal movement input
        if (Mathf.Abs(moveInput.x) > 0.01f)
        {
            facingDirection = moveInput.x > 0f ? 1 : -1;
        }

        // Handle shooting input
        if (shootAction != null && shootAction.WasPressedThisFrame())
        {
            Shoot();
        }
    }

    private void FixedUpdate()
    {
        if (playerRigidbody == null || moveAction == null)
        {
            return;
        }

        // Handle dashing logic
        if (isDashing)
        {
            dashTimeRemaining -= Time.fixedDeltaTime;
            playerRigidbody.linearVelocity = new Vector2(facingDirection * dashSpeed, 0f);

            if (dashTimeRemaining <= 0f)
            {
                isDashing = false;
                playerRigidbody.gravityScale = 1f;
            }

            return;
        }

        Move();

        if (jumpRequested && isGrounded)
        {
            playerRigidbody.linearVelocity = new Vector2(playerRigidbody.linearVelocity.x, jumpForce);
            isGrounded = false;
        }

        if (dashRequested && (isGrounded || hasAirDash))
        {
            StartDash();
        }

        jumpRequested = false;
        dashRequested = false;
    }

    private void Move()
    {
        // Calculate the target speed based on input and apply acceleration or deceleration
        float targetSpeed = moveInput.x * moveSpeed;
        float speedChange = Mathf.Abs(targetSpeed) > 0.01f ? acceleration : deceleration;
        float newSpeed = Mathf.MoveTowards(playerRigidbody.linearVelocity.x, targetSpeed, speedChange * Time.fixedDeltaTime);

        playerRigidbody.linearVelocity = new Vector2(newSpeed, playerRigidbody.linearVelocity.y);
    }

    private void StartDash()
    {
        // Start the dash by setting the appropriate flags and adjusting gravity
        isDashing = true;
        dashTimeRemaining = dashDuration;
        playerRigidbody.gravityScale = 0f;

        if (!isGrounded)
        {
            hasAirDash = false;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if ((groundLayer.value & (1 << collision.gameObject.layer)) == 0)
        {
            return;
        }

        // Check if the player is grounded based on the collision contacts
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                isGrounded = true;
                hasAirDash = true;
                return;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // Check if the player has exited a collision with the ground layer
        if ((groundLayer.value & (1 << collision.gameObject.layer)) != 0)
        {
            isGrounded = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check for collisions with objects that can trigger victory conditions
        if (other.CompareTag("Campana") || other.gameObject.name == "Campana")
        {
            PlaySound(bellSound, other.transform.position);
            GameFlowController.Instance?.ShowVictory();
        }
    }

    private void Shoot()
    {
        // Instantiate a projectile at the specified spawn point or the player's position if no spawn point is assigned
        if (projectile == null)
        {
            Debug.LogError("Assign a projectile prefab to PlayerShooting.", this);
            return;
        }

        Transform spawnPoint = projectileSpawnPoint != null ? projectileSpawnPoint : transform;
        PlaySound(shootSound, spawnPoint.position);
        Instantiate(projectile, spawnPoint.position, Quaternion.identity);
    }

    private void PlaySound(AudioClip clip, Vector3 position)
    {
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, position);
        }
    }
}
