using UnityEngine;

public class ExplosiveBrick : Brick
{
    [Header("爆炸设置")]
    public float explosionThreshold = 100f;  // 爆炸阈值
    public float explosionValue = 0f;        // 当前爆炸值
    public float hitExplosionAdd = 35f;      // 每次击中增加的爆炸值
    public float explosionDecayRate = 10f;   // 每秒衰减的爆炸值
    public float explosionRadius = 3f;       // 爆炸影响半径
    
    [Header("视觉效果")]
    public Color normalColor = Color.white;
    public Color warningColor = Color.yellow;
    public Color criticalColor = Color.red;
    public ParticleSystem explosionEffectPrefab; // 爆炸特效预制体
    private SpriteRenderer spriteRenderer;
    
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = normalColor;
    }
    
    private void Update()
    {
        // 随时间衰减爆炸值
        if (explosionValue > 0)
        {
            explosionValue = Mathf.Max(0, explosionValue - explosionDecayRate * Time.deltaTime);
            UpdateVisualState();
        }
    }
    
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            // 增加爆炸值
            explosionValue += hitExplosionAdd;
            UpdateVisualState();
            
            // 检查是否达到爆炸阈值
            if (explosionValue >= explosionThreshold)
            {
                Explode();
            }
        }
    }
    
    private void UpdateVisualState()
    {
        float percentage = explosionValue / explosionThreshold;
        if (percentage >= 0.8f)
        {
            spriteRenderer.color = criticalColor;
        }
        else if (percentage >= 0.5f)
        {
            spriteRenderer.color = warningColor;
        }
        else
        {
            spriteRenderer.color = normalColor;
        }
    }
    
    private void Explode()
    {
        // 创建爆炸效果
        CreateExplosionEffect();
        
        // 获取爆炸范围内的所有碰撞体
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        
        foreach (Collider2D collider in colliders)
        {
            // 检查是否是砖块
            Brick brick = collider.GetComponent<Brick>();
            if (brick != null && brick != this)
            {
                // 销毁范围内的砖块
                Destroy(brick.gameObject);
            }
        }
        
        // 最后销毁自己
        Destroy(gameObject);
    }
    
    private void CreateExplosionEffect()
    {
        if (explosionEffectPrefab != null)
        {
            // 实例化预制体
            ParticleSystem effect = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
            // 设置自动销毁
            Destroy(effect.gameObject, effect.main.startLifetime.constant);
        }
        else
        {
            Debug.LogWarning("未设置爆炸特效预制体！");
        }
    }
    
    private void OnDrawGizmos()
    {
        // 在编辑器中显示爆炸范围
        Gizmos.color = new Color(1, 0, 0, 0.2f);
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
