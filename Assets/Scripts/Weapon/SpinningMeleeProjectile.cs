using UnityEngine;

public class SpinningMeleeProjectile : MonoBehaviour
{
    public float duration = 0.5f;
    public float rotationSpeed = 720f; // 초당 회전 각도 (degrees per second)
    public float radius = 1.5f;        // 캐릭터로부터 떨어질 거리(orbit radius)
    public float damage = 5f;
    public LayerMask enemyLayer;

    private float timeElapsed = 0f;
    private Transform center;  // 회전의 중심으로 사용할 캐릭터의 Transform

    void Start()
    {
        // 부모가 아닌, 태그 "Player"를 가진 캐릭터를 중심으로 설정
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            center = player.transform;
        }
        else
        {
            Debug.LogError("Player를 찾을 수 없습니다! 캐릭터가 'Player' 태그를 가지고 있는지 확인하세요.");
            // 임시 center를 생성합니다.
            GameObject tempCenter = new GameObject("TempCenter");
            tempCenter.transform.position = transform.position - new Vector3(radius, 0, 0);
            center = tempCenter.transform;
        }

        // 투사체의 시작 위치를 캐릭터의 오른쪽(혹은 원하는 방향)에서 radius 거리 떨어지도록 설정합니다.
        transform.position = center.position + new Vector3(radius, 0, 0);
    }

    void Update()
    {
        timeElapsed += Time.deltaTime;
        if (timeElapsed > duration)
        {
            Destroy(gameObject);
            return;
        }

        // center를 기준으로 회전
        transform.RotateAround(center.position, Vector3.forward, rotationSpeed * Time.deltaTime);

        // 충돌 검사: 주변에 있는 적에게 데미지 적용
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.3f, enemyLayer);
        foreach (var hit in hits)
        {
            var target = hit.GetComponent<ResourceController>();
            if (target != null)
            {
                target.ChangeHealth(-damage);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.3f);
    }
}