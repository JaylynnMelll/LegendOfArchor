using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponCategory
{
    Melee,
    Ranged,
}

public enum WhoIsWeilding
{
    Player,
    Enemy,
}
public class WeaponHandler : MonoBehaviour
{
    [SerializeField] public WeaponCategory weaponCategory;
    [SerializeField] public WhoIsWeilding whoIsWeilding;
    [SerializeField] public BaseWeaponStats weaponStats;
    [SerializeField] public Skill multiShotSkill;

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

    [SerializeField] private float ciriticalChance = 0.2f;
    public float CriticalChance { get => ciriticalChance; set => ciriticalChance = value; }

    [SerializeField] private float ciriticalDamage = 1.5f;
    public float CriticalDamage { get => ciriticalDamage; set => ciriticalDamage = value; }

    [SerializeField] private int price = 100;
    public int Price { get => price; set => price = value; }

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
    protected PlayerSkillHandler playerSkillHandler;

    public AudioClip attackSoundClip;

    protected virtual void Awake()
    {
        playerSkillHandler = FindObjectOfType<PlayerSkillHandler>();
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
        if (this is MeleeWeaponHandler && whoIsWeilding == WhoIsWeilding.Player)
        {
            StartCoroutine(DeactivateAnimatingWeaponPivot());
        }

        AttackAnimation();

        if (attackSoundClip != null)
            SoundManager.PlayClip(attackSoundClip);
    }

    private IEnumerator DeactivateAnimatingWeaponPivot()
    {
        // 근거리 무기를 휘두를 동안 player 손에 쥔 무기 이미지 비활성화
        GameObject weaponImage = transform.GetChild(0).gameObject;
        if (weaponImage != null)
        {
            weaponImage.SetActive(false);
            yield return new WaitForSeconds(0.8f);
            weaponImage.SetActive(true);
        }
    }

    public virtual void ResetWeaponStats()
    {
        // Reset weapon stats to default 
        WeaponSize = weaponStats.WeaponSize;
        WeaponPower = weaponStats.WeaponPower;
        WeaponSpeed = weaponStats.WeaponSpeed;
        WeaponRange = weaponStats.WeaponRange;
        CriticalChance = weaponStats.CriticalChance;
        CriticalDamage = weaponStats.CriticalDamage;
        KnockbackPower = weaponStats.KnockbackPower;
        KnockbackTime = weaponStats.KnockbackTime;
    }

    public virtual void ApplyFinalWeaponStats()
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

    public Sprite GetWeaponSprite()
    {
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        return sr != null ? sr.sprite : null;
    }

}
