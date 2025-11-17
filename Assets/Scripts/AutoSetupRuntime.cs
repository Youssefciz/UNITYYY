using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class AutoSetupRuntime : MonoBehaviour
{
    void Start()
    {
        SetupEverything();
    }
    
    void SetupEverything()
    {
        // Setup Camera Follow
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            CameraFollow cf = mainCam.GetComponent<CameraFollow>();
            if (cf == null)
            {
                cf = mainCam.gameObject.AddComponent<CameraFollow>();
            }
            
            if (cf.player == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    cf.player = player.transform;
                }
            }
        }
        
        // Setup Background Music
        GameObject bgMusicObj = GameObject.Find("BackgroundMusic");
        if (bgMusicObj != null)
        {
            BackgroundMusic bgMusic = bgMusicObj.GetComponent<BackgroundMusic>();
            if (bgMusic != null && bgMusic.backgroundMusic == null)
            {
#if UNITY_EDITOR
                AudioClip music = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Lo-Fi Chillout Music For Games/Lo-Fi Music/1Dreams Of Her Best Friend.wav");
                if (music != null)
                {
                    bgMusic.backgroundMusic = music;
                    EditorUtility.SetDirty(bgMusic);
                }
#endif
            }
        }
        
        // Setup Pickup Sounds
        GameObject[] pickups = GameObject.FindGameObjectsWithTag("PickUp");
#if UNITY_EDITOR
        AudioClip pickupSound = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/HintsStarsLite/Positive Vibes Coin.wav");
        
        if (pickupSound != null)
        {
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
                }
            }
        }
#endif
    }
}

