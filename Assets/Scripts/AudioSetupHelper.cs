using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class AudioSetupHelper : MonoBehaviour
{
    [ContextMenu("Setup Background Music")]
    public void SetupBackgroundMusic()
    {
        GameObject bgMusicObj = GameObject.Find("BackgroundMusic");
        if (bgMusicObj != null)
        {
            BackgroundMusic bgMusic = bgMusicObj.GetComponent<BackgroundMusic>();
            if (bgMusic != null)
            {
#if UNITY_EDITOR
                AudioClip music = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Lo-Fi Chillout Music For Games/Lo-Fi Music/1Dreams Of Her Best Friend.wav");
                if (music != null)
                {
                    bgMusic.backgroundMusic = music;
                    EditorUtility.SetDirty(bgMusic);
                    Debug.Log("✓ Background music assigned: " + music.name);
                }
                else
                {
                    Debug.LogWarning("Could not find background music file.");
                }
#endif
            }
        }
    }
    
    [ContextMenu("Setup Pickup Sounds")]
    public void SetupPickupSounds()
    {
        GameObject[] pickups = GameObject.FindGameObjectsWithTag("PickUp");
        
#if UNITY_EDITOR
        AudioClip pickupSound = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/HintsStarsLite/Positive Vibes Coin.wav");
        
        if (pickupSound == null)
        {
            Debug.LogWarning("Could not find pickup sound file.");
            return;
        }
        
        int count = 0;
        foreach (GameObject pickup in pickups)
        {
            PickupSound ps = pickup.GetComponent<PickupSound>();
            if (ps == null)
            {
                ps = pickup.AddComponent<PickupSound>();
            }
            
            ps.pickupSound = pickupSound;
            EditorUtility.SetDirty(pickup);
            count++;
        }
        
        Debug.Log($"✓ Setup complete! Added pickup sounds to {count} pickup objects.");
#endif
    }
    
    [ContextMenu("Setup All Audio")]
    public void SetupAllAudio()
    {
        SetupBackgroundMusic();
        SetupPickupSounds();
    }
}

