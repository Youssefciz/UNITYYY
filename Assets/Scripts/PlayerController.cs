using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{
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

    // Reference to the door object.
    public GameObject doorObject;

    // Start is called before the first frame update.
    void Start()
    {
        // Get and store the Rigidbody component attached to the player.
        rb = GetComponent<Rigidbody>();

        // Get collider for ground detection
        playerCollider = GetComponent<Collider>();

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
    
    // Update is called once per frame - handle input here
    void Update()
    {
        // Check for jump input using old Input System as backup (in case PlayerInput isn't working)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpRequested = true;
        }
    }

    // FixedUpdate is called once per fixed frame-rate frame - handle physics here
    private void FixedUpdate()
    {
        if (rb == null) return;
        
        // Check if player is on ground using raycast
        CheckGrounded();
        
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
        float verticalVelocity = rb.linearVelocity.y; // Preserve current Y velocity
        
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
        if (count >= 12)
        {
            // Display the win text.
            if (winTextObject != null)
            {
                winTextObject.SetActive(true);
            }

            // Destroy the enemy GameObject.
            GameObject enemy = GameObject.FindGameObjectWithTag("Enemy");
            if (enemy != null)
            {
                Destroy(enemy);
            }
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
}