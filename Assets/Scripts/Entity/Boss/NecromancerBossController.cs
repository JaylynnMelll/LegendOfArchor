using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class NecromancerBossController : BaseController, IEnemy
{
    [Header("������ ����")]
    [SerializeField] private int phaseTwoHealthThreshold = 50;
    private bool isPhaseTwo = false;
    [Header("���̷��� ��ȯ")]
    [SerializeField] private GameObject skeletonPrefab;
    [SerializeField] private float summonInterval = 5f;
    [SerializeField] private int skeletonCountPerSummon = 3;
    [SerializeField] private GameObject summonEffectPrefab;
    [Header("�����")]
    [SerializeField] private GameObject shockwaveEffectPrefab;
    [SerializeField] private float shockwaveCooldown = 8f;
    [Header("����ü �߻�")]
    [SerializeField] private float projectileAttackInterval = 3f;
    [SerializeField] private float followRange = 15f;
    private RangeWeaponHandler rangeWeaponHandler;
    private EnemyManager enemyManager;
    private GameManager gameManager;
    private Transform target;
    public GameObject ConnectedHPBar { get; set; }

    public bool IsSummoned => false;

    private List<GameObject> summonedSkeletons = new();

    // �ӽ� �������̽� ����
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
                Debug.Log("����ü �߻�");
                rangeWeaponHandler.Attack();
            }
            else
            {
                Debug.LogWarning("rangeWeaponHandler�� �������� ����");
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
        // 2������ ����
        Debug.Log("��ũ�θㅁㅇǼ� ����: 2������ ����");
    }
    private IEnumerator SummonSkeletons()
    {
        while (true)
        {
            yield return new WaitForSeconds(summonInterval);

            enemyManager.aliveEnemyCount += skeletonCountPerSummon;

            for (int i = 0; i < skeletonCountPerSummon; i++)
            {
                Vector2 spawnPos = (Vector2)transform.position + Random.insideUnitCircle * 2f;

                // 마법진 이펙트
                GameObject effect = Instantiate(summonEffectPrefab, spawnPos, Quaternion.identity);

                // 스켈레톤 소환 딜레이
                yield return new WaitForSeconds(0.3f);

                GameObject skeleton = Instantiate(skeletonPrefab, spawnPos, Quaternion.identity);

                summonedSkeletons.Add(skeleton);

                IEnemy enemy = skeleton.GetComponent<IEnemy>();
                if (enemy != null)
                {
                    enemy.InitEnemy(enemyManager, target);

                    // ��ȯ���ͷ� üũ
                    EnemyController enemyController = skeleton.GetComponent<EnemyController>();
                    enemyController.SummonCheck();
                }
                else
                {
                    Debug.LogWarning($"���̷��� �����տ� IEnemy�� ������ ������Ʈ�� �����ϴ�: {skeleton.name}");
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
                Vector2 abovePlayerPos = target.position + Vector3.up * 3f;
                Instantiate(shockwaveEffectPrefab, transform.position, Quaternion.identity);
                Debug.Log("����� �ߵ�");
            }
        }
    }
    public override void Died()
    {
        base.Died();

        foreach (var skeleton in summonedSkeletons)
        {
            if (skeleton != null)
            {
                ResourceController resourceController = skeleton.GetComponent<ResourceController>();
                if (resourceController != null)
                    resourceController.ChangeHealth(-999);
                else
                    Destroy(skeleton);
            }
        }

        summonedSkeletons.Clear();

        base.OnDeathComplete();
        enemyManager.RemoveEnemyOnDeath(this);

        enemyManager.aliveEnemyCount--;
    }
}