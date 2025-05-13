using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Skill UI�� �ִ� 3���� ��ư�� ���� �Ҵ�Ǿ�
/// �������� ���õ� ��ų�� UI�� �����ְ�, �� ��ų�� ������ �������ִ� ��ũ��Ʈ.
/// </summary>
public class SkillButtonData : MonoBehaviour
{
    [Header("Connected Components")]
    [SerializeField] private PlayerSkillHandler playerSkillHandler;
    [SerializeField] private RangeWeaponHandler rangeWeaponHandler;     // Dynamically assigned
    [SerializeField] private Cooldown cooldown;


    [Header("UI Components")]
    public Button button;
    public Image skillIcon;
    public TextMeshProUGUI skillNameText;
    public Skill assignedSkill;

    [Header("Skill Applying Events")]
    public UnityEvent ApplyingSkillToStats;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.1f);
        rangeWeaponHandler = FindObjectOfType<RangeWeaponHandler>();
        Debug.Log("RangeWeaponHandler found and assigned to SkillButtonData.");

        if (rangeWeaponHandler != null)
        {
            // Prevention of multiple event calls on scene reload
            ApplyingSkillToStats.RemoveAllListeners();

            // ApplyingSKillToStats�̺�Ʈ�� �޼��� �߰� (Dynamically assigned)
            ApplyingSkillToStats.AddListener(rangeWeaponHandler.ResetStats);
            ApplyingSkillToStats.AddListener(rangeWeaponHandler.ApplyFinalStats);
        }
    }

    /// <summary>
    /// SkillButtonData�� �� ��ư�� ��ų ������ �������ִ� �޼���.
    /// </summary>
    /// <param name="skill"></param>
    public void SetSkillDataToButton(Skill skill)
    {
        skillIcon.sprite = skill.icon;
        skillNameText.text = skill.skillName;
        assignedSkill = skill;
    }

    /// <summary>
    /// ��ų�� ������ ��ư�� Ŭ������ �� ������ ��ų�� 
    /// Player���� ��������ִ� �޼���.
    /// </summary>
    public void AddSkillOnClick()
    {
        // ��ų ��ư�� �ٽ� Ȱ��ȭ �Ǳ������ ��ٿ� �ð��� üũ
        if (cooldown.IsCoolingDown)
        {
#if UNITY_EDITOR
            Debug.Log("It's cooling down!");
#endif
            return;
        }

        playerSkillHandler.SkillAcquired(assignedSkill);
        ApplyingSkillsToStats();
        GameManager.instance.SkillAdded();

        // ��ٿ� ����
        cooldown.StartCoolingDown();
    }

    public void ApplyingSkillsToStats()
    {
        Debug.Log("Skills are Applied to Stats!");
        ApplyingSkillToStats?.Invoke();
    }
}
