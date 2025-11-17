using UnityEngine;
using TMPro;

public class WinTextFontFixer : MonoBehaviour
{
    void Start()
    {
        // Find WinText and assign default font if missing
        TextMeshProUGUI winText = GetComponent<TextMeshProUGUI>();
        if (winText != null && winText.font == null)
        {
            // Load the default font from TextMesh Pro resources
            TMP_FontAsset defaultFont = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF - Fallback");
            if (defaultFont != null)
            {
                winText.font = defaultFont;
                Debug.Log("Font assigned to WinText");
            }
            else
            {
                // Try to get from TMP Settings (static property)
                if (TMP_Settings.defaultFontAsset != null)
                {
                    winText.font = TMP_Settings.defaultFontAsset;
                    Debug.Log("Default font from TMP Settings assigned to WinText");
                }
            }
        }
    }
}
