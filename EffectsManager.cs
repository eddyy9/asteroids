using UnityEngine;

public class EffectsManager : MonoBehaviour
{
    [SerializeField] private ParticleSystem explosion;

    private void OnEnable()
    {
        GameEvents.OnPlayerDied += HandlePlayerDeath;
        GameEvents.OnAsteroidDestroyed += HandleAsteroidDestroyed;
    }
    
    private void OnDisable()
    {
        GameEvents.OnPlayerDied -= HandlePlayerDeath;
        GameEvents.OnAsteroidDestroyed -= HandleAsteroidDestroyed;
    }
    
    private void HandlePlayerDeath()
    {
        PlayerManager playerManager = FindObjectOfType<PlayerManager>();
        if (playerManager != null && playerManager.CurrentPlayer != null)
        {
            Vector3 position = playerManager.CurrentPlayer.transform.position;
            explosion.transform.position = position;
            explosion.Play();
        }
    }

    private void HandleAsteroidDestroyed(Vector3 position)
    {
        explosion.transform.position = position;
        explosion.Play();
    }
}
