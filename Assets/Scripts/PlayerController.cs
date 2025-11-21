using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{

    // Respawn system variables
    private RespawnPoint currentRespawnPoint;
    private Vector3 initialSpawnPosition;
    private Quaternion initialSpawnRotation;

    // Rigidbody of the player.
    private Rigidbody rb;

    // Variable to keep track of collected "PickUp" objects.
    private int count;

    // Movement along X and Y axes.
    private float movementX;
    private float movementY;

    // Track if player is on ground.
    private bool isGrounded;
    
    // Track if jump was requested (for input handling in Update, physics in FixedUpdate)
    private bool jumpRequested = false;

    // Speed at which the player moves.
    public float speed = 5f;

    // Jump force applied when jumping.
    public float jumpForce = 8f;

    // Distance to check for ground (ground detection).
    public float groundCheckDistance = 0.3f;

    // Layer mask for what counts as ground (all layers by default).
    public LayerMask groundLayerMask = -1; // All layers

    // Reference to collider for better ground detection.
    private Collider playerCollider;

    // UI text component to display count of "PickUp" objects collected.
    public TextMeshProUGUI countText;

    // UI object to display winning text.
    public GameObject winTextObject;

    // Dash variables
    private bool dashRequested = false;
    private bool isDashing = false;
    private float dashTimer = 0f;
    public float dashForce = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    private float dashCooldownTimer = 0f;
    private Vector3 dashDirection = Vector3.zero;
    
    // Win condition flag to prevent multiple triggers
    private bool winConditionTriggered = false;

    // Start is called before the first frame update.
    void Start()
    {
        // Get and store the Rigidbody component attached to the player.
        rb = GetComponent<Rigidbody>();

        // Get collider for ground detection
        playerCollider = GetComponent<Collider>();

        // Store initial spawn position and rotation
        initialSpawnPosition = transform.position;
        initialSpawnRotation = transform.rotation;

        // Initialize count to zero.
        count = 0;
        
        // Initialize movement to zero
        movementX = 0f;
        movementY = 0f;
        
        // Initialize grounded state
        isGrounded = false;

        // Update the count display.
        SetCountText();

        // Initially set the win text to be inactive.
        if (winTextObject != null)
        {
            winTextObject.SetActive(false);
        }
        
        // Set default speed if not set in inspector
        if (speed <= 0)
        {
            speed = 10f;
        }
        
        // Set drag on Rigidbody for proper movement (like Roll-a-Ball tutorial)
        if (rb != null)
        {
            // Set linearDamping (Unity 6) for proper stopping
            rb.linearDamping = 3f;
        }
    }

    // This function is called when a move input is detected.
    void OnMove(InputValue movementValue)
    {
        // Convert the input value into a Vector2 for movement.
        Vector2 movementVector = movementValue.Get<Vector2>();
        
        // Apply deadzone to prevent unwanted movement from gamepad drift
        float deadzone = 0.2f;
        if (movementVector.magnitude < deadzone)
        {
            movementVector = Vector2.zero;
        }
        else
        {
            // Normalize the input after deadzone to ensure consistent speed
            movementVector = movementVector.normalized * ((movementVector.magnitude - deadzone) / (1f - deadzone));
        }

        // Store the X and Y components of the movement.
        // Always update, even if zero, to ensure we stop when input stops
        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    // This function is called when jump input is detected (called by PlayerInput component).
    void OnJump(InputValue jumpValue)
    {
        // Request jump if button is pressed - actual jump happens in FixedUpdate
        if (jumpValue.isPressed)
        {
            jumpRequested = true;
        }
    }

    // This function is called when dash input is detected (called by PlayerInput component).
    void OnSprint(InputValue sprintValue)
    {
        // Request dash if button is pressed and cooldown is ready
        if (sprintValue.isPressed && dashCooldownTimer <= 0f && !isDashing)
        {
            dashRequested = true;
        }
    }
    
    // Update is called once per frame - handle input here
    void Update()
    {
        // Check for jump input using old Input System as backup (in case PlayerInput isn't working)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpRequested = true;
        }
        
        // Check for dash input using old Input System as backup
        if (Input.GetKeyDown(KeyCode.LeftShift) && dashCooldownTimer <= 0f && !isDashing)
        {
            dashRequested = true;
        }
        
        // Update dash cooldown timer
        if (dashCooldownTimer > 0f)
        {
            dashCooldownTimer -= Time.deltaTime;
        }
        
        // Update dash timer
        if (isDashing)
        {
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0f)
            {
                isDashing = false;
                dashCooldownTimer = dashCooldown;
            }
        }
    }

    // FixedUpdate is called once per fixed frame-rate frame - handle physics here
    private void FixedUpdate()
    {
        if (rb == null) return;
        
        // Check if player is on ground using raycast
        CheckGrounded();
        
        // Preserve current Y velocity for all movement calculations
        float verticalVelocity = rb.linearVelocity.y;
        
        // Handle dash request
        if (dashRequested && dashCooldownTimer <= 0f && !isDashing)
        {
            // Create a 3D movement vector using the X and Y inputs.
            Vector3 dashMovement = new Vector3(movementX, 0.0f, movementY);
            
            // If there's no movement input, dash forward in the direction the player is facing
            if (dashMovement.magnitude < 0.1f)
            {
                dashMovement = transform.forward;
            }
            else
            {
                dashMovement.Normalize();
            }
            
            // Start dash
            dashDirection = dashMovement;
            isDashing = true;
            dashTimer = dashDuration;
            dashRequested = false;
            dashCooldownTimer = dashCooldown;
        }
        
        // Apply dash force if currently dashing
        if (isDashing)
        {
            // Apply dash force in the dash direction
            Vector3 dashVelocity = dashDirection * dashForce;
            rb.linearVelocity = new Vector3(dashVelocity.x, verticalVelocity, dashVelocity.z);
            return; // Skip normal movement during dash
        }
        
        // Create a 3D movement vector using the X and Y inputs.
        Vector3 movement = new Vector3(movementX, 0.0f, movementY);
        
        // Normalize if magnitude > 1 to prevent diagonal movement being faster
        if (movement.magnitude > 1f)
        {
            movement.Normalize();
        }

        // Calculate horizontal velocity
        Vector3 targetVelocity = movement * speed;
        
        // Handle jump request (physics in FixedUpdate)
        
        if (jumpRequested && isGrounded)
        {
            // Apply upward velocity for jumping
            verticalVelocity = jumpForce;
            jumpRequested = false; // Reset jump request
        }
        else if (jumpRequested)
        {
            // If jump was requested but not grounded, just clear the request
            jumpRequested = false;
        }
        
        // Apply velocity - horizontal movement + vertical (jump or gravity)
        rb.linearVelocity = new Vector3(targetVelocity.x, verticalVelocity, targetVelocity.z);
    }

    // Check if the player is on the ground using a raycast.
    void CheckGrounded()
    {
        if (playerCollider == null)
        {
            // Fallback: use transform position if no collider
            float distance = groundCheckDistance + 0.1f;
            isGrounded = Physics.Raycast(transform.position, Vector3.down, distance, groundLayerMask);
            return;
        }

        // Get the bottom of the collider
        float colliderBottom = playerCollider.bounds.min.y;
        Vector3 rayStart = new Vector3(transform.position.x, colliderBottom + 0.05f, transform.position.z);
        float rayDistance = groundCheckDistance + 0.05f;
        
        // Check if we hit something on the ground layer using raycast
        isGrounded = Physics.Raycast(rayStart, Vector3.down, rayDistance, groundLayerMask);
        
        // Also check using a sphere cast for more reliable detection
        // This is more forgiving for uneven terrain
        if (!isGrounded)
        {
            Vector3 sphereCenter = new Vector3(transform.position.x, colliderBottom, transform.position.z);
            isGrounded = Physics.CheckSphere(sphereCenter, groundCheckDistance, groundLayerMask);
        }
        
        // Additional check: if Y velocity is very small and we're near ground, consider grounded
        if (!isGrounded && Mathf.Abs(rb.linearVelocity.y) < 0.1f)
        {
            float distanceToGround = transform.position.y - colliderBottom;
            if (distanceToGround < groundCheckDistance * 2f)
            {
                isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance * 2f, groundLayerMask);
            }
        }
    }


    void OnTriggerEnter(Collider other)
    {
        // Check if the object the player collided with has the "PickUp" tag.
        if (other.gameObject.CompareTag("PickUp"))
        {
            // Play pickup sound if the pickup has a PickupSound component
            PickupSound pickupSound = other.GetComponent<PickupSound>();
            if (pickupSound != null)
            {
                pickupSound.PlayPickupSound();
            }
            
            // Deactivate the collided object (making it disappear).
            other.gameObject.SetActive(false);

            // Increment the count of "PickUp" objects collected.
            count = count + 1;

            // Update the count display.
            SetCountText();
        }
    }

    // Function to update the displayed count of "PickUp" objects collected.
    void SetCountText()
    {
        if (countText != null)
        {
            countText.text = "Count: " + count.ToString();
        }

        // Check if the count has reached or exceeded the win condition.
        if (count >= 12 && !winConditionTriggered)
        {
            // Prevent multiple triggers
            winConditionTriggered = true;
            
            // Display the win text.
            if (winTextObject != null)
            {
                winTextObject.SetActive(true);
            }

            // Destroy the north wall to create an opening
            GameObject northWallLeft = GameObject.Find("North Wall Left (1)");
            // Try alternative path if not found (in case it's nested under Walls parent)
            if (northWallLeft == null)
            {
                northWallLeft = GameObject.Find("Walls/North Wall Left (1)");
            }
            if (northWallLeft == null)
            {
                northWallLeft = GameObject.Find("North Wall Left");
            }
            if (northWallLeft == null)
            {
                northWallLeft = GameObject.Find("Walls/North Wall Left");
            }
            
            if (northWallLeft != null)
            {
                Destroy(northWallLeft);
                Debug.Log("North Wall Left destroyed - path opened!");
            }
            else
            {
                Debug.LogWarning("Could not find North Wall Left to destroy!");
            }
            
            Debug.Log("You Win! All 12 pickups collected!");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            OnEnemyCollision();
        }
    }

    public void OnEnemyCollision()
    {
        // Update the winText to display "You Lose!" BEFORE destroying
        if (winTextObject != null)
        {
            winTextObject.gameObject.SetActive(true);
            var textComponent = winTextObject.GetComponent<TextMeshProUGUI>();
            if (textComponent != null)
            {
                textComponent.text = "You Lose!";
            }
        }
        
        // Disable player movement
        enabled = false;
        
        // Hide the player visually
        var meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.enabled = false;
        }
        
        // Destroy the player after a short delay so text can display
        Destroy(gameObject, 0.1f);
    }


    // Set the active respawn point (called by RespawnPoint when player touches it)
    public void SetRespawnPoint(RespawnPoint respawnPoint)
    {
        // If this is a new checkpoint or a later one, update the respawn point
        if (currentRespawnPoint == null || 
            respawnPoint.checkpointOrder >= currentRespawnPoint.checkpointOrder)
        {
            // Deactivate old respawn point visual
            if (currentRespawnPoint != null)
            {
                currentRespawnPoint.SetAsActiveRespawnPoint(false);
            }
            
            // Set new respawn point
            currentRespawnPoint = respawnPoint;
            currentRespawnPoint.SetAsActiveRespawnPoint(true);
        }
    }
    
    // Respawn the player at the last checkpoint or initial spawn
    public void Respawn()
    {
        Vector3 respawnPosition;
        Quaternion respawnRotation;
        
        // Use current respawn point if available, otherwise use initial spawn
        if (currentRespawnPoint != null)
        {
            respawnPosition = currentRespawnPoint.GetRespawnPosition();
            respawnRotation = currentRespawnPoint.GetRespawnRotation();
        }
        else
        {
            respawnPosition = initialSpawnPosition;
            respawnRotation = initialSpawnRotation;
        }
        
        // Reset player position and rotation
        transform.position = respawnPosition;
        transform.rotation = respawnRotation;
        
        // Reset velocity
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        
        // Reset dash state
        isDashing = false;
        dashRequested = false;
        dashTimer = 0f;
        dashCooldownTimer = 0f;
        
        // Reset movement input
        movementX = 0f;
        movementY = 0f;
        
        // Reset jump request
        jumpRequested = false;
    }
}