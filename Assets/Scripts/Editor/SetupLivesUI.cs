using UnityEngine;
using UnityEditor;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Editor script to set up the lives UI system.
/// Creates Canvas, LivesText, and GameOverText if they don't exist.
/// </summary>
public class SetupLivesUI : EditorWindow
{
    [MenuItem("Tools/Setup Lives UI")]
    public static void SetupUI()
    {
        // Find or create Canvas
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("Canvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            Debug.Log("Created Canvas");
        }

        // Find or create LivesText
        TextMeshProUGUI livesText = GameObject.Find("LivesText")?.GetComponent<TextMeshProUGUI>();
        if (livesText == null)
        {
            GameObject livesTextObj = new GameObject("LivesText");
            livesTextObj.transform.SetParent(canvas.transform, false);
            livesText = livesTextObj.AddComponent<TextMeshProUGUI>();
            livesText.text = "Lives: 3";
            livesText.fontSize = 36;
            livesText.color = Color.white;
            
            // Position in top-left corner
            RectTransform rectTransform = livesTextObj.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0, 1);
            rectTransform.anchorMax = new Vector2(0, 1);
            rectTransform.pivot = new Vector2(0, 1);
            rectTransform.anchoredPosition = new Vector2(20, -20);
            rectTransform.sizeDelta = new Vector2(200, 50);
            
            Debug.Log("Created LivesText");
        }

        // Find or create GameOverText
        TextMeshProUGUI gameOverText = GameObject.Find("GameOverText")?.GetComponent<TextMeshProUGUI>();
        if (gameOverText == null)
        {
            GameObject gameOverTextObj = new GameObject("GameOverText");
            gameOverTextObj.transform.SetParent(canvas.transform, false);
            gameOverText = gameOverTextObj.AddComponent<TextMeshProUGUI>();
            gameOverText.text = "Game Over\nPress R to Restart";
            gameOverText.fontSize = 48;
            gameOverText.color = Color.red;
            gameOverText.alignment = TextAlignmentOptions.Center;
            
            // Position in center of screen
            RectTransform rectTransform = gameOverTextObj.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = new Vector2(400, 150);
            
            // Initially disable
            gameOverTextObj.SetActive(false);
            
            Debug.Log("Created GameOverText");
        }

        // Find Player and add PlayerLives component if not present
        GameObject player = GameObject.Find("Player");
        if (player != null)
        {
            PlayerLives playerLives = player.GetComponent<PlayerLives>();
            if (playerLives == null)
            {
                playerLives = player.AddComponent<PlayerLives>();
                Debug.Log("Added PlayerLives component to Player");
            }
            
            // Set references
            playerLives.livesText = livesText;
            playerLives.gameOverText = gameOverText;
            
            EditorUtility.SetDirty(playerLives);
            Debug.Log("Set UI references in PlayerLives");
        }
        else
        {
            Debug.LogWarning("Player GameObject not found! Please add PlayerLives component manually.");
        }

        Debug.Log("Lives UI setup complete!");
    }
}
