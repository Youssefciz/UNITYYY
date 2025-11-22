using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Manages player lives, handles enemy collisions, invincibility periods, and game over state.
/// </summary>
public class PlayerLives : MonoBehaviour
{
    [Header("Life Settings")]
    [Tooltip("Maximum number of lives the player starts with")]
    public int maxLives = 3;
    
    [Header("Respawn Settings")]
    [Tooltip("Optional spawn point transform. If not set, uses player's initial position")]
    public Transform spawnPoint;
    
    [Header("Invincibility Settings")]
    [Tooltip("Duration of invincibility period after taking damage (in seconds)")]
    public float invincibilityDuration = 2f;
    
    [Header("UI References")]
    [Tooltip("Text component to display current lives count")]
    public TextMeshProUGUI livesText;
    
    [Tooltip("Text component to display Game Over message")]
    public TextMeshProUGUI gameOverText;
    
    // Current number of lives
    private int currentLives;
    
    // Flag to track if player is currently invincible
    private bool isInvincible = false;
    
    // Reference to PlayerController to disable movement on game over
    private PlayerController playerController;
    
    // Store initial position as fallback spawn point
    private Vector3 initialSpawnPosition;
    private Quaternion initialSpawnRotation;
    
    // Reference to Rigidbody for resetting velocity
    private Rigidbody rb;
    
    // Flag to track if game is over
    private bool isGameOver = false;

    void Start()
    {
        Debug.Log("=== PlayerLives: Start() called ===");
        
        // Initialize lives to max
        currentLives = maxLives;
        Debug.Log("PlayerLives: Initialized with " + currentLives + " lives (max: " + maxLives + ")");
        
        // Store initial position and rotation as fallback spawn point
        initialSpawnPosition = transform.position;
        initialSpawnRotation = transform.rotation;
        
        // Get references to components
        playerController = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody>();
        
        Debug.Log("PlayerLives: Component references - PlayerController: " + (playerController != null ? "FOUND" : "NULL") + 
                 ", Rigidbody: " + (rb != null ? "FOUND" : "NULL"));
        
        // Try to find UI elements if not set
        if (livesText == null)
        {
            GameObject livesTextObj = GameObject.Find("LivesText");
            if (livesTextObj != null)
            {
                livesText = livesTextObj.GetComponent<TextMeshProUGUI>();
                Debug.Log("PlayerLives: Found LivesText via GameObject.Find");
            }
        }
        
        if (gameOverText == null)
        {
            GameObject gameOverTextObj = GameObject.Find("GameOverText");
            if (gameOverTextObj != null)
            {
                gameOverText = gameOverTextObj.GetComponent<TextMeshProUGUI>();
                Debug.Log("PlayerLives: Found GameOverText via GameObject.Find");
            }
        }
        
        Debug.Log("PlayerLives: UI references - livesText: " + (livesText != null ? "SET" : "NULL") + 
                 ", gameOverText: " + (gameOverText != null ? "SET" : "NULL"));
        
        // Update UI
        UpdateLivesUI();
        
        // Ensure Game Over text is hidden initially
        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(false);
            Debug.Log("PlayerLives: GameOverText hidden on Start");
        }
        else
        {
            Debug.LogWarning("PlayerLives: gameOverText is NULL! UI may not work correctly.");
        }
        
        Debug.Log("=== PlayerLives: Start() complete ===");
    }

    void Update()
    {
        // Check for restart input when game is over
        if (isGameOver && Input.GetKeyDown(KeyCode.R))
        {
            ReloadScene();
        }
    }

    /// <summary>
    /// Handles collision with enemy objects.
    /// </summary>
    void OnCollisionEnter(Collision collision)
    {
        // Check if collided with an enemy and not invincible
        if (collision.gameObject.CompareTag("Enemy") && !isInvincible && !isGameOver)
        {
            TakeDamage();
        }
    }

    /// <summary>
    /// Handles trigger collision with enemy objects (if enemies use triggers).
    /// </summary>
    void OnTriggerEnter(Collider other)
    {
        // Check if triggered with an enemy and not invincible
        if (other.gameObject.CompareTag("Enemy") && !isInvincible && !isGameOver)
        {
            TakeDamage();
        }
    }

    /// <summary>
    /// Reduces player lives by 1 and handles respawn or game over.
    /// </summary>
    private void TakeDamage()
    {
        // Reduce lives
        currentLives--;
        
        // Update UI
        UpdateLivesUI();
        
        // Check if player has lives remaining
        if (currentLives > 0)
        {
            // Player still has lives - respawn and grant invincibility
            RespawnPlayer();
            StartCoroutine(InvincibilityCoroutine());
        }
        else
        {
            // No lives remaining - game over
            GameOver();
        }
    }

    /// <summary>
    /// Respawns the player at the spawn point or initial position.
    /// Prioritizes using PlayerController's Respawn method which handles RespawnPoint checkpoints.
    /// </summary>
    private void RespawnPlayer()
    {
        // If PlayerController has a Respawn method, use it (it handles RespawnPoint checkpoints)
        if (playerController != null)
        {
            // Use reflection to call Respawn if it exists (to avoid modifying PlayerController)
            var respawnMethod = playerController.GetType().GetMethod("Respawn");
            if (respawnMethod != null)
            {
                respawnMethod.Invoke(playerController, null);
                return; // PlayerController.Respawn handles everything
            }
        }
        
        // Fallback: Manual respawn if PlayerController.Respawn is not available
        Vector3 respawnPosition;
        Quaternion respawnRotation;
        
        // Use spawn point if set, otherwise use initial position
        if (spawnPoint != null)
        {
            respawnPosition = spawnPoint.position;
            respawnRotation = spawnPoint.rotation;
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
    }

    /// <summary>
    /// Coroutine that handles the invincibility period.
    /// </summary>
    private System.Collections.IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;
        
        // Wait for invincibility duration
        yield return new WaitForSeconds(invincibilityDuration);
        
        isInvincible = false;
    }

    /// <summary>
    /// Handles game over state - disables player movement and shows game over text.
    /// </summary>
    private void GameOver()
    {
        Debug.Log("=== PlayerLives: GameOver() called ===");
        isGameOver = true;
        
        // Disable player movement
        if (playerController != null)
        {
            playerController.enabled = false;
            Debug.Log("PlayerLives: Disabled PlayerController");
        }
        else
        {
            Debug.LogWarning("PlayerLives: PlayerController is NULL, cannot disable movement!");
        }
        
        // Show game over text
        if (gameOverText != null)
        {
            gameOverText.text = "Game Over\nPress R to Restart";
            gameOverText.gameObject.SetActive(true);
            Debug.Log("PlayerLives: GameOverText activated and set to: " + gameOverText.text);
        }
        else
        {
            Debug.LogError("PlayerLives: gameOverText is NULL! Cannot show Game Over message!");
        }
        
        Debug.Log("=== PlayerLives: Game Over! All lives lost. ===");
    }

    /// <summary>
    /// Updates the lives display UI.
    /// </summary>
    private void UpdateLivesUI()
    {
        if (livesText != null)
        {
            string newText = "Lives: " + currentLives.ToString();
            livesText.text = newText;
            Debug.Log("PlayerLives: UpdateLivesUI() called - Set text to: " + newText);
            
            // Ensure the text object is active
            if (!livesText.gameObject.activeInHierarchy)
            {
                livesText.gameObject.SetActive(true);
                Debug.LogWarning("PlayerLives: LivesText was inactive, activating it now");
            }
        }
        else
        {
            Debug.LogWarning("PlayerLives: UpdateLivesUI() called but livesText is NULL!");
        }
    }

    /// <summary>
    /// Reloads the current scene to restart the level.
    /// </summary>
    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Public method to check if player is currently invincible (useful for visual feedback).
    /// </summary>
    public bool IsInvincible()
    {
        return isInvincible;
    }

    /// <summary>
    /// Public method to check if game is over.
    /// </summary>
    public bool IsGameOver()
    {
        return isGameOver;
    }
}
