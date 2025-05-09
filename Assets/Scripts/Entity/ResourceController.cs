using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceController : MonoBehaviour
{
    [SerializeField] private float healthChangeDealy = 0.5f;

    private BaseController baseController;
    private StatHandler statHandler;
    private AnimationHandler animationHandler;
    public AudioClip damageSoundClip;
    private Action<float, float> OnChangeHealth;

    private float timeSinceLastHealthChange = float.MaxValue;

    public float CurrentHealth { get; private set; }
    public float MaxHealth => statHandler.Health;

    private SlimeBossController slimeBossController;
    private EnemyController enemyController;

    private void Awake()
    {
        baseController = GetComponent<BaseController>();
        statHandler = GetComponent<StatHandler>();
        animationHandler = GetComponent<AnimationHandler>();
        slimeBossController = GetComponent<SlimeBossController>();
        enemyController = GetComponent<EnemyController>();
    }

    private void Start()
    {
        CurrentHealth = statHandler.Health;
    }

    private void Update()
    {
        if (timeSinceLastHealthChange < healthChangeDealy)
        {
            timeSinceLastHealthChange += Time.deltaTime;
            if (timeSinceLastHealthChange >= healthChangeDealy)
            {
                animationHandler.InvinvibilityEnd();
            }
        }
    }

    // 보스몬스터 체력 초기화 위해 추가
    public void SetHealth(float value)
    {
        CurrentHealth = Mathf.Clamp(value, 0, MaxHealth);
        OnChangeHealth?.Invoke(CurrentHealth, MaxHealth);
    }

    public bool ChangeHealth (float change)
    {
        if (change == 0 || timeSinceLastHealthChange < healthChangeDealy)
        {
            return false;
        }

        timeSinceLastHealthChange = 0.0f;
        CurrentHealth += change;
        CurrentHealth = CurrentHealth > MaxHealth ? MaxHealth : CurrentHealth;
        CurrentHealth = CurrentHealth < 0 ? 0 : CurrentHealth;

        OnChangeHealth?.Invoke(CurrentHealth, MaxHealth);

        if (change < 0)
        {
            animationHandler.Damage();

            if (damageSoundClip != null)
                SoundManager.PlayClip(damageSoundClip);
        }

        if (CurrentHealth <= 0f)
        {
            Died();
        }

        return true;
    }

    public void AddHealthChangeEvent(Action<float, float> action)
    {
        OnChangeHealth += action;
    }

    public void RemoveHealthChangeEvent(Action<float, float> action)
    {
        OnChangeHealth -= action;
    }

    private void Died()
    {
        if(slimeBossController != null)
        {
            slimeBossController.Died();
        }
        else if (enemyController != null)
        {
            baseController?.Died();
        }
    }
}

