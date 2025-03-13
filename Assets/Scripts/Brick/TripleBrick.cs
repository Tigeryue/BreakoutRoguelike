using UnityEngine;

public class TripleBrick : Brick
{
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    public Color hitColor;
    private bool isHit = false;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        hitColor = new Color(0, 1, 0);
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball") && !isHit)
        {
            spriteRenderer.color = hitColor;
            // 通知GameManager记录TripleBrick的击中
            GameManager.Instance.OnTripleBrickHit();
            
            // 不调用基类的碰撞处理，这样就不会销毁砖块
            // base.OnCollisionEnter2D(collision);
            isHit = true;
        }
    }
} 