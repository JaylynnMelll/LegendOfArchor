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
    // ��ų�� �� ��ų�� ���� ���� �����ϴ� ����Ʈ
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

    

