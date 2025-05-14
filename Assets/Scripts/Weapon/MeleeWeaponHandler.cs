using UnityEngine;

public class MeleeWeaponHandler : WeaponHandler
{
    [Header("Melee Attack Info")]
    public Vector2 collideBoxSize = Vector2.one;

    [Header("SpinSlash Settings")]
    public bool useSpinningProjectile = false;
    public GameObject spinSlashPrefab;
    public LayerMask wallLayer;

    protected override void Start()
    {
        base.Start();
        collideBoxSize = collideBoxSize * WeaponSize;
    }

    public override void Attack()
    {
        base.Attack();

        if (IsPlayerWeapon && useSpinningProjectile && spinSlashPrefab != null)
        {
            int slashCount = 1;

            //  멀티샷 스킬 반영
            var multiSkill = PlayerSkillHandler.acquiredSkills.Find(s => s.appliesMultiShot);
            if (multiSkill != null)
            {
                var runtime = PlayerSkillHandler.trackingList.Find(rt => rt.skill == multiSkill);
                if (runtime != null)
                    slashCount += runtime.currentStacks;
            }

            //  태양처럼 360도 고르게 퍼짐
            float totalAngle = 360f;
            float angleStep = slashCount > 0 ? totalAngle / slashCount : 0f;

            for (int i = 0; i < slashCount; i++)
            {
                float angleOffset = i * angleStep;

                GameObject spinObj = Instantiate(spinSlashPrefab, transform.position, Quaternion.identity);
                spinObj.transform.SetParent(transform); // 회전 중심 = 플레이어

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

        // 기존 박스 판정 근접 공격
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
        }
    }

    public override void Rotate(bool isLeft)
    {
        transform.eulerAngles = isLeft ? new Vector3(0, 180, 0) : Vector3.zero;
    }
}