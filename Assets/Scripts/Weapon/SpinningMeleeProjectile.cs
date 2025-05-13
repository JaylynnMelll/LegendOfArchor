using UnityEngine;

public class SpinningMeleeProjectile : MonoBehaviour
{
    public float duration = 0.5f;
    public float rotationSpeed = 720f; // �ʴ� ȸ�� ���� (degrees per second)
    public float radius = 1.5f;        // ĳ���ͷκ��� ������ �Ÿ�(orbit radius)
    public float damage = 5f;
    public LayerMask enemyLayer;

    private float timeElapsed = 0f;
    private Transform center;  // ȸ���� �߽����� ����� ĳ������ Transform

    void Start()
    {
        // �θ� �ƴ�, �±� "Player"�� ���� ĳ���͸� �߽����� ����
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            center = player.transform;
        }
        else
        {
            Debug.LogError("Player�� ã�� �� �����ϴ�! ĳ���Ͱ� 'Player' �±׸� ������ �ִ��� Ȯ���ϼ���.");
            // �ӽ� center�� �����մϴ�.
            GameObject tempCenter = new GameObject("TempCenter");
            tempCenter.transform.position = transform.position - new Vector3(radius, 0, 0);
            center = tempCenter.transform;
        }

        // ����ü�� ���� ��ġ�� ĳ������ ������(Ȥ�� ���ϴ� ����)���� radius �Ÿ� ���������� �����մϴ�.
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

        // center�� �������� ȸ��
        transform.RotateAround(center.position, Vector3.forward, rotationSpeed * Time.deltaTime);

        // �浹 �˻�: �ֺ��� �ִ� ������ ������ ����
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