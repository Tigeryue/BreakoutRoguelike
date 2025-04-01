using UnityEngine;

[CreateAssetMenu(fileName = "BallSizePerk", menuName = "Perks/Ball/BallSizePerk")]
public class BallSizePerk : Perk
{
    public float sizeMultiplier = 1.5f;
    public string perkDescription;  // 添加描述字段
    
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
[CreateAssetMenu(fileName = "BallSpeedPerk", menuName = "Perks/Ball/BallSpeedPerk")]
public class BallSpeedPerk : Perk
{
    public float speedMultiplier = 1.5f;
    
    public override void ApplyEffect(GameObject target)
    {
        if (target.TryGetComponent<BallController>(out var ball))
        {
            ball.SetSpeedMultiplier(speedMultiplier);
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

[CreateAssetMenu(fileName = "BallSplitPerk", menuName = "Perks/Ball/BallSplitPerk")]
public class BallSplitPerk : Perk
{
    public float splitAngle = 30f; // 分裂角度
    
    public override void ApplyEffect(GameObject target)
    {
        if (target.TryGetComponent<BallController>(out var ball))
        {
            ball.Split(splitAngle);
        }
    }
    
    public override void RemoveEffect(GameObject target)
    {
        // 分裂是一次性效果，不需要移除
    }
}


