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
    
    // Создает начальные сердечки
    private void CreateHearts()
    {
        // Очищаем существующие сердечки
        foreach (var heart in hearts)
        {
            if (heart != null)
                Destroy(heart.gameObject);
        }
        hearts.Clear();
        
        // Создаем новые сердечки
        for (int i = 0; i < maxHearts; i++)
        {
            // 1. Создаем сам объект из префаба (теперь это GameObject)
            GameObject newHeartObj = Instantiate(heartPrefab, heartsParent);
    
            // 2. Ищем скрипт не на самом объекте, а ВНУТРИ него (у детей)
            HeartUI newHeart = newHeartObj.GetComponentInChildren<HeartUI>();
    
            // Дальше все как было...
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
        
        // Инициализируем здоровье
        maxHealth = maxHearts * 2f; // 2 единицы здоровья на сердечко
        currentHealth = maxHealth;
    }
    
    // Обновляет отображение здоровья
    private void UpdateHealthDisplay(float newCurrentHealth, float newMaxHealth)
    {
        currentHealth = newCurrentHealth;
        maxHealth = newMaxHealth;
        
        // Пересчитываем состояние каждого сердечка
        for (int i = 0; i < hearts.Count; i++)
        {
            float heartHealthStart = i * 2f; // Каждое сердечко отвечает за 2 единицы здоровья
            float heartHealthEnd = heartHealthStart + 2f;
            
            HeartState targetState;
            
            if (currentHealth > heartHealthStart + 1f)
            {
                // Полное сердечко
                targetState = HeartState.Full;
            }
            else if (currentHealth > heartHealthStart)
            {
                // Половинчатое сердечко
                targetState = HeartState.Half;
            }
            else
            {
                // Пустое сердечко
                targetState = HeartState.Empty;
            }
            
            hearts[i].SetHeartState(targetState, false);
        }
    }
    
    // Обработка урона с анимацией
    private void HandlePlayerDamage(DamageType damageType)
    {
        // Находим последнее сердечко, которое может получить урон
        HeartUI targetHeart = FindHeartForDamage(damageType);
        
        if (targetHeart != null)
        {
            targetHeart.TakeDamage(damageType);
            
            // Обновляем текущее здоровье
            RecalculateCurrentHealth();
        }
    }
    
    // Находит сердечко для получения урона
    private HeartUI FindHeartForDamage(DamageType damageType)
    {
        // Ищем с конца (справа налево)
        for (int i = hearts.Count - 1; i >= 0; i--)
        {
            if (hearts[i].CanTakeDamage(damageType))
            {
                return hearts[i];
            }
        }
        return null;
    }
    
    // Пересчитывает текущее здоровье на основе состояния сердечек
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
        
        // Уведомляем другие системы об изменении здоровья
        GameEvents.HealthChanged(currentHealth, maxHealth);
    }
    
    // Проверяет, есть ли еще здоровье
    public bool HasHealth()
    {
        return hearts.Any(heart => heart.CurrentState != HeartState.Empty);
    }
    
    // Восстанавливает здоровье (для будущих улучшений)
    public void RestoreHealth(float amount)
    {
        // Логика восстановления здоровья
        // Можно реализовать позже для аптечек или бонусов
    }
    
    // Сброс здоровья до полного
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