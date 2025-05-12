using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockwaveDamage : MonoBehaviour
{
    [SerializeField] private float radius = 3f;
    [SerializeField] private float damage = 3f;
    [SerializeField] private float delayBeforeDamage = 0.5f; // ��ƼŬ ������ �ð��� ���߱�
    [SerializeField] private float initialSpeed = 5f;
    [SerializeField] private float slowDownRate = 2f;
    [SerializeField] private LayerMask targetLayer;

    [Header("ī�޶� ��鸲")]
    [SerializeField] private float shakeIntensity = 0.4f;
    [SerializeField] private float shakeDuration = 0.25f;

    private Transform playerTarget;
    private float currentSpeed;

    private void Start()
    {
        currentSpeed = initialSpeed;

        // �÷��̾� ��Ī
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTarget = playerObj.transform;
        }

        Debug.Log("Start called");

        StartCoroutine(DelayedDamage());
    }

    private void Update()
    {
        // �÷��̾� ����
        if (playerTarget != null)
        {
            Vector2 dir = (playerTarget.position - transform.position).normalized;
            transform.position += (Vector3)dir * currentSpeed * Time.deltaTime;

            currentSpeed = Mathf.Max(0f, currentSpeed - slowDownRate * Time.deltaTime);

            Debug.Log("����� �̵�");
        }
        else 
        {
            Debug.Log("playerTarget�� null�Դϴ�");
        }
    }

    private IEnumerator DelayedDamage()
    {
        yield return new WaitForSeconds(delayBeforeDamage);

        // ���� ������
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius, targetLayer);
        foreach (var hit in hits)
        {
            ResourceController rc = hit.GetComponent<ResourceController>();
            if (rc != null)
            {
                rc.ChangeHealth(-damage);
                Debug.Log("����� �߻� " + rc.name);
            }
        }

        // ī�޶� ��鸲
        CameraShaker.Instance?.Shake(shakeIntensity, shakeDuration);

        Destroy(gameObject);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawWireSphere(transform.position, radius);
    }
#endif
}
