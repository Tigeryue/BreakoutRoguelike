using UnityEngine;

[CreateAssetMenu(fileName = "BallSizePerk", menuName = "Perks/Ball/BallSizePerk")]
public class BallSizePerk : Perk
{
    public float sizeMultiplier = 1.5f;
    
    public override void ApplyEffect(GameObject target)
    {
        if (target.TryGetComponent<Transform>(out var transform))
        {
            transform.localScale *= sizeMultiplier;
        }
    }
    
    public override void RemoveEffect(GameObject target)
    {
        if (target.TryGetComponent<Transform>(out var transform))
        {
            transform.localScale /= sizeMultiplier;
        }
    }
}

[CreateAssetMenu(fileName = "BallInvinciblePerk", menuName = "Perks/Ball/BallInvinciblePerk")]
public class BallInvinciblePerk : Perk
{
    public override void ApplyEffect(GameObject target)
    {
        if (target.TryGetComponent<BallController>(out var ball))
        {
            ball.SetInvincible(true);
        }
    }
    
    public override void RemoveEffect(GameObject target)
    {
        if (target.TryGetComponent<BallController>(out var ball))
        {
            ball.SetInvincible(false);
        }
    }
} 