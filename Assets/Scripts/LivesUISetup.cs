using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Runtime script that automatically sets up the lives UI system on scene load.
/// This ensures Canvas, LivesText, and GameOverText exist and are properly configured.
/// </summary>
public class LivesUISetup : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void SetupUI()
    {
        Debug.Log("=== LivesUISetup: Starting UI setup ===");
        
        // Ensure EventSystem exists (required for UI)
        if (FindFirstObjectByType<EventSystem>() == null)
        {
            GameObject eventSystemObj = new GameObject("EventSystem");
            eventSystemObj.AddComponent<EventSystem>();
            eventSystemObj.AddComponent<StandaloneInputModule>();
            Debug.Log("LivesUISetup: Created EventSystem");
        }
        else
        {
            Debug.Log("LivesUISetup: EventSystem already exists");
        }

        // Find or create Canvas
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("Canvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            canvasObj.SetActive(true);
            Debug.Log("LivesUISetup: Created Canvas with Screen Space Overlay mode");
        }
        else
        {
            // Ensure Canvas is properly configured
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.gameObject.SetActive(true);
            Debug.Log("LivesUISetup: Found existing Canvas, ensuring it's configured correctly");
        }

        // Ensure Canvas is active
        if (!canvas.gameObject.activeInHierarchy)
        {
            canvas.gameObject.SetActive(true);
            Debug.LogWarning("LivesUISetup: Canvas was inactive, activating it now");
        }

        // Attach font fixer to Canvas to ensure fonts are assigned (runs on Start)
        if (canvas.GetComponent<LivesTextFontFixer>() == null)
        {
            canvas.gameObject.AddComponent<LivesTextFontFixer>();
            Debug.Log("LivesUISetup: Added LivesTextFontFixer component to Canvas");
        }

        // Find or create LivesText
        TextMeshProUGUI livesText = GameObject.Find("LivesText")?.GetComponent<TextMeshProUGUI>();
        if (livesText == null)
        {
            GameObject livesTextObj = new GameObject("LivesText");
            livesTextObj.transform.SetParent(canvas.transform, false);
            livesText = livesTextObj.AddComponent<TextMeshProUGUI>();
            
            // Set text properties
            livesText.text = "Lives: 3";
            livesText.fontSize = 36;
            livesText.color = Color.white;
            livesText.fontStyle = FontStyles.Bold;
            
            // Try to get default TMP font from TMP Settings (using static property like WinTextFontFixer)
            TMP_FontAsset defaultFont = null;
            
            // First try: Get from TMP Settings static property
            if (TMP_Settings.defaultFontAsset != null)
            {
                defaultFont = TMP_Settings.defaultFontAsset;
                Debug.Log("LivesUISetup: Got default font from TMP_Settings.defaultFontAsset: " + defaultFont.name);
            }
            else
            {
                // Fallback: Try to load from Resources
                defaultFont = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF - Fallback");
                if (defaultFont != null)
                {
                    Debug.Log("LivesUISetup: Loaded font from Resources: " + defaultFont.name);
                }
                else
                {
                    // Last resort: Find any TMP font asset in the project
                    TMP_FontAsset[] fonts = Resources.FindObjectsOfTypeAll<TMP_FontAsset>();
                    if (fonts != null && fonts.Length > 0)
                    {
                        defaultFont = fonts[0];
                        Debug.Log("LivesUISetup: Found font asset in project: " + defaultFont.name);
                    }
                }
            }
            
            if (defaultFont != null)
            {
                livesText.font = defaultFont;
                Debug.Log("LivesUISetup: Successfully assigned font to LivesText: " + defaultFont.name);
            }
            else
            {
                Debug.LogError("LivesUISetup: CRITICAL - Could not find any TMP font asset! LivesText will not display. Please import TextMeshPro Essentials (Window > TextMeshPro > Import TMP Essential Resources).");
            }
            
            // Position in top-right corner with padding
            RectTransform rectTransform = livesTextObj.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(1, 1);
            rectTransform.anchorMax = new Vector2(1, 1);
            rectTransform.pivot = new Vector2(1, 1);
            rectTransform.anchoredPosition = new Vector2(-20, -20); // 20 pixels from top and right edges
            rectTransform.sizeDelta = new Vector2(200, 50);
            
            livesTextObj.SetActive(true);
            Debug.Log("LivesUISetup: Created LivesText in top-right corner. Text: " + livesText.text);
        }
        else
        {
            // Update existing LivesText position to top-right if it's not already there
            RectTransform rectTransform = livesText.GetComponent<RectTransform>();
            if (rectTransform.anchorMin.x < 0.9f) // If not already anchored to right
            {
                rectTransform.anchorMin = new Vector2(1, 1);
                rectTransform.anchorMax = new Vector2(1, 1);
                rectTransform.pivot = new Vector2(1, 1);
                rectTransform.anchoredPosition = new Vector2(-20, -20);
                Debug.Log("LivesUISetup: Updated existing LivesText position to top-right");
            }
            livesText.gameObject.SetActive(true);
            Debug.Log("LivesUISetup: Found existing LivesText. Current text: " + livesText.text);
        }

        // Find or create GameOverText
        TextMeshProUGUI gameOverText = GameObject.Find("GameOverText")?.GetComponent<TextMeshProUGUI>();
        if (gameOverText == null)
        {
            GameObject gameOverTextObj = new GameObject("GameOverText");
            gameOverTextObj.transform.SetParent(canvas.transform, false);
            gameOverText = gameOverTextObj.AddComponent<TextMeshProUGUI>();
            
            // Set text properties
            gameOverText.text = "Game Over\nPress R to Restart";
            gameOverText.fontSize = 48;
            gameOverText.color = Color.red;
            gameOverText.alignment = TextAlignmentOptions.Center;
            gameOverText.fontStyle = FontStyles.Bold;
            
            // Try to get default TMP font from TMP Settings (using static property like WinTextFontFixer)
            TMP_FontAsset defaultFont = null;
            
            // First try: Get from TMP Settings static property
            if (TMP_Settings.defaultFontAsset != null)
            {
                defaultFont = TMP_Settings.defaultFontAsset;
                Debug.Log("LivesUISetup: Got default font from TMP_Settings.defaultFontAsset: " + defaultFont.name);
            }
            else
            {
                // Fallback: Try to load from Resources
                defaultFont = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF - Fallback");
                if (defaultFont != null)
                {
                    Debug.Log("LivesUISetup: Loaded font from Resources: " + defaultFont.name);
                }
                else
                {
                    // Last resort: Find any TMP font asset in the project
                    TMP_FontAsset[] fonts = Resources.FindObjectsOfTypeAll<TMP_FontAsset>();
                    if (fonts != null && fonts.Length > 0)
                    {
                        defaultFont = fonts[0];
                        Debug.Log("LivesUISetup: Found font asset in project: " + defaultFont.name);
                    }
                }
            }
            
            if (defaultFont != null)
            {
                gameOverText.font = defaultFont;
                Debug.Log("LivesUISetup: Successfully assigned font to GameOverText: " + defaultFont.name);
            }
            else
            {
                Debug.LogError("LivesUISetup: CRITICAL - Could not find any TMP font asset! GameOverText will not display. Please import TextMeshPro Essentials (Window > TextMeshPro > Import TMP Essential Resources).");
            }
            
            // Position in center of screen
            RectTransform rectTransform = gameOverTextObj.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = new Vector2(400, 150);
            
            // Initially disable
            gameOverTextObj.SetActive(false);
            
            Debug.Log("LivesUISetup: Created GameOverText (initially hidden)");
        }
        else
        {
            gameOverText.gameObject.SetActive(false);
            Debug.Log("LivesUISetup: Found existing GameOverText, ensuring it's hidden");
        }

        // Find Player and set references
        GameObject player = GameObject.Find("Player");
        if (player != null)
        {
            PlayerLives playerLives = player.GetComponent<PlayerLives>();
            if (playerLives == null)
            {
                playerLives = player.AddComponent<PlayerLives>();
                Debug.Log("LivesUISetup: Added PlayerLives component to Player");
            }
            else
            {
                Debug.Log("LivesUISetup: Found existing PlayerLives component on Player");
            }
            
            // Always set references (in case they were null)
            playerLives.livesText = livesText;
            playerLives.gameOverText = gameOverText;
            
            Debug.Log("LivesUISetup: Set UI references in PlayerLives - livesText: " + (livesText != null ? "SET" : "NULL") + 
                     ", gameOverText: " + (gameOverText != null ? "SET" : "NULL"));
        }
        else
        {
            Debug.LogError("LivesUISetup: Player GameObject not found! Please ensure a GameObject named 'Player' exists in the scene.");
        }

        Debug.Log("=== LivesUISetup: UI setup complete! ===");
    }
}
