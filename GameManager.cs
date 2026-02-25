using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Делаем статический доступ (Singleton), чтобы другие скрипты могли легко к нему обращаться
    public static GameManager Instance { get; private set; }

    [Header("Settings - Unlock Requirements")]
    public int resourcesForDash = 15;
    public int researchForDash = 3;

    [Header("Current State (Read Only)")]
    public int currentResources = 0;
    public int currentResearch = 0;
    public bool isDashUnlocked = false;

    private void Awake()
    {
        // Проверка Синглтона: если GameManager уже есть, удаляем дубликат
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        // Позволяет объекту жить при смене сцены (из Меню в Игру и обратно)
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        GameEvents.OnGameOver += HandleGameOver;
        // Подпишемся здесь позже на сбор ресурсов, когда обновим другие скрипты
    }

    private void OnDisable()
    {
        GameEvents.OnGameOver -= HandleGameOver;
    }
    
    // Метод, который мы будем вызывать из скрипта игрока при подборе
    public void AddResource(int amount)
    {
        currentResources += amount;
        GameEvents.ResourcesChanged(currentResources, resourcesForDash);
        CheckProgression();
    }

    // Метод, который мы вызовем после успешного сканирования
    public void AddResearch()
    {
        currentResearch++;
        GameEvents.ResearchProgressChanged(currentResearch);
        CheckProgression();
    }

    private void CheckProgression()
    {
        if (isDashUnlocked) return;

        // Если собрали ресурсы И провели исследования -> Открываем Dash
        if (currentResources >= resourcesForDash && currentResearch >= researchForDash)
        {
            isDashUnlocked = true;
            GameEvents.DashAbilityUnlocked();
            Debug.Log("<color=green>ABILITY UNLOCKED: DASH ENGINE</color>");
        }
    }

    private void HandleGameOver()
    {
        Debug.Log("GAME OVER LOGIC FROM MANAGER");
    }
}