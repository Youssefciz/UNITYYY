using UnityEngine;
using TMPro;

/// <summary>
/// Ensures LivesText and GameOverText have font assets assigned.
/// Similar to WinTextFontFixer, this runs on Start to fix any missing fonts.
/// </summary>
public class LivesTextFontFixer : MonoBehaviour
{
    void Start()
    {
        // Find LivesText and assign default font if missing
        GameObject livesTextObj = GameObject.Find("LivesText");
        if (livesTextObj != null)
        {
            TextMeshProUGUI livesText = livesTextObj.GetComponent<TextMeshProUGUI>();
            if (livesText != null && livesText.font == null)
            {
                AssignFont(livesText, "LivesText");
            }
        }

        // Find GameOverText and assign default font if missing
        GameObject gameOverTextObj = GameObject.Find("GameOverText");
        if (gameOverTextObj != null)
        {
            TextMeshProUGUI gameOverText = gameOverTextObj.GetComponent<TextMeshProUGUI>();
            if (gameOverText != null && gameOverText.font == null)
            {
                AssignFont(gameOverText, "GameOverText");
            }
        }
    }

    private void AssignFont(TextMeshProUGUI textComponent, string componentName)
    {
        TMP_FontAsset defaultFont = null;
        
        // First try: Get from TMP Settings (static property)
        if (TMP_Settings.defaultFontAsset != null)
        {
            defaultFont = TMP_Settings.defaultFontAsset;
            Debug.Log("LivesTextFontFixer: Got default font from TMP_Settings.defaultFontAsset for " + componentName);
        }
        else
        {
            // Fallback: Try to load from Resources
            defaultFont = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF - Fallback");
            if (defaultFont != null)
            {
                Debug.Log("LivesTextFontFixer: Loaded font from Resources for " + componentName);
            }
            else
            {
                // Last resort: Find any TMP font asset in the project
                TMP_FontAsset[] fonts = Resources.FindObjectsOfTypeAll<TMP_FontAsset>();
                if (fonts != null && fonts.Length > 0)
                {
                    defaultFont = fonts[0];
                    Debug.Log("LivesTextFontFixer: Found font asset in project for " + componentName);
                }
            }
        }
        
        if (defaultFont != null)
        {
            textComponent.font = defaultFont;
            Debug.Log("LivesTextFontFixer: Successfully assigned font to " + componentName + ": " + defaultFont.name);
        }
        else
        {
            Debug.LogError("LivesTextFontFixer: CRITICAL - Could not find any TMP font asset for " + componentName + "! Text will not display.");
        }
    }
}
