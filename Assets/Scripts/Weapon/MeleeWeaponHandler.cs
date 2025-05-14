using System.Linq;
using UnityEngine;

public class MeleeWeaponHandler : WeaponHandler
{
    [Header("Melee Attack Info")]
    public Vector2 collideBoxSize = Vector2.one;

    [Header("SpinSlash Settings")]
    public bool useSpinningProjectile = false;
    public GameObject spinSlashPrefab;
    public LayerMask wallLayer;
    public float weaponSizeMultiplier = 1.2f;

    public Skill WeaponSizeEnlargedSkill;
    public Skill parryingSkill;

    protected override void Start()
    {
        base.Start();
        collideBoxSize = SetColliderBoxSize();
    }

    public override void Attack()
    {
        base.Attack();

        if (IsPlayerWeapon && useSpinningProjectile && spinSlashPrefab != null)
        {
            int slashCount = 1;

            // ��Ƽ�� ��ų�� ����� ���
            if (playerSkillHandler.acquiredSkills.Contains(multiShotSkill))
            {
                MeleeMultiShotApplied(ref slashCount);
            }

            //  �¾�ó�� 360�� ���� ����
            float totalAngle = 360f;
            float angleStep = slashCount > 0 ? totalAngle / slashCount : 0f;

            for (int i = 0; i < slashCount; i++)
            {
                float angleOffset = i * angleStep;

                GameObject spinObj = Instantiate(spinSlashPrefab, transform.position, Quaternion.identity);
                spinObj.transform.SetParent(transform); // ȸ�� �߽� = �÷��̾�

                var spin = spinObj.GetComponent<SpinningMeleeProjectile>();
                if (spin != null)
                {
                    spin.Init(
                        transform,
                        target,
                        wallLayer,
                        PlayerSkillHandler.CalculateFinalDamage(WeaponPower),
                        PlayerSkillHandler.CalculateFinalRange(WeaponRange),
                        PlayerSkillHandler.CalculateFinalCriticalChance(0f),
                        PlayerSkillHandler.CalculateFinalCriticalDamage(1f),
                        GetWeaponSprite(),
                        angleOffset
                    );
                }
            }

            return;
        }
        
        // ���� �ڽ� ���� ���� ����
        RaycastHit2D hit = Physics2D.BoxCast(
            transform.position + (Vector3)Controller.LookDirection * collideBoxSize.x,
            collideBoxSize, 0, Vector2.zero, 0, target);

        if (hit.collider != null)
        {
            ResourceController resourceController = hit.collider.GetComponent<ResourceController>();
            if (resourceController != null)
            {
                resourceController.ChangeHealth(-WeaponPower);
                if (IsOnKnockback)
                {
                    BaseController controller = hit.collider.GetComponent<BaseController>();
                    if (controller != null)
                    {
                        controller.ApplyKnockback(transform, KnockbackPower, KnockbackTime);
                    }
                }
            }
            //if (whoIsWeilding == WhoIsWeilding.Player && hit.collider == GameObject.FindObjectByTag)
            //{
            //    //  �ĸ� ��ų�� ����� ���
            //    ParryingSkillApplied();
            //}
        }
    }

    public void MeleeMultiShotApplied(ref int slashCount)
    {
        //  ��Ƽ�� ��ų �÷��̾�Ը� �ݿ�
        if (playerSkillHandler.trackingList != null)
        {
            Skill multiShotSkill = playerSkillHandler.acquiredSkills.Find(s => s.appliesMultiShot);
            if (multiShotSkill && whoIsWeilding == WhoIsWeilding.Player)
            {
                RuntimeSkill multiShotCurrentStacks = playerSkillHandler.trackingList.Find(rt => rt.skill == multiShotSkill);
                if (multiShotCurrentStacks != null && whoIsWeilding == WhoIsWeilding.Player)
                {
                    if (multiShotCurrentStacks.currentStacks < multiShotSkill.maxStacks)
                        slashCount += multiShotCurrentStacks.currentStacks;
                }
            }
        }
    }


    public void ParryingSkillApplied()
    {
        ////  �ĸ� ��ų �÷��̾�Ը� �ݿ�
        //Skill ParryingSkill = playerSkillHandler.acquiredSkills.Find(rt => rt.appliesParrying);
        //RuntimeSkill ParryingSkillCurrentStacks = playerSkillHandler.trackingList.Find(rt => rt.skill.appliesParrying);
        //if (ParryingSkillCurrentStacks != null && whoIsWeilding == WhoIsWeilding.Player)
        //{
        //    if (ParryingSkillCurrentStacks.currentStacks <= ParryingSkill.maxStacks)
        //    {
        //        WeaponSize = 1f;
        //        WeaponSize += (weaponSizeMultiplier * ParryingSkillCurrentStacks.currentStacks);
        //        collideBoxSize = SetColliderBoxSize();
        //    }
        //    else
        //        return;
        //}
    }

    public override void Rotate(bool isLeft)
    {
        transform.eulerAngles = isLeft ? new Vector3(0, 180, 0) : Vector3.zero;
    }

    private Vector2 SetColliderBoxSize()
    {
        return collideBoxSize = collideBoxSize * WeaponSize;
    }
}