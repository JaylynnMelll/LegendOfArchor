using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Skill의 카테고리를 정의해주는 enum.
/// </summary>
public enum SkillCategory
{
    Attribute,      // 공격력 증가나 방어력 증가와 같은 부스트 스킬들
    Elemental,      // 속성 공격력 관련한 스킬들
    DeathEffect,    // 적 처치시 발동하는 스킬들
    WeaponMod,      // 무기 관련 스킬들 (멀티샷, 옆으로도 나가는 화살, 관통, 반사 등등)
    Special         // 특수 스킬들
}

/// <summary>
/// 무기 타입을 정의해주는 enum.
/// </summary>
public enum WeaponType
{
    Melee,
    Ranged,
    Both
}

/// <summary>
/// 공격속성 타입을 정의해주는 enum.
/// </summary>
public enum ElementType
{
    None,
    Fire,
    Ice,
    Poison,
    Lightning,
    Holy,
    Dark,
}

/// <summary>
/// 스킬 발동 조건을 정의해주는 enum.
/// </summary>
public enum TriggerType
{
    Passive,
    OnHit,
    OnKill,
    OnLowHealth,
    TimedBuff
}

public enum SkillID
{
    AttackBoost,
    AttackSpeedBoost,
    CriticalMaster,
    HPBoost,
    MultiShot,
    BouncingShot,
    PiercingShot,
    Parrying,
    WeaponEnlarging,
}

//-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//

/// <summary>
/// Skill Scritable Object를 만들기 위한 기본 틀을 제공해주는 클래스.
/// </summary>
[CreateAssetMenu (fileName = "New Skill", menuName = "Skills/New Skill")]
public class Skill : ScriptableObject
{
    [Header("Basic Skill Info")]
    public string skillName;
    [TextArea] public string skillDescription;
    public Sprite icon;

    [Header("Classification")]
    public SkillID skillID;                
    public SkillCategory category;
    public WeaponType weaponType;
    public ElementType elementType;
    public TriggerType triggerType;

    [Header("Effect Details")]
    
    // Damage
    [Range(0f, 1f)]
    public float additionalDamagePercent = 0f;
    public float baseDamageMultiplier = 0f;

    // Attack Speed
    [Range(0f, 1f)]
    public float additionalAttakSpeedPercent = 0f;
    public float baseAttackSpeedMultiplier = 0f;

    // Attack Range
    [Range(0f, 1f)]
    public float additionalAttackRangePercent = 0f;
    public float baseAttackRangeMultiplier = 0f;
  

    // Critical Hit
    [Range(0f, 1f)]
    public float additionalCriticalChancePercent = 0f;
    public float criticalDamageMultiplier = 0f;

    // Projectile Size
    [Range(0f, 1f)]
    public float additionalProjectileSizePercent = 0f;    
    public float baseProjectileSizeMultiplier= 0f;

    // HP
    [Range(0f, 1f)]
    public float additionalHealthPercent = 0f;
    public float baseHealthMultiplier = 0f;     

    public float duration = 0f;                 
    public float interval = 0f;                 
    public bool isStackable = false;
    public int maxStacks = 1;                   

    [Header("Special Parameters")]
    // 도트뎀 관련
    public bool appliesDot;     
    public float dotInterval;                   // 몇 초마다 도트 데미지를 줄 것인지                    
    public float dotPercent;                    // 데미지별 도트 데미지 비율

    [Header("Weapon Modifiers")]
    // ranged weapon 관련
    public bool appliesMultiShot;           
    public bool appliesPiercing;            
    public bool appliesBouncing;
    public bool appliesParrying;
    public bool appliesEnlargning;

    [Header("Elemental Effects")]
    public bool appliesFreeze;                  // 적을 얼릴 것인지
    public float freezeDuration;                // 얼리는 시간
    public bool appliesBurn;                    // 적을 불태울 것인지
    public bool appliesChainEffect;
}
