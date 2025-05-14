using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class PlayerSkillHandler : MonoBehaviour
{
    [Header("Connected Components")]
    public WeaponHandler weaponHandler;
    public RangeWeaponHandler rangeWeaponHandler;
    public MeleeWeaponHandler meleeWeaponHandler;
    public ProjectileController projectileController;

    [Header("Skill Modifiers")]
    // 스킬과 그 스킬의 스택 수를 저장하는 리스트
    public List<Skill> acquiredSkills = new List<Skill>();

    // runtime에서 acquiredSkills에 추가된 스킬들의 스택을 관리하기 위한 리스트
    public List<RuntimeSkill> trackingList = new List<RuntimeSkill>(); 

    private float damageAdditive = 0f;
    private float damageMultiplicative = 1f;

    private float attackSpeedAdditive = 0f;
    private float attackSpeedMultiplicative = 1f;

    private float criticalChanceAdditive = 0f;
    private float criticalDamageMultiplicative = 1f;

    private float rangeAdditive = 0f;
    private float rangeMultiplicative = 1f;

    private float healthBonusAdditive = 0f;
    private float healthBonusMultiplicative = 1f;

    private ElementType currentElement = ElementType.None;

    //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
    // [Public Methods]
    public void SkillAcquired(Skill skill)
    {
        // acquiredSkills에 있는 스킬에 붙어있는 runtimeSkill을 찾기 위한 코드
        RuntimeSkill runtimeSkill = trackingList.Find(s => s.skill == skill);

        // 1. acquiredSkills에 해당 스킬이 없고, runtimeSkill이 null일 경우
        if (!acquiredSkills.Contains(skill) && runtimeSkill == null)
        {
            // 01) acquiredSkills에 스킬을 추가
            acquiredSkills.Add(skill);
            // 02) acquiredSkills에 추가된 스킬을 trackingList에 추가
            runtimeSkill = new RuntimeSkill(skill);
            trackingList.Add(runtimeSkill); 

            runtimeSkill.AddStack();
            ApplySkillEffect(skill);
#if UNITY_EDITOR
            Debug.Log($"Learned new skill! : {skill.skillName}");
#endif
        }
        // 2. currentStacks이 maxStacks보다 작고 acquiredSkills에 해당 스킬이 있고, runtimeSkill이 null이 아닐 경우
        else if (runtimeSkill.AddStack())
        {
            ApplySkillEffect(skill);
#if UNITY_EDITOR
            Debug.Log($"{skill.skillName} stacked! Now at {runtimeSkill.currentStacks} stacks.");
#endif
        }
        // 3. currentStacks이 maxStacks보다 크고 acquiredSkills에 해당 스킬이 있고, runtimeSkill이 null이 아닐 경우
        else if (runtimeSkill.AddStack())
        {
#if UNITY_EDITOR
            Debug.Log($"{skill.skillName} is already at max stacks.");
#endif
        }
    }

    public bool HasThisSkill (Skill skill)
    {
        return skill != null && acquiredSkills.Contains(skill); 
    }

    public float CalculateFinalDamage(float baseDamage)
    {
        float finalDamage = (baseDamage * damageMultiplicative) + (baseDamage * damageAdditive);
        return finalDamage;
    }

    public float CalculateFinalAttackSpeed(float baseAttackSpeed)
    {
        float finalAttackSpeed = (baseAttackSpeed * attackSpeedMultiplicative) + (baseAttackSpeed * attackSpeedAdditive);
        return finalAttackSpeed;
    }

    public float CalculateFinalCriticalChance(float baseCriticalChance)
    {
        float finalCriticalChance = (baseCriticalChance + criticalChanceAdditive);
        return finalCriticalChance;
    }

    public float CalculateFinalCriticalDamage(float baseCriticalDamage)
    {
        float finalCriticalDamage = (baseCriticalDamage * criticalDamageMultiplicative);
        return finalCriticalDamage;
    }

    public float CalculateFinalRange(float baseRange)
    {
        float finalRange = (baseRange * rangeMultiplicative) + (baseRange * rangeAdditive);
        return finalRange;
    }

    public float CalculateFinalHealth(float baseHealth)
    {
        float finalHealth = (baseHealth * healthBonusMultiplicative) + (baseHealth * healthBonusAdditive);
        return finalHealth;
    }

    // 한 번 쏠때 발사되는 총알의 개수를 반환하는 메서드 (스킬도 제작)
    // -> 쌓이는 스택마다 RangeWeaponHandler의 NumberofProjectilesPerShot값을 건들여주면 됨.
    // -> 기본 1발사, 스택이 쌓일때마다 발사되는 총알의 개수가 1개씩 증가함.

    //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
    // [Private Methods]
    private void ApplySkillEffect(Skill skill)
    {
        // 스킬 카테고리별로 알맞는 효과를 적용시켜주는 메서드
        switch (skill.category)
        {
            case SkillCategory.Attribute:
                ApplyAttributeBoost(skill);
                break;

            case SkillCategory.Elemental:
                // Apply elemental effect
                break;

            case SkillCategory.DeathEffect:
                // Apply death effect
                break;

            case SkillCategory.WeaponMod:
                // Apply weapon modification
                break;

            case SkillCategory.Special:
                // Apply special effect
                break;

            default:
                Debug.LogWarning("Unknown skill category: " + skill.category);
                break;
        }
    }

    private void ApplyAttributeBoost(Skill skill)
    {
        damageAdditive += skill.additionalDamagePercent;
        damageMultiplicative *= 1 + skill.baseDamageMultiplier;

        attackSpeedAdditive += skill.additionalAttakSpeedPercent;
        attackSpeedMultiplicative *= 1 + skill.baseAttackSpeedMultiplier;

        criticalChanceAdditive += skill.additionalCriticalChancePercent;
        criticalDamageMultiplicative *= 1 + skill.criticalDamageMultiplier;

        rangeAdditive += skill.additionalAttackRangePercent;
        rangeMultiplicative *= 1 + skill.baseAttackRangeMultiplier;

        healthBonusAdditive += skill.additionalHealthPercent;
        healthBonusMultiplicative *= 1 + skill.baseHealthMultiplier;
    }
}

    

