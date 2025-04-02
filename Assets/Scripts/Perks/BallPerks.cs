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

[CreateAssetMenu(fileName = "BulletBallPerk", menuName = "Perks/Ball/BulletBallPerk")]
public class BulletBallPerk : Perk
{
    public float fireInterval = 2f;  // 发射间隔
    public float bulletSpeed = 10f;  // 子弹速度
    public int bulletHealth = 3;     // 子弹生命值
    public GameObject bulletBallPrefab;  // 子弹球预制体
    
    public override void ApplyEffect(GameObject target)
    {
        if (target.TryGetComponent<BallController>(out var ballController))
        {
            ballController.EnableBulletBall(this);
        }
    }
    
    public override void RemoveEffect(GameObject target)
    {
        if (target.TryGetComponent<BallController>(out var ballController))
        {
            ballController.DisableBulletBall();
        }
    }
}

[CreateAssetMenu(fileName = "BulletBallFireRatePerk", menuName = "Perks/Ball/BulletBallFireRatePerk")]
public class BulletBallFireRatePerk : Perk
{
    public float fireIntervalReduction = 0.1f;  // 发射间隔减少值
    
    public override void ApplyEffect(GameObject target)
    {
        // 找到所有 BulletBallPerk 并减少它们的发射间隔
        var bulletBallPerks = FindObjectsOfType<BulletBallPerk>();
        foreach (var perk in bulletBallPerks)
        {
            perk.fireInterval = Mathf.Max(0.1f, perk.fireInterval - fireIntervalReduction);
        }
    }
    
    public override void RemoveEffect(GameObject target)
    {
        // 找到所有 BulletBallPerk 并恢复它们的发射间隔
        var bulletBallPerks = FindObjectsOfType<BulletBallPerk>();
        foreach (var perk in bulletBallPerks)
        {
            perk.fireInterval += fireIntervalReduction;
        }
    }
}

[CreateAssetMenu(fileName = "BulletBallSpeedPerk", menuName = "Perks/Ball/BulletBallSpeedPerk")]
public class BulletBallSpeedPerk : Perk
{
    public float speedIncrease = 1f;  // 速度增加值
    
    public override void ApplyEffect(GameObject target)
    {
        // 找到所有 BulletBallPerk 并增加它们的子弹速度
        var bulletBallPerks = FindObjectsOfType<BulletBallPerk>();
        foreach (var perk in bulletBallPerks)
        {
            perk.bulletSpeed += speedIncrease;
        }
    }
    
    public override void RemoveEffect(GameObject target)
    {
        // 找到所有 BulletBallPerk 并恢复它们的子弹速度
        var bulletBallPerks = FindObjectsOfType<BulletBallPerk>();
        foreach (var perk in bulletBallPerks)
        {
            perk.bulletSpeed -= speedIncrease;
        }
    }
}


