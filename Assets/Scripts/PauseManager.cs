using UnityEngine;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    private bool isPaused = false;
    
    // Optional: Reference to pause menu UI
    public GameObject pauseMenuUI;
    
    // Optional: Reference to PlayerInput component (if using Input System)
    private PlayerInput playerInput;
    
    void Start()
    {
        // Try to get PlayerInput component
        playerInput = GetComponent<PlayerInput>();
        
        // Ensure pause menu is hidden at start
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }
    }
    
    void Update()
    {
        // Use unscaled time to check input even when paused
        // Backup pause input using old Input System (Escape key)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }
    
    // Called by Input System when Pause action is triggered
    public void OnPause(InputValue pauseValue)
    {
        if (pauseValue.isPressed)
        {
            TogglePause();
        }
    }
    
    public void TogglePause()
    {
        isPaused = !isPaused;
        
        if (isPaused)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }
    
    public void PauseGame()
    {
        Time.timeScale = 0f; // Pause the game
        isPaused = true;
        
        // Show pause menu if available
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(true);
        }
    }
    
    public void ResumeGame()
    {
        Time.timeScale = 1f; // Resume the game
        isPaused = false;
        
        // Hide pause menu if available
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }
    }
    
    public bool IsPaused()
    {
        return isPaused;
    }
}
