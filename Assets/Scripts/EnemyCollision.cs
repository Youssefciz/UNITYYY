using UnityEngine;

/// <summary>
/// Handles enemy-side collision detection with the player.
/// Note: PlayerLives handles all collision detection from the player side,
/// so this script is now redundant and can be safely removed from enemy GameObjects.
/// </summary>
public class EnemyCollision : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Try to get PlayerLives component first (new system)
            PlayerLives playerLives = collision.gameObject.GetComponent<PlayerLives>();
            if (playerLives != null && !playerLives.IsGameOver())
            {
                // PlayerLives handles the collision detection itself via OnCollisionEnter,
                // so we don't need to do anything here. The collision will be handled
                // by PlayerLives.OnCollisionEnter on the player side.
                // This script can be removed if not needed, as PlayerLives handles everything.
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // Try to get PlayerLives component first (new system)
            PlayerLives playerLives = other.gameObject.GetComponent<PlayerLives>();
            if (playerLives != null && !playerLives.IsGameOver())
            {
                // PlayerLives handles the collision detection itself via OnTriggerEnter,
                // so we don't need to do anything here. The collision will be handled
                // by PlayerLives.OnTriggerEnter on the player side.
                // This script can be removed if not needed, as PlayerLives handles everything.
            }
        }
    }
}
