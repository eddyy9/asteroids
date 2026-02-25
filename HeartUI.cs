using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HeartUI : MonoBehaviour
{
    [Header("Heart Sprites")]
    [SerializeField] private Sprite fullHeartSprite;
    [SerializeField] private Sprite halfHeartSprite;
    [SerializeField] private Sprite emptyHeartSprite;

    [Header("Animation Settings")] 
    [SerializeField] private float shakeIntensity = 10f;
    [SerializeField] private float shakeDuration = 0.5f;
    [SerializeField] private float transitionDuration = 0.3f;

    private Image heartImage;
    private HeartState currentState = HeartState.Full;
    private Vector3 originalPosition;
    private RectTransform rectTransform;

    private void Awake()
    {
        heartImage = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        originalPosition = rectTransform.anchoredPosition;
        SetHeartState(HeartState.Full, false);
    }

    public void SetHeartState(HeartState newState, bool animate = true)
    {
        if (currentState == newState) return;
        
        currentState = newState;

        if (animate)
        {
            StartCoroutine(AnimateStateChange(newState));
        }
        else
        {
            UpdateHeartSprite(newState);
        }
    }

    private IEnumerator AnimateStateChange(HeartState targetState)
    {
        yield return StartCoroutine(ShakeHeart());
        
        yield return StartCoroutine(TransitionToState(targetState));
    }

    private IEnumerator ShakeHeart()
    {
        float elapsedTime = 0f;

        while (elapsedTime < shakeDuration)
        {
            Vector3 randomOffset = new Vector3(
                Random.Range(-shakeIntensity, shakeIntensity), 
                Random.Range(-shakeIntensity, shakeIntensity), 
                0f);
            
            rectTransform.anchoredPosition = originalPosition + randomOffset;
            
            elapsedTime += Time.deltaTime;
            yield return null;
            
        }
        rectTransform.anchoredPosition = originalPosition;
    }

    private IEnumerator TransitionToState(HeartState targetState)
    {
        Color startColor = heartImage.color;
        Color transparentColor = new Color(startColor.r, startColor.g, startColor.b, 0f);
        
        float elapsedTime = 0f;
        while (elapsedTime < transitionDuration / 2)
        {
            float t = elapsedTime / (transitionDuration / 2);
            heartImage.color = Color.Lerp(startColor, transparentColor, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        UpdateHeartSprite(targetState);
        
        elapsedTime = 0f;
        while (elapsedTime < transitionDuration / 2)
        {
            float t = elapsedTime / (transitionDuration / 2);
            heartImage.color = Color.Lerp(startColor, transparentColor, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        heartImage.color = startColor;
    }

    private void UpdateHeartSprite(HeartState state)
    {
        switch (state)
        {
            case HeartState.Full:
                heartImage.sprite = fullHeartSprite;
                break;
            case HeartState.Half:
                heartImage.sprite = halfHeartSprite;   
                break;
            case HeartState.Empty:
                heartImage.sprite = emptyHeartSprite;
                break;
        }
    }
    
    public HeartState CurrentState => currentState;

    public bool CanTakeDamage(DamageType damageType)
    {
        if (damageType == DamageType.Half)
        {
            return currentState == HeartState.Full;
        }
        else
        {
            return currentState != HeartState.Empty;
        }
    }
    
    public bool TakeDamage(DamageType damageType)
    {
        switch (damageType)
        {
            case DamageType.Half:
                if (currentState == HeartState.Full)
                {
                    SetHeartState(HeartState.Half);
                    return true;
                }
                break;
                
            case DamageType.Full:
                if (currentState == HeartState.Full)
                {
                    SetHeartState(HeartState.Empty);
                    return true;
                }
                else if (currentState == HeartState.Half)
                {
                    SetHeartState(HeartState.Empty);
                    return true;
                }
                break;
        }
        return false;
    }
    
    // Восстановление здоровья (для будущих улучшений)
    public void Heal()
    {
        if (currentState == HeartState.Empty)
        {
            SetHeartState(HeartState.Half);
        }
        else if (currentState == HeartState.Half)
        {
            SetHeartState(HeartState.Full);
        }
    }
}
