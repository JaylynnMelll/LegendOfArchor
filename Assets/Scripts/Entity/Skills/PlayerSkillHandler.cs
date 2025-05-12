using System.Collections;
using System.Collections.Generic;
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

    private float damageMultiplicative = 1f;
    private float damageAdditive = 0f;

    private float attackSpeedMultiplicative = 1f;
    private float attackSpeedAdditive = 0f;

    private float criticalChanceAdditive = 0f;
    private float criticalDamageMultiplicative = 1f;

    private float rangeMultiplicative = 1f;
    private float rangeAdditive = 0f;

    private float healthBonus = 0f;

    private ElementType currentElement = ElementType.None;

    //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
    // [Public Methods]
    public void SkillAcquired(Skill skill)
    {
        if (!acquiredSkills.Contains(skill))
        {
            acquiredSkills.Add(skill);
            skill.currentStacks += 1; 
            ApplySkillEffect(skill);
#if UNITY_EDITOR
            Debug.Log($"Learned new skill! : {skill.skillName}");
#endif

        }
        else if (acquiredSkills.Contains(skill) && skill.currentStacks < skill.maxStacks)
        {
            acquiredSkills.Add(skill);
            skill.currentStacks += 1;
            ApplySkillEffect(skill);
#if UNITY_EDITOR
            Debug.Log($"{skill.skillName} stacked! Now at {skill.currentStacks} stacks.");
#endif
        }
        else
        {
#if UNITY_EDITOR
            Debug.Log($"{skill.skillName} is already at max stacks.");
#endif
        }
    }

    public bool HasThisSkill (Skill skill)
    {
        return acquiredSkills.Contains(skill) || skill != null ? true : false;
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
        float finalCriticalChance = (baseCriticalChance * criticalChanceAdditive);
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
        float finalHealth = (baseHealth + healthBonus);
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
        damageMultiplicative *= 1 + skill.baseDamageMultiplier;
        damageAdditive += skill.additionalDamagePercent;

        attackSpeedMultiplicative *= 1 + skill.attakSpeedModifier;
        attackSpeedAdditive += skill.attakSpeedModifier;

        criticalChanceAdditive += skill.criticalChanceModifier;
        criticalDamageMultiplicative *= 1 + skill.criticalDamageModifier;

        rangeMultiplicative *= 1 + skill.attackRangeModifier;
        rangeAdditive += skill.attackRangeModifier;

        healthBonus += skill.healththModifier;
    }
}

    

