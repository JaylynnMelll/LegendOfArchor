using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallBounce : MonoBehaviour
{
    [SerializeField] [Range (0f, 1f)] private float bounceForce = 0.75f; // BounceForce (0 ~ 1)
    [SerializeField] private LayerMask levelCollisionLayer;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // �浹�� ��� ������Ʈ�� Wall ���̾��� ���
        if (collision.gameObject.layer == LayerMask.NameToLayer("Level"))
        {
            // �浹�� ��� ������Ʈ�� Normal ���͸� ����.
            Vector2 collisionNormal = (transform.position - collision.transform.position).normalized;

            // �浹�� ��� ������Ʈ�� Normal ���͸� �������� �ݻ�� ���͸� ����.
            Vector2 reflectedVelocity = Vector2.Reflect(rb.velocity, collisionNormal);

            // �ݻ�� ���Ϳ� BounceForce�� ���Ͽ� �ӵ��� ������.
            rb.velocity = reflectedVelocity * bounceForce;
        }
    }

}
