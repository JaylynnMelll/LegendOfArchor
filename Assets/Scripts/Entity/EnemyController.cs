using System.Collections;
using UnityEngine;

public class EnemyController : BaseController, IEnemy
{
    private EnemyManager enemyManager;
    private Transform target;


    [SerializeField] private float followRange = 15f;

    GameObject IEnemy.gameObject { get => gameObject; set => throw new System.NotImplementedException(); }

    private bool isSummoned = false;
    public bool IsSummoned => isSummoned;

    public void Init(EnemyManager enemyManager, Transform target)
    {
        this.enemyManager = enemyManager;
        this.target = target;
    }

    // �ӽñ���
    public void InitEnemy(EnemyManager enemyManager, Transform target, bool isSplitSpawn = false)
    {
        this.enemyManager = enemyManager;
        this.target = target;
    }

    public void SummonCheck()
    {
        isSummoned = true;
    }

    protected float DistanceToTarget()
    {
        return Vector3.Distance(transform.position, target.position);
    }

    protected override void HandleAction()
    {
        base.HandleAction();

        if (weaponHandler == null || target == null)
        {
            if (!movementDirection.Equals(Vector2.zero)) movementDirection = Vector2.zero;
            return;
        }

        float distance = DistanceToTarget();
        Vector2 direction = DirectionToTarget();

        isAttacking = false;
        if (distance <= followRange)
        {
            lookDirection = direction;

            if (distance <= weaponHandler.WeaponRange)
            {
                int layerMaskTarget = weaponHandler.target;
                RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, weaponHandler.WeaponRange * 1.5f,
                (1 << LayerMask.NameToLayer("Level")) | layerMaskTarget | (1 << LayerMask.NameToLayer("Obstacle")));

                if (hit.collider != null && layerMaskTarget == (layerMaskTarget | (1 << hit.collider.gameObject.layer)))
                {
                    isAttacking = true;
                }

                movementDirection = Vector2.zero;
                return;
            }

            movementDirection = direction;
        }

    }

    protected Vector2 DirectionToTarget()
    {
        return (target.position - transform.position).normalized;
    }

    public override void Died()
    {
        base.Died();
        enemyManager.RemoveEnemyOnDeath(this);
    }

    public override void Reset()
    {
        base.Reset();
    }


}
