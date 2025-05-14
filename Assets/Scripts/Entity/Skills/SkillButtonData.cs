using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Presets;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SkillButtonData : MonoBehaviour
{
    [Header("Connected Components")]
    [SerializeField] private PlayerSkillHandler playerSkillHandler;
    [SerializeField] private SkillManager skillManager;
    [SerializeField] private Transform weaponPivot;
    [SerializeField] private WeaponHandler weaponHandler;
    [SerializeField] private RangeWeaponHandler rangeWeaponHandler;
    [SerializeField] private MeleeWeaponHandler meleeWeaponHandler;
    [SerializeField] private ResourceController resourceController;
    [SerializeField] private Cooldown cooldown;
    private WeaponCategory weaponCategory;

    [Header("UI Components")]
    public Button button;
    public Image skillIcon;
    public TextMeshProUGUI skillNameText;
    public Skill assignedSkill;

    [Header("Skill Applying Events")]
    public UnityEvent ApplySkillToStats;
    public UnityEvent ApplyHPBoost;
    public UnityEvent ApplyMultiShot;

    //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
    // [Unity LifeCycle]
    public void Init()
    {
        GrabWeaponScripts();

        // Event for applying skills to ranged weapon stats
        if (weaponHandler != null)
        {
            // Prevention of multiple event calls on scene reload
            ApplySkillToStats.RemoveAllListeners();

            // Subscribing methods to ApplyingSKillToStats() events (Dynamically assigned)
            ApplySkillToStats.AddListener(weaponHandler.ResetWeaponStats);
            ApplySkillToStats.AddListener(weaponHandler.ApplyFinalWeaponStats);

            if (weaponCategory == WeaponCategory.Ranged)
            {
                ApplyMultiShot.RemoveAllListeners();
                ApplyMultiShot.AddListener(rangeWeaponHandler.MultiShot);
            }
        }

        // Event for applying skills to player stats
        if (resourceController != null)
        {
            ApplyHPBoost.RemoveAllListeners();

            ApplyHPBoost.AddListener(resourceController.HPReset);
            ApplyHPBoost.AddListener(resourceController.HPBoost);
        }
    }

    //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
    // [Public Methods]
    public void SetSkillDataToButton(Skill skill)
    {
        skillIcon.sprite = skill.icon;
        skillNameText.text = skill.skillName;
        assignedSkill = skill;
    }

    public void AddSkillOnClick()
    {
        if (cooldown.IsCoolingDown)
        {
#if UNITY_EDITOR
            Debug.Log("It's cooling down!");
#endif
            return;
        }

        playerSkillHandler.SkillAcquired(assignedSkill);
        ApplyingSkillsToStats();
        GameManager.Instance.SkillAdded();

        cooldown.StartCoolingDown();
    }

    public void ApplyingSkillsToStats()
    {
        switch (assignedSkill.skillID)
        {
            case SkillID.HPBoost:
                ApplyHPBoost?.Invoke();
                break;

            case SkillID.MultiShot:
                ApplySkillToStats?.Invoke();
                ApplyMultiShot?.Invoke();
                break;

            case SkillID.Parrying:
            case SkillID.WeaponEnlarging:
            case SkillID.BouncingShot:
            case SkillID.PiercingShot:
                break;

            default:
                ApplySkillToStats?.Invoke();
                break;
        }
        
        Debug.Log("Skill applied to stats: " + assignedSkill.name);
    }

    //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
    // [Private Methods]
    private void GrabWeaponScripts()
    {
        if (weaponCategory == WeaponCategory.Melee)
        {
            meleeWeaponHandler = weaponPivot.GetComponentInChildren<MeleeWeaponHandler>();
            weaponHandler = meleeWeaponHandler;
            Debug.Log("MeleeWeaponHandler found in the weapon pivot.");
        }
        else if (weaponCategory == WeaponCategory.Ranged)
        {
            rangeWeaponHandler = weaponPivot.GetComponentInChildren<RangeWeaponHandler>();
            weaponHandler = rangeWeaponHandler;
            Debug.Log("RangeWeaponHandler found in the weapon pivot.");
        }
        else
        {
            Debug.LogError("No weapon found in the weapon pivot.");
        }
    }
}
