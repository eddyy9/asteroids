using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private int pointsPerAsteroid = 10;
    
    private int currentScore;

    private void Start()
    {
        currentScore = 0;
        GameEvents.ScoreChanged(currentScore);
    }

    private void OnEnable()
    {
        // Исправлено имя метода
        GameEvents.OnAsteroidDestroyed += HandleAsteroidDestroyed;
    }
    
    private void OnDisable()
    {
        // Исправлено имя метода
        GameEvents.OnAsteroidDestroyed -= HandleAsteroidDestroyed;
    }

    // Исправлено имя метода (убрана опечатка)
    private void HandleAsteroidDestroyed(Vector3 position)
    {
        currentScore += pointsPerAsteroid;
        GameEvents.ScoreChanged(currentScore);
    }
}
