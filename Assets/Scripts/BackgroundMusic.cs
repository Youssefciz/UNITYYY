using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    [Tooltip("The background music audio clip")]
    public AudioClip backgroundMusic;
    
    [Tooltip("Volume for background music (0.0 to 1.0)")]
    [Range(0f, 1f)]
    public float volume = 0.5f;
    
    [Tooltip("Should the music loop?")]
    public bool loop = true;
    
    private AudioSource audioSource;
    
    void Start()
    {
        // Get or add AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // Configure AudioSource
        audioSource.clip = backgroundMusic;
        audioSource.volume = volume;
        audioSource.loop = loop;
        audioSource.playOnAwake = true;
        
        // Play the music
        if (backgroundMusic != null)
        {
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("BackgroundMusic: No audio clip assigned! Please assign a music clip in the Inspector.");
        }
    }
    
    // Method to change music at runtime
    public void ChangeMusic(AudioClip newMusic)
    {
        if (audioSource != null && newMusic != null)
        {
            audioSource.Stop();
            audioSource.clip = newMusic;
            audioSource.Play();
        }
    }
    
    // Method to stop music
    public void StopMusic()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }
    
    // Method to pause/resume music
    public void PauseMusic()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Pause();
        }
    }
    
    public void ResumeMusic()
    {
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.UnPause();
        }
    }
}
