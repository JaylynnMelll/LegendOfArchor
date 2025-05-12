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
    public SkillCategory category;
    public WeaponType weaponType;
    public ElementType elemenType;
    public TriggerType triggerType;

    [Header("Effect Details")]
    public float baseDamageMultiplier = 1f;
    [Range(0.01f, 1f)]
    public float additionalDamagePercent = 0f;
    public float attakSpeedModifier = 0f;
    public float attackRangeModifier = 0f;       // 공격 범위 증가 비율
    [Range(0.01f, 1f)]
    public float criticalChanceModifier = 0f;
    public float criticalDamageModifier = 0f;
    public float projectileSizeModifier = 0f;    // 투사체 크기 증가 비율
    public float healththModifier = 0f;
    public float duration = 0f;                  // 제한 시간동안 적용되는 버프에 한해 적용
    public float interval = 0f;                  // 지속시간동안 몇 초마다 적용되는 버프에 한해 적용
    public bool isStackable = false;
    public int currentStacks = 0;                // 현재 스택 수치
    public int maxStacks = 1;                    // 스택이 쌓일 수 있는 최대치

    [Header("Special Parameters")]
    // 도트뎀 관련
    public bool appliesDot;     
    public float dotInterval;                   // 몇 초마다 도트 데미지를 줄 것인지                    
    public float dotPercent;                    // 데미지별 도트 데미지 비율
    public bool appliesMultiShot;                // 멀티샷 여부
    public float delayBetweenShots;             // 멀티샷 간격
    public bool appliesPiercing;                // 관통 여부
    public bool appliesReflective;              // 반사 여부

    [Header("Elemental Effects")]
    public bool appliesFreeze;                  // 적을 얼릴 것인지
    public float freezeDuration;                // 얼리는 시간
    public bool appliesBurn;                    // 적을 불태울 것인지
    public bool appliesChainEffect;
}
