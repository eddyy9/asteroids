using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private TopDownShipController player;
    [SerializeField] private float startingLives = 3f;
    [SerializeField] private float respawnDelay = 1f;
    [SerializeField] private float immunityDuration = 2f;
    
    private float currentLives;

    // Добавляем публичное свойство для доступа к игроку
    public TopDownShipController CurrentPlayer => player;

    private void Start()
    {
        currentLives = startingLives;
        GameEvents.LivesChanged(currentLives);

        if (player == null)
        {
            player = FindObjectOfType<TopDownShipController>(true);
        }
    }

    private void OnEnable()
    {
        GameEvents.OnPlayerDied += HandlePlayerDeath;
    }
    
    private void OnDisable()
    {
        GameEvents.OnPlayerDied -= HandlePlayerDeath;
    }

    private void HandlePlayerDeath()
    {
        currentLives--;
        GameEvents.LivesChanged(currentLives);

        if (currentLives <= 0)
        {
            GameEvents.GameOver();
        }
        else
        {
            StartCoroutine(RespawnCorountine());
        }
    }

    private IEnumerator RespawnCorountine()
    {
        yield return new WaitForSeconds(respawnDelay);
        RespawnPlayer();
    }

    private void RespawnPlayer()
    {
        player.transform.position = Vector3.zero;
        player.gameObject.SetActive(true);
        player.ResetShip();

        player.SetImmune(true, immunityDuration);
    }
}
