using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeBossController : BaseController, IEnemy

{

    [SerializeField] private GameObject bossSlimeSplit; // 분열 시 생성될 보스 프리팹
    [SerializeField] private GameObject bossSlimeSplitEffect; // 분열 이펙트
    [SerializeField] private int maxSplitCount = 4; // 최대 분열 횟수
    [SerializeField] private int splitCount = 0; // 현재 분열 횟수
    [SerializeField] private int splitSpawnCount = 2; // 한 번에 나오는 분열 수


    [SerializeField] private float chargeSpeed = 10f; // 몸통박치기 속도
    [SerializeField] private float chargeDuration = 1f; // 돌진 지속 시간
    [SerializeField] private float chargeCooldown = 3f; // 돌진 간격

    private bool isCharging = false;
    private Transform target; // 플레이어 추적

    private ResourceController resourceController;
    private LineRenderer lineRenderer;
    private Animator animator;

    public void InitEnemy(EnemyManager manager, Transform player)
    {
        target = player;
        StartCoroutine(ChargeRoutine());
    }
    protected override void Awake()
    {
        base.Awake();
        _rigidbody = GetComponent<Rigidbody2D>();
        resourceController = GetComponent<ResourceController>();
        lineRenderer = GetComponent<LineRenderer>();
        animator = GetComponent<Animator>();

        if (lineRenderer != null)
        {
            lineRenderer.enabled = false;
            lineRenderer.positionCount = 2;
            lineRenderer.startWidth = 0.1f;
            lineRenderer.endWidth = 0.1f;
        }
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
            base.Died();
        }
    }

    private void Split()
    {
        Debug.Log($"슬라임 분열, 현재 분열 횟수 {splitCount}");

        // 슬라임 분열 이펙트
        if (bossSlimeSplitEffect != null)
        {
            Instantiate(bossSlimeSplitEffect, transform.position, Quaternion.identity);
        }

        // 분열된 개체 생성
        for (int i = 0; i < splitSpawnCount; i++)
        {
            Vector2 spawnPos = (Vector2)transform.position + Random.insideUnitCircle * 0.5f;
            GameObject split = Instantiate(bossSlimeSplit, spawnPos, Quaternion.identity);

            SlimeBossController splitcontroller = split.GetComponent<SlimeBossController>();
            if (splitcontroller != null)
            {
                // EnemyManager와 플레이어 Transform을 전달하여 초기화
                splitcontroller.InitEnemy(GetComponentInParent<EnemyManager>(), FindObjectOfType<GameManager>().player.transform);
                splitcontroller.InitSplit(splitCount + 1);
            }
        }

        Destroy(gameObject); // 분열 전 슬라임 제거
    }

    public void InitSplit(int newSplitCount)
    {
        this.splitCount = newSplitCount;

        // 체력 줄이기
        statHandler.Health = Mathf.Max(5, statHandler.Health - 20);
        resourceController.SetHealth(statHandler.Health);

        // 크기 줄이기
        float splitScale = Mathf.Max(1f, 5f - newSplitCount);
        transform.localScale = new Vector3(splitScale, splitScale, 1f);

        // 이동속도 증가
        statHandler.Speed += 0.5f;

        // 분열 이후 돌진패턴 시작
        StartCoroutine(ChargeRoutine());

    }

    public IEnumerator ChargeRoutine()
    {
        Debug.Log("ChargeRoutine 시작");

        while (true)
        {
            yield return new WaitForSeconds(chargeCooldown);

            if (target == null || isCharging) continue;

            Debug.Log("돌진 시작");

            // 돌진 예고선 표시
            Vector2 direction = (target.position - transform.position).normalized;
            Vector2 start = transform.position;
            Vector2 end = start + direction * 5f;

            if (lineRenderer != null)
            {
                lineRenderer.SetPosition(0, start);
                lineRenderer.SetPosition(1, end);
                lineRenderer.startColor = new Color(1, 0, 0, 0.6f);
                lineRenderer.endColor = new Color(1, 0, 0, 0.1f);
                lineRenderer.enabled = true;
            }

            yield return new WaitForSeconds(0.8f); // 예고 시간

            if (lineRenderer != null) lineRenderer.enabled = false;

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

            // 애니메이션 트리거 추가 예정
            // animator?.SetTrigger("ChargeEnd");
        }
    }

    // 충돌 피해 판정 메서드
    private void OnCollisionEnter2D(Collision2D collision)
    {
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