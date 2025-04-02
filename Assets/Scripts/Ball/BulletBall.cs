using UnityEngine;
using System.Collections;

public class BulletBall : MonoBehaviour
{
    private Rigidbody2D rb;
    private float maxSpeed = 10f;
    public int maxHealth = 3;
    private int currentHealth;
    private float ballRadius;
    private CircleCollider2D circleCollider;
    private float lifetime = 3f;  // 存活时间限制

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        circleCollider = GetComponent<CircleCollider2D>();
        ballRadius = circleCollider.radius;
        currentHealth = maxHealth;
    }

    private void FixedUpdate()
    {
        // 限制最大速度
        Vector2 currentVelocity = rb.velocity;
        if (currentVelocity.magnitude > maxSpeed)
        {
            rb.velocity = currentVelocity.normalized * maxSpeed;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 检查碰撞对象是否为砖块
        Brick brick = collision.gameObject.GetComponent<Brick>();
        if (brick != null)
        {
            // 减少生命值
            currentHealth--;
            
            // 如果生命值耗尽，销毁子弹球
            if (currentHealth <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    public void Initialize(Vector2 velocity)
    {
        rb.velocity = velocity;
        // 禁用碰撞器0.1秒
        StartCoroutine(DisableColliderTemporarily());
        // 设置自动销毁
        Destroy(gameObject, lifetime);
    }

    private System.Collections.IEnumerator DisableColliderTemporarily()
    {
        circleCollider.enabled = false;
        yield return new WaitForSeconds(0.1f);
        circleCollider.enabled = true;
    }
} 