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
    // ��ų�� �� ��ų�� ���� ���� �����ϴ� ����Ʈ
    public List<Skill> acquiredSkills = new List<Skill>();

    // runtime���� acquiredSkills�� �߰��� ��ų���� ������ �����ϱ� ���� ����Ʈ
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
        // acquiredSkills�� �ִ� ��ų�� �پ��ִ� runtimeSkill�� ã�� ���� �ڵ�
        RuntimeSkill runtimeSkill = trackingList.Find(s => s.skill == skill);

        // 1. acquiredSkills�� �ش� ��ų�� ����, runtimeSkill�� null�� ���
        if (!acquiredSkills.Contains(skill) && runtimeSkill == null)
        {
            // 01) acquiredSkills�� ��ų�� �߰�
            acquiredSkills.Add(skill);
            // 02) acquiredSkills�� �߰��� ��ų�� trackingList�� �߰�
            runtimeSkill = new RuntimeSkill(skill);
            trackingList.Add(runtimeSkill); 

            runtimeSkill.AddStack();
            ApplySkillEffect(skill);
#if UNITY_EDITOR
            Debug.Log($"Learned new skill! : {skill.skillName}");
#endif
        }
        // 2. currentStacks�� maxStacks���� �۰� acquiredSkills�� �ش� ��ų�� �ְ�, runtimeSkill�� null�� �ƴ� ���
        else if (runtimeSkill.AddStack())
        {
            ApplySkillEffect(skill);
#if UNITY_EDITOR
            Debug.Log($"{skill.skillName} stacked! Now at {runtimeSkill.currentStacks} stacks.");
#endif
        }
        // 3. currentStacks�� maxStacks���� ũ�� acquiredSkills�� �ش� ��ų�� �ְ�, runtimeSkill�� null�� �ƴ� ���
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

    // �� �� �� �߻�Ǵ� �Ѿ��� ������ ��ȯ�ϴ� �޼��� (��ų�� ����)
    // -> ���̴� ���ø��� RangeWeaponHandler�� NumberofProjectilesPerShot���� �ǵ鿩�ָ� ��.
    // -> �⺻ 1�߻�, ������ ���϶����� �߻�Ǵ� �Ѿ��� ������ 1���� ������.

    //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
    // [Private Methods]
    private void ApplySkillEffect(Skill skill)
    {
        // ��ų ī�װ����� �˸´� ȿ���� ��������ִ� �޼���
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

    

