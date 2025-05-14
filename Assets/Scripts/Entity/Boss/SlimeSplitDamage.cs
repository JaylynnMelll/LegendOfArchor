using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeSplitDamageZone : MonoBehaviour
{
    [SerializeField] private float damage = 1f;
    [SerializeField] private float lifetime = 0.5f;

    [Header("슬로우 효과")]
    [SerializeField] private float slowMultiplier = 0.5f;
    [SerializeField] private float slowDuration = 2f;

    private void Start()
    {
        Destroy(gameObject, lifetime); // 파티클 수명
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        var resource = other.GetComponent<ResourceController>();
        var stat = other.GetComponent<StatHandler>();
        var status = other.GetComponent<PlayerDebuffHandler>();

        if (resource != null)
            resource.ChangeHealth(-damage);

        if (stat != null && status != null)
            status.ApplySlow(stat, slowMultiplier, slowDuration);
    }
}
