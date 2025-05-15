using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeBossController : BaseController, IEnemy

{
    [SerializeField] private GameObject bossSlimeSplit; // 분열 시 생성될 보스 프리팹
    [SerializeField] private GameObject bossSlimeSplitEffect; // 분열 이펙트
    [SerializeField] private AudioClip splitSound;
    [SerializeField] private int maxSplitCount = 4; // 최대 분열 횟수
    [SerializeField] private int splitCount = 0; // 현재 분열 횟수
    [SerializeField] private int splitSpawnCount = 2; // 한 번에 나오는 분열 수


    [SerializeField] private float chargeSpeed = 10f; // 몸통박치기 속도
    [SerializeField] private float chargeDuration = 1f; // 돌진 지속 시간
    [SerializeField] private float chargeCooldown = 3f; // 돌진 간격
    [SerializeField] private GameObject chargeWarningBoxPrefab;
    private GameObject currentWarningBox;

    private bool isCharging = false;
    private Transform target; // 플레이어 추적

    private EnemyManager enemyManager;

    private Animator animator;

    private Coroutine chargeCoroutine;

    public bool IsSummoned => isSummoned;
    public bool isSummoned = false;

    // 이 슬라임인지 분열체인지 판별(InitEnemy 때문에)
    private bool isSplitSpawn = false;

    // 임시 인터페이스 구현
    GameObject IEnemy.gameObject { get => gameObject; set => throw new System.NotImplementedException(); }

    public void InitEnemy(EnemyManager manager, Transform player, bool isSplitSpawn = false)
    {
        target = player;
        enemyManager = manager;

        // 스테이지에 비례해 적의 체력이 늘어남
        // Split 내부 호출로는 체력이 증가하지 않게끔 함
        if (!isSplitSpawn && StageManager.instance.currentStage >= 2)
        {
            statHandler.Health = statHandler.Health + StageManager.instance.currentStage;
            resourceController.SetHealth(statHandler.Health);
            Debug.Log($"InitEnemy 이후 체력: {statHandler.Health}");
        }

        if (chargeCoroutine == null)
        {
            chargeCoroutine = StartCoroutine(ChargeRoutine());
        }
    }

    public void SummonCheck()
    {
        isSummoned = false;
    }
    protected override void Awake()
    {
        base.Awake();
        _rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

    }

    protected override void FixedUpdate()
    {
        if (isCharging) return;
        base.FixedUpdate();
    }


    public override void Died()
    {
        if (splitCount < maxSplitCount)
        {
            Split();
        }
        else
        {
            enemyManager.aliveEnemyCount--;
            enemyManager.RemoveEnemyOnDeath(this);
            base.Died();
            base.OnDeathComplete();

            if (chargeCoroutine != null)
            {
                StopCoroutine(chargeCoroutine);
                chargeCoroutine = null;
            }

            if (currentWarningBox != null)
                Destroy(currentWarningBox);
        }
    }

    private void Split()
    {
        Debug.Log($"슬라임 분열, 현재 분열 횟수 {splitCount}");

        if (bossSlimeSplitEffect != null)
        {
            GameObject fx = Instantiate(bossSlimeSplitEffect, transform.position, Quaternion.identity);

            // 분열 횟수에 따라 파티클 크기 조정
            var scaler = fx.GetComponent<SlimeSplitEffectScaler>();
            if (scaler != null)
            {
                scaler.ApplyScaleBySplitCount(splitCount);
            }
        }

        // 분열 사운드 재생
        if (splitSound != null)
            AudioSource.PlayClipAtPoint(splitSound, transform.position);

        // 분열된 개체 생성
        for (int i = 0; i < splitSpawnCount; i++)
        {
            Vector2 spawnPos = (Vector2)transform.position + Random.insideUnitCircle * 0.5f;
            GameObject split = Instantiate(bossSlimeSplit, spawnPos, Quaternion.identity);

            SlimeBossController splitcontroller = split.GetComponent<SlimeBossController>();

                if (splitcontroller != null)
                {
                    // EnemyManager와 플레이어 Transform을 전달하여 초기화
                    splitcontroller.InitEnemy(FindObjectOfType<EnemyManager>(), FindObjectOfType<GameManager>().player.transform, true);

                    splitcontroller.InitSplit(splitCount + 1);

                    // 체력바 생성
                    var resource = split.GetComponent<ResourceController>();
                    GameObject hpBar = GameManager.Instance.CreateEnemyHPBar(split.transform, resource);
                    splitcontroller.ConnectedHPBar = hpBar;
                }
                enemyManager.aliveEnemyCount++;
         }

        enemyManager.aliveEnemyCount--;
        enemyManager.RemoveEnemyOnDeath(this);
        Destroy(gameObject); // 분열 전 슬라임 제거

        if (currentWarningBox != null)
            Destroy(currentWarningBox); // 경고 박스 제거
    }

    public void InitSplit(int newSplitCount)
    {
        Debug.Log($"InitSplit 이전 체력: {statHandler.Health}");
        this.splitCount = newSplitCount;

        // 체력 줄이기
        statHandler.Health = Mathf.Max(5, statHandler.Health / 2);
        resourceController.SetHealth(statHandler.Health);

        // 크기 줄이기
        float splitScale = Mathf.Max(1f, 5f - newSplitCount);
        transform.localScale = new Vector3(splitScale, splitScale, 1f);

        // 이동속도 증가
        statHandler.Speed += 0.5f;

        // 분열 이후 돌진패턴 시작
        if (chargeCoroutine == null)
        {
            chargeCoroutine = StartCoroutine(ChargeRoutine());
        }

    }

    public IEnumerator ChargeRoutine()
    {
        Debug.Log("ChargeRoutine 시작");

        while (true)
        {
            yield return new WaitForSeconds(chargeCooldown);

            if (target == null || isCharging) continue;

            Debug.Log("돌진 시작");

            // 돌진 경고 박스 표시
            Vector2 direction = (target.position - transform.position).normalized;
            Vector2 start = transform.position;

            // 돌진 거리
            float chargeRange = 10f;

            // 슬라임 스케일 반영
            float slimeScaleY = transform.localScale.y;
            float slimeScaleX = transform.localScale.x;

            // 박스 중심 = 돌진 거리의 절반 지점
            Vector2 center = start + direction * (chargeRange / 2f);

            // 회전 각도 계산
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            if (chargeWarningBoxPrefab != null)
            {
                currentWarningBox = Instantiate(chargeWarningBoxPrefab, center, Quaternion.identity);

                // 방향으로 회전
                currentWarningBox.transform.rotation = Quaternion.Euler(0, 0, angle);

                // 스케일 조정: 길이 = 돌진 거리, 높이 = 슬라임 크기 기반
                float width = chargeRange;
                float height = 1f * slimeScaleY;  // 원한다면 1.2f 등으로 살짝 더 키워도 OK

                currentWarningBox.transform.localScale = new Vector3(width, height, 1f);
            }

            yield return new WaitForSeconds(0.8f); // 예고 시간

            if (currentWarningBox != null)
                Destroy(currentWarningBox);

            // 애니메이션 트리거 추가 예정
            // animator?.SetTrigger("ChargeStart");

            isCharging = true;
            float elapsed = 0f;

            while (elapsed < chargeDuration)
            {
                _rigidbody.velocity = direction * chargeSpeed;
                elapsed += Time.deltaTime;
                yield return null;
            }

            _rigidbody.velocity = Vector2.zero;
            isCharging = false;

            // 충돌 다시 활성화
            Collider2D myCollider = GetComponent<Collider2D>();
            Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(transform.position, 5f, LayerMask.GetMask("Enemy"));

            foreach (var col in nearbyEnemies)
            {
                if (col != myCollider)
                {
                    Physics2D.IgnoreCollision(myCollider, col, false);
                }
            }

            // 애니메이션 트리거 추가 예정
            // animator?.SetTrigger("ChargeEnd");
        }
    }

    // 충돌 피해 판정 메서드
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isCharging && collision.collider.CompareTag("Enemy"))
        {
            // 돌진 중 슬라임끼리 충돌 무시
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), collision.collider, true);
        }

        if (isCharging && collision.collider.CompareTag("Player"))
        {
            var playerResource = collision.collider.GetComponent<ResourceController>();
            if (playerResource != null)
            {
                playerResource.ChangeHealth(-1f); // 충돌 시 데미지
            }
        }
    }
}