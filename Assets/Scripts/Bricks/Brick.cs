using UnityEngine;
using System;

public class Brick : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f; // 砖块移动速度
    [SerializeField] private float fadeOutDuration = 0.1f; // 渐变消失持续时间
    [SerializeField] protected bool enableMoveAndFade = true; // 是否启用移动和渐变效果
    
    private bool isMoving = false;
    private Vector2 moveDirection;
    private SpriteRenderer spriteRenderer;
    private float fadeStartTime;
    private float initialAlpha;
    private Rigidbody2D rb;
    private Rigidbody2D ballRb;

    // 添加事件
    public event Action<Vector2> OnBrickDestroyed;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        initialAlpha = spriteRenderer.color.a;
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (isMoving)
        {
            // 移动砖块
            transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
            
            // 处理渐变消失
            float elapsedTime = Time.time - fadeStartTime;
            float alpha = Mathf.Lerp(initialAlpha, 0f, elapsedTime / fadeOutDuration);
            Color newColor = spriteRenderer.color;
            newColor.a = alpha;
            spriteRenderer.color = newColor;

            // 当完全透明时销毁物体
            if (alpha <= 0)
            {
                // 触发销毁事件
                OnBrickDestroyed?.Invoke(transform.position);
                Destroy(gameObject);
            }
        }
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            ballRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (enableMoveAndFade)
            {
                // 使用碰撞前的相对速度来确定方向
                Vector2 relativeVelocity = collision.relativeVelocity;
                moveDirection = relativeVelocity.normalized;
                isMoving = true;
                fadeStartTime = Time.time;
                
                // 禁用物理效果
                if (rb != null)
                {
                    rb.simulated = false;
                }
            }
            else
            {
                // 如果不启用移动和渐变效果，直接销毁
                OnBrickDestroyed?.Invoke(transform.position);
                Destroy(gameObject);
            }
        }
    }
}
