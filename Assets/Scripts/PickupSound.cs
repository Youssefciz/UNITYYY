using UnityEngine;

public class PickupSound : MonoBehaviour
{
    [Tooltip("Sound to play when this pickup is collected")]
    public AudioClip pickupSound;
    
    [Tooltip("Volume for pickup sound (0.0 to 1.0)")]
    [Range(0f, 1f)]
    public float volume = 0.7f;
    
    private bool hasPlayed = false;
    
    // Called when the pickup is collected
    public void PlayPickupSound()
    {
        if (pickupSound != null && !hasPlayed)
        {
            // Use PlayClipAtPoint so the sound plays even if the GameObject is about to be destroyed
            AudioSource.PlayClipAtPoint(pickupSound, transform.position, volume);
            hasPlayed = true;
        }
    }
}


