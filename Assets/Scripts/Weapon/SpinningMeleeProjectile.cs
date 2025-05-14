using UnityEngine;
using System.Collections.Generic;

public class SpinningMeleeProjectile : MonoBehaviour
{
    public float duration = 0.5f;
    public float rotationSpeed = 720f;
    public float radius = 1.5f;
    public float damage = 5f;
    public float hitRadius = 0.6f;
    public LayerMask enemyLayer;
    public LayerMask target;

    private float timeElapsed = 0f;
    private Transform center;
    private Transform visualTransform;
    private HashSet<GameObject> alreadyHit = new HashSet<GameObject>();

    public void Init(Transform owner, LayerMask targetMask, float power, float knockback, float knockTime, float range, Sprite weaponSprite)
    {
        this.damage = power;
        this.target = targetMask;

        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null && weaponSprite != null)
        {
            sr.sprite = weaponSprite;
            sr.color = new Color(1f, 1f, 1f, 0.8f); // 약간 반투명 (선택)
        }

        Destroy(gameObject, duration);
    }

    void Start()
    {
        center = transform.parent;
        transform.SetParent(null);

        visualTransform = transform.Find("Visual");
        if (visualTransform != null)
            visualTransform.localPosition = Vector3.right * radius;
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

        if (visualTransform != null)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(visualTransform.position, hitRadius, target);
            foreach (var hit in hits)
            {
                if (!alreadyHit.Contains(hit.gameObject))
                {
                    var rc = hit.GetComponent<ResourceController>();
                    if (rc != null)
                    {
                        rc.ChangeHealth(-damage);
                        alreadyHit.Add(hit.gameObject);
                        Debug.Log($"[SpinSlash] {hit.name}에게 {damage} 데미지!");
                    }
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (visualTransform != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(visualTransform.position, hitRadius);
        }
    }
}
