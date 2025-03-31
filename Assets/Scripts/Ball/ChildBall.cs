using UnityEngine;
using System.Collections.Generic;

public class ChildBall : BallController
{
    private List<Perk> inheritedPerks = new List<Perk>();
    
    public void Initialize(Vector2 velocity)
    {
        // 设置初始速度
        rb.velocity = velocity;
        
        // 注册到 PerkManager
        var perkManager = FindObjectOfType<PerkManager>();
        // 继承父球的所有 perk
        inheritedPerks =perkManager.GetCurrentActivePerks();

        if (perkManager != null)
        {
            perkManager.RegisterSplitBall(this);
        }
        
        // 应用所有继承的 perk
        foreach (var perk in inheritedPerks)
        {
            if (perk.affectsBall)
            {
                perk.ApplyEffect(gameObject);
            }
        }
    }
    
    private void OnDestroy()
    {
        // 从 PerkManager 注销
        var perkManager = FindObjectOfType<PerkManager>();
        if (perkManager != null)
        {
            perkManager.UnregisterSplitBall(this);
        }
        
        // 移除所有继承的 perk
        foreach (var perk in inheritedPerks)
        {
            if (perk.affectsBall)
            {
                perk.RemoveEffect(gameObject);
            }
        }
    }
} 