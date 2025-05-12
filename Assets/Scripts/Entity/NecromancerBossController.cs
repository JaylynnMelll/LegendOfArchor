using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class NecromancerBossController : BaseController, IEnemy
{
    [Header("페이즈 설정")]
    [SerializeField] private int phaseTwoHealthThreshold = 50;
    private bool isPhaseTwo = false;
    [Header("스켈레톤 소환")]
    [SerializeField] private GameObject skeletonPrefab;
    [SerializeField] private float summonInterval = 5f;
    [SerializeField] private int skeletonCountPerSummon = 3;
    [Header("충격파")]
    [SerializeField] private GameObject shockwaveEffectPrefab;
    [SerializeField] private float shockwaveCooldown = 8f;
    [Header("투사체 발사")]
    [SerializeField] private float projectileAttackInterval = 3f;
    [SerializeField] private float followRange = 15f;
    private RangeWeaponHandler rangeWeaponHandler;
    private ResourceController resourceController;
    private EnemyManager enemyManager;
    private Transform target;

    public bool IsSummoned => false;

    // 임시 인터페이스 구현
    GameObject IEnemy.gameObject { get => gameObject; set => throw new System.NotImplementedException(); }

    public void InitEnemy(EnemyManager manager, Transform player)
    {
        enemyManager = manager;
        target = player;
        StartCoroutine(SummonSkeletons());
        StartCoroutine(ShockwaveRoutine());
        StartCoroutine(ProjectileAttack());
    }
    protected override void Awake()
    {
        base.Awake();
        resourceController = GetComponent<ResourceController>();
        rangeWeaponHandler = GetComponentInChildren<RangeWeaponHandler>();
    }
    private void Update()
    {
        if (!isPhaseTwo && resourceController.CurrentHealth <= phaseTwoHealthThreshold)
        {
            EnterPhaseTwo();
        }
    }
    private IEnumerator ProjectileAttack()
    {
        while (true)
        {
            yield return new WaitForSeconds(projectileAttackInterval);
            if (rangeWeaponHandler != null)
            {
                if (target != null)
                {
                    lookDirection = (target.position - transform.position).normalized;
                }
                Debug.Log("투사체 발사");
                rangeWeaponHandler.Attack();
            }
            else
            {
                Debug.LogWarning("rangeWeaponHandler가 설정되지 않음");
            }
        }
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
                  (1 << LayerMask.NameToLayer("Level")) | layerMaskTarget);
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
    private void EnterPhaseTwo()
    {
        isPhaseTwo = true;
        // 2페이즈 진입
        Debug.Log("네크로맨서 보스: 2페이즈 돌입");
    }
    private IEnumerator SummonSkeletons()
    {
        while (true)
        {
            yield return new WaitForSeconds(summonInterval);
            for (int i = 0; i < skeletonCountPerSummon; i++)
            {
                Vector2 spawnPos = (Vector2)transform.position + Random.insideUnitCircle * 2f;
                GameObject skeleton = Instantiate(skeletonPrefab, spawnPos, Quaternion.identity);
                IEnemy enemy = skeleton.GetComponent<IEnemy>();
                if (enemy != null)
                {
                    enemy.InitEnemy(enemyManager, target);

                    // 소환몬스터로 체크
                    EnemyController enemyController = skeleton.GetComponent<EnemyController>();
                    enemyController.SummonCheck();
                }
                else
                {
                    Debug.LogWarning($"스켈레톤 프리팹에 IEnemy를 구현한 컴포넌트가 없습니다: {skeleton.name}");
                }
            }
        }
    }
    private IEnumerator ShockwaveRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(shockwaveCooldown);
            if (isPhaseTwo && shockwaveEffectPrefab != null)
            {
                Instantiate(shockwaveEffectPrefab, transform.position, Quaternion.identity);
                Debug.Log("충격파 발동");
            }
        }
    }
    public override void Died()
    {
        base.Died();
        enemyManager.RemoveEnemyOnDeath(this);
    }
}