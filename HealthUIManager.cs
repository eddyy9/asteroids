using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class HealthUIManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private GameObject heartPrefab;
    [SerializeField] private Transform heartsParent; // HealthPanel
    [SerializeField] private int maxHearts = 3;
    
    private List<HeartUI> hearts = new List<HeartUI>();
    private float currentHealth;
    private float maxHealth;
    
    private void Start()
    {
        CreateHearts();
    }
    
    private void OnEnable()
    {
        GameEvents.OnHealthChanged += UpdateHealthDisplay;
        GameEvents.OnPlayerTookDamage += HandlePlayerDamage;
    }
    
    private void OnDisable()
    {
        GameEvents.OnHealthChanged -= UpdateHealthDisplay;
        GameEvents.OnPlayerTookDamage -= HandlePlayerDamage;
    }
    
    private void CreateHearts()
    {
        foreach (var heart in hearts)
        {
            if (heart != null)
                Destroy(heart.gameObject);
        }
        hearts.Clear();
        
        for (int i = 0; i < maxHearts; i++)
        {
            GameObject newHeartObj = Instantiate(heartPrefab, heartsParent);
    
            HeartUI newHeart = newHeartObj.GetComponentInChildren<HeartUI>();
    
            if (newHeart != null)
            {
                newHeart.SetHeartState(HeartState.Full, false);
                hearts.Add(newHeart);
            }
            else
            {
                Debug.LogError("На префабе сердечка (внутри HeartVisual) нет скрипта HeartUI!");
            }
        }
        
        maxHealth = maxHearts * 2f; // 2 единицы здоровья на сердечко
        currentHealth = maxHealth;
    }
    
    private void UpdateHealthDisplay(float newCurrentHealth, float newMaxHealth)
    {
        currentHealth = newCurrentHealth;
        maxHealth = newMaxHealth;
        
        for (int i = 0; i < hearts.Count; i++)
        {
            float heartHealthStart = i * 2f; // Каждое сердечко отвечает за 2 единицы здоровья
            float heartHealthEnd = heartHealthStart + 2f;
            
            HeartState targetState;
            
            if (currentHealth > heartHealthStart + 1f)
            {
                targetState = HeartState.Full;
            }
            else if (currentHealth > heartHealthStart)
            {
                targetState = HeartState.Half;
            }
            else
            {
                targetState = HeartState.Empty;
            }
            
            hearts[i].SetHeartState(targetState, false);
        }
    }
    
    private void HandlePlayerDamage(DamageType damageType)
    {
        HeartUI targetHeart = FindHeartForDamage(damageType);
        
        if (targetHeart != null)
        {
            targetHeart.TakeDamage(damageType);
            
            RecalculateCurrentHealth();
        }
    }
    
    private HeartUI FindHeartForDamage(DamageType damageType)
    {
        for (int i = hearts.Count - 1; i >= 0; i--)
        {
            if (hearts[i].CanTakeDamage(damageType))
            {
                return hearts[i];
            }
        }
        return null;
    }
    
    private void RecalculateCurrentHealth()
    {
        float totalHealth = 0f;
        
        foreach (var heart in hearts)
        {
            switch (heart.CurrentState)
            {
                case HeartState.Full:
                    totalHealth += 2f;
                    break;
                case HeartState.Half:
                    totalHealth += 1f;
                    break;
                case HeartState.Empty:
                    totalHealth += 0f;
                    break;
            }
        }
        
        currentHealth = totalHealth;
        
        GameEvents.HealthChanged(currentHealth, maxHealth);
    }
    
    public bool HasHealth()
    {
        return hearts.Any(heart => heart.CurrentState != HeartState.Empty);
    }
    
    public void RestoreHealth(float amount)
    {
    }
    
    public void ResetHealth()
    {
        foreach (var heart in hearts)
        {
            heart.SetHeartState(HeartState.Full, false);
        }
        
        currentHealth = maxHealth;
        GameEvents.HealthChanged(currentHealth, maxHealth);
    }
}