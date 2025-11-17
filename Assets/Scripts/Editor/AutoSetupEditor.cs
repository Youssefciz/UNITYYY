using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public class AutoSetupEditor
{
    static AutoSetupEditor()
    {
        EditorApplication.delayCall += RunAutoSetup;
    }
    
    [MenuItem("Tools/Auto Setup Camera and Audio")]
    public static void RunAutoSetup()
    {
        // Only run in the "ok" scene
        if (SceneManager.GetActiveScene().name != "ok")
        {
            Debug.Log("AutoSetup: Not in 'ok' scene, skipping setup.");
            return;
        }
        
        Debug.Log("AutoSetup: Starting automatic setup...");
            
        // Setup Camera
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            CameraFollow cf = mainCam.GetComponent<CameraFollow>();
            if (cf == null)
            {
                cf = mainCam.gameObject.AddComponent<CameraFollow>();
            }
            
            // Always set player reference if it's null
            if (cf.player == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    cf.player = player.transform;
                    Debug.Log("AutoSetup: Camera player reference set.");
                }
                else
                {
                    Debug.LogWarning("AutoSetup: Could not find Player GameObject!");
                }
            }
            EditorUtility.SetDirty(mainCam);
        }
        
        // Setup Background Music
        GameObject bgMusicObj = GameObject.Find("BackgroundMusic");
        if (bgMusicObj == null)
        {
            bgMusicObj = new GameObject("BackgroundMusic");
            bgMusicObj.AddComponent<BackgroundMusic>();
            bgMusicObj.AddComponent<AudioSource>();
        }
        
        BackgroundMusic bgMusic = bgMusicObj.GetComponent<BackgroundMusic>();
        if (bgMusic != null)
        {
            if (bgMusic.backgroundMusic == null)
            {
                AudioClip music = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Lo-Fi Chillout Music For Games/Lo-Fi Music/1Dreams Of Her Best Friend.wav");
                if (music != null)
                {
                    bgMusic.backgroundMusic = music;
                    EditorUtility.SetDirty(bgMusic);
                    Debug.Log("AutoSetup: Background music assigned: " + music.name);
                }
                else
                {
                    Debug.LogWarning("AutoSetup: Could not find background music file!");
                }
            }
        }
        
        // Setup Pickup Sounds
        GameObject[] pickups = GameObject.FindGameObjectsWithTag("PickUp");
        Debug.Log($"AutoSetup: Found {pickups.Length} pickup objects.");
        
        AudioClip pickupSound = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/HintsStarsLite/Positive Vibes Coin.wav");
        
        if (pickupSound != null)
        {
            int setupCount = 0;
            foreach (GameObject pickup in pickups)
            {
                PickupSound ps = pickup.GetComponent<PickupSound>();
                if (ps == null)
                {
                    ps = pickup.AddComponent<PickupSound>();
                }
                
                if (ps.pickupSound == null)
                {
                    ps.pickupSound = pickupSound;
                    EditorUtility.SetDirty(pickup);
                    setupCount++;
                }
            }
            Debug.Log($"AutoSetup: Setup {setupCount} pickup sounds.");
        }
        else
        {
            Debug.LogWarning("AutoSetup: Could not find pickup sound file!");
        }
        
        Debug.Log("AutoSetup: Complete!");
    }
}

