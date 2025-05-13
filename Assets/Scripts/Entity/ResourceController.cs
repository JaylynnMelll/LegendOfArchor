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
    public float CurrentHealth { get; set; }

    [SerializeField] private int _maxHealth = 10; // 최대 체력
    public int MaxHealth
    {
        get { return _maxHealth; }
        set
        {
            _maxHealth = value;
        }
    }
    private SlimeBossController slimeBossController;
    private NecromancerBossController necromancerBossController;
    private EnemyController enemyController;
    private PlayerController playerController;
    private PlayerSkillHandler playerSkillHandler;
    private void Awake()
    {
        baseController = GetComponent<BaseController>();
        statHandler = GetComponent<StatHandler>();
        animationHandler = GetComponent<AnimationHandler>();
        slimeBossController = GetComponent<SlimeBossController>();
        necromancerBossController = GetComponent<NecromancerBossController>();
        enemyController = GetComponent<EnemyController>();
        playerController = GetComponent<PlayerController>();
        playerSkillHandler = GetComponent<PlayerSkillHandler>();
        CurrentHealth = statHandler.Health;
    }
    private void Start()
    {
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

    public void SetHealth(float value)
    {
        CurrentHealth = Mathf.Clamp(value, 0, MaxHealth);
        OnChangeHealth?.Invoke(CurrentHealth, MaxHealth);
    }
    public bool ChangeHealth(float change)
    {
        var playerController = GetComponent<PlayerController>();
        if (change < 0 && playerController != null && playerController.IsDodging)
        {
            return false;
        }

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
            GameManager.instance.ShowDamageText(transform.position, Mathf.FloorToInt(-change));
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
        if (slimeBossController != null)
        {
            slimeBossController.Died();
        }
        else if (necromancerBossController != null)
        {
            necromancerBossController.Died();
        }
        else if (enemyController != null)
        {
            baseController?.Died();
        }
        else if (playerController != null)
        {
            baseController?.Died();
        }
    }

    //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
    public void HPReset()
    {
        MaxHealth = 100;
    }

    public void HPBoost()
    {
        MaxHealth = (int)playerSkillHandler.CalculateFinalHealth(MaxHealth);
    }
}