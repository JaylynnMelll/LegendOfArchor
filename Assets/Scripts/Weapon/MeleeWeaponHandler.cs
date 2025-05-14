using UnityEngine;

public class MeleeWeaponHandler : WeaponHandler
{
    [Header("Melee Attack Info")]
    public Vector2 collideBoxSize = Vector2.one;
    public bool useSpinningProjectile = false; // ȸ���� ���� ��� ����
    public GameObject spinSlashPrefab; // ȸ���� ������

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
            GameObject spinObj = Instantiate(spinSlashPrefab, transform.position, Quaternion.identity);
            spinObj.transform.SetParent(transform); // �÷��̾� ���� ȸ��

            var spinComp = spinObj.GetComponent<SpinningMeleeProjectile>();
            if (spinComp != null)
            {
                spinComp.Init(
                    transform,
                    target,
                    WeaponPower,
                    KnockbackPower,
                    KnockbackTime,
                    WeaponRange,
                    GetWeaponSprite()
                );
            }
            return;
        }

        // ���� BoxCast ���
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
        if (isLeft)
            transform.eulerAngles = new Vector3(0, 180, 0);
        else
            transform.eulerAngles = new Vector3(0, 0, 0);
    }
}
