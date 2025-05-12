using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    [Header("Attack Info")]
    [SerializeField] private float attackDelay = 1f;
    public float AttackDelay { get => attackDelay; set => attackDelay = value; }

    [SerializeField] private float weaponSize = 1f;
    public float WeaponSize { get => weaponSize; set => weaponSize = value; }

    [SerializeField] private float weaponPower = 1f;
    public float WeaponPower { get => weaponPower; set => weaponPower = value; }

    [SerializeField] private float weaponSpeed = 1f;
    public float WeaponSpeed { get => weaponSpeed; set => weaponSpeed = value; }

    [SerializeField] private float weaponRange = 10f;
    public float WeaponRange { get => weaponRange; set => weaponRange = value; }

    [SerializeField] private float ciriticalChance =0.2f;
    public float CriticalChance { get => ciriticalChance; set => ciriticalChance = value; }

    [SerializeField] private float ciriticalDamage = 1.5f;
    public float CriticalDamage { get => ciriticalDamage; set => ciriticalDamage = value; }

    public LayerMask target;


    [Header("KnockBack Info")]
    [SerializeField] private bool isOnKnockback = false;
    public bool IsOnKnockback { get => isOnKnockback; set => isOnKnockback = value; }

    [SerializeField] private float knockbackPower = 0.1f;
    public float KnockbackPower { get => knockbackPower; set => knockbackPower = value; }

    [SerializeField] private float knockbackTime = 0.5f;
    public float KnockbackTime { get => knockbackTime; set => knockbackTime = value; }

    private static readonly int IsAttaking = Animator.StringToHash("IsAttacking");

    public BaseController Controller { get; private set; }
    public PlayerSkillHandler PlayerSkillHandler { get; private set; }
    public bool IsPlayerWeapon { get; private set; }

    private Animator animator;
    private SpriteRenderer weaponRenderer;
    private PlayerSkillHandler playerSkillHandler;

    public AudioClip attackSoundClip;

    protected virtual void Awake()
    {
        playerSkillHandler = GetComponentInParent<PlayerSkillHandler>();
        Controller = GetComponentInParent<BaseController>();
        animator = GetComponentInChildren<Animator>();
        weaponRenderer = GetComponentInChildren<SpriteRenderer>();

        animator.speed = 1.0f / attackDelay;
        transform.localScale = Vector3.one * weaponSize;
    }

    protected virtual void Start()
    {

    }

    public void SetAsPlayerWeapon(PlayerSkillHandler handler)
    {
        IsPlayerWeapon = true;
        PlayerSkillHandler = handler;
    }

    public virtual void Attack()
    {
        AttackAnimation();

        if (attackSoundClip != null)
            SoundManager.PlayClip(attackSoundClip);
    }

    public virtual void ResetStats()
    {
        // Reset weapon stats to default 
        WeaponSize = 1f;
        WeaponPower = 1f;
        WeaponSpeed = 1f;
        WeaponRange = 10f;
        KnockbackPower = 0.1f;
        KnockbackTime = 0.5f;
    }

    public virtual void ApplyFinalStats()
    {
        WeaponPower = playerSkillHandler.CalculateFinalDamage(WeaponPower);
        WeaponSpeed = playerSkillHandler.CalculateFinalAttackSpeed(WeaponSpeed);
        CriticalChance = playerSkillHandler.CalculateFinalCriticalChance(CriticalChance);
        CriticalDamage = playerSkillHandler.CalculateFinalCriticalDamage(CriticalDamage);
        WeaponRange = playerSkillHandler.CalculateFinalRange(WeaponRange);
    }

    public void AttackAnimation()
    {
        animator.SetTrigger(IsAttaking);
    }

    public virtual void Rotate(bool isLeft)
    {
        weaponRenderer.flipY = isLeft;
    }
}
