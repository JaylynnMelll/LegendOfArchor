using UnityEngine;

public class SpinningMeleeProjectile : MonoBehaviour
{
    public float duration = 0.5f;
    public float rotationSpeed = 720f; // degrees per second
    public float radius = 1.5f;
    public float damage = 5f;
    public LayerMask enemyLayer;

    private float timeElapsed = 0f;
    private Transform center;
    private Transform visualTransform;

    void Start()
    {
        center = transform.parent; // 캐릭터 중심 기준 회전
        transform.SetParent(null); // 부모에서 분리

        // 자식 Visual을 찾고 위치 조정
        visualTransform = transform.Find("Visual");
        if (visualTransform != null)
        {
            visualTransform.localPosition = Vector3.right * radius;
        }
    }

    void Update()
    {
        timeElapsed += Time.deltaTime;
        if (timeElapsed > duration)
        {
            Destroy(gameObject);
            return;
        }

        transform.RotateAround(center.position, Vector3.forward, rotationSpeed * Time.deltaTime);

        // 충돌 판정
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
