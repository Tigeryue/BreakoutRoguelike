using UnityEngine;

[CreateAssetMenu(fileName = "BrickExplosionPerk", menuName = "Perks/Brick/BrickExplosionPerk")]
public class BrickExplosionPerk : Perk
{
    public float explosionRadius = 2f;
    public float explosionForce = 5f;
    
    public override void ApplyEffect(GameObject target)
    {
        if (target.TryGetComponent<Brick>(out var brick))
        {
            // 在砖块被销毁时触发爆炸
            brick.OnBrickDestroyed += Explode;
        }
    }
    
    public override void RemoveEffect(GameObject target)
    {
        if (target.TryGetComponent<Brick>(out var brick))
        {
            brick.OnBrickDestroyed -= Explode;
        }
    }
    
    private void Explode(Vector2 position)
    {
        // 获取爆炸范围内的所有物体
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, explosionRadius);
        
        foreach (Collider2D collider in colliders)
        {
            if (collider.TryGetComponent<Rigidbody2D>(out var rb))
            {
                // 计算爆炸力方向
                Vector2 direction = (collider.transform.position - (Vector3)position).normalized;
                rb.AddForce(direction * explosionForce, ForceMode2D.Impulse);
            }
        }
    }
} 