using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
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
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        GameEvents.OnGameOver += HandleGameOver;
    }

    private void OnDisable()
    {
        GameEvents.OnGameOver -= HandleGameOver;
    }
    
    public void AddResource(int amount)
    {
        currentResources += amount;
        GameEvents.ResourcesChanged(currentResources, resourcesForDash);
        CheckProgression();
    }

    public void AddResearch()
    {
        currentResearch++;
        GameEvents.ResearchProgressChanged(currentResearch);
        CheckProgression();
    }

    private void CheckProgression()
    {
        if (isDashUnlocked) return;

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