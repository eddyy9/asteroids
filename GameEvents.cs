using UnityEngine;
using System;

public enum HeartState
{
    Full,
    Half,
    Empty
}

public enum DamageType
{
    Half,
    Full
}

public static class GameEvents
{
    public static event Action OnPlayerDied;
    public static event Action<Vector3> OnAsteroidDestroyed;
    public static event Action<int> OnScoreChanged;
    public static event Action<float> OnLivesChanged;
    public static event Action OnGameOver;
    
    public static event Action<float, float> OnHealthChanged;
    public static event Action<DamageType> OnPlayerTookDamage;

    public static event Action<int, int> OnResourcesChanged;
    public static event Action<int> OnResearchProgressChanged;
    public static event Action OnDashAbilityUnlocked;
    
    public static void PlayerDied() => OnPlayerDied?.Invoke();
    public static void AsteroidDestroyed(Vector3 position) => OnAsteroidDestroyed?.Invoke(position);
    public static void ScoreChanged(int newScore) => OnScoreChanged?.Invoke(newScore);
    public static void LivesChanged(float newLives) => OnLivesChanged?.Invoke(newLives);
    public static void GameOver() => OnGameOver?.Invoke();
    public static void HealthChanged(float current, float max) => OnHealthChanged?.Invoke(current, max);
    public static void PlayerTookDamage(DamageType type) => OnPlayerTookDamage?.Invoke(type);
    
    public static void ResourcesChanged(int current, int required) => OnResourcesChanged?.Invoke(current, required);
    public static void ResearchProgressChanged(int count) => OnResearchProgressChanged?.Invoke(count);
    public static void DashAbilityUnlocked() => OnDashAbilityUnlocked?.Invoke();
    
}
