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
    [SerializeField] private RangeWeaponHandler rangeWeaponHandler;
    [SerializeField] private ResourceController resourceController;
    [SerializeField] private Cooldown cooldown;

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
        GrabWeaponScript();

        // Event for applying skills to ranged weapon stats
        if (rangeWeaponHandler != null)
        {
            // Prevention of multiple event calls on scene reload
            ApplySkillToStats.RemoveAllListeners();

            // Subscribing methods to ApplyingSKillToStats() events (Dynamically assigned)
            ApplySkillToStats.AddListener(rangeWeaponHandler.ResetWeaponStats);
            ApplySkillToStats.AddListener(rangeWeaponHandler.ApplyFinalWeaponStats);

            ApplyMultiShot.RemoveAllListeners();
            ApplyMultiShot.AddListener(rangeWeaponHandler.MultiShot);
        }

        // Event for applying skills to player stats
        if (resourceController != null)
        {
            ApplyHPBoost.RemoveAllListeners();

            ApplyHPBoost.AddListener(resourceController.HPReset);
            ApplyHPBoost.AddListener(resourceController.HPBoost);
        }

        Debug.Log("SkillButtonData initialized successfully.");
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

            case SkillID.BounceShot:
                break;

            default:
                ApplySkillToStats?.Invoke();
                break;
        }
        
        Debug.Log("Skill applied to stats: " + assignedSkill.name);
    }

    //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
    // [Private Methods]
    private void GrabWeaponScript()
    {
        if (weaponPivot.childCount > 0)
        {
            Transform weapon = weaponPivot.GetChild(0);
            rangeWeaponHandler = weapon.GetComponent<RangeWeaponHandler>();
            Debug.Log("Weapon script grabbed successfully.");
        }
        else
        {
            Debug.LogError("No weapon found in the weapon pivot.");
        }
    }
}
