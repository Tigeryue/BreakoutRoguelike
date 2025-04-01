using UnityEngine;
using System.Collections.Generic;

public class ChildBall : BallController
{
    private List<Perk> inheritedPerks = new List<Perk>();
    
    public void Initialize(Vector2 velocity)
    {
        // 初始化必要的组件
        Start();
        
        // 设置初始速度
        rb.velocity = velocity;
        
        // 动态设置必要的组件和参数
        SetupComponents();
        
        // 禁用拖拽功能
        DisableDragControl();
        
        // 注册到 PerkManager
        var perkManager = FindObjectOfType<PerkManager>();
        // 继承父球的所有 perk
        inheritedPerks = perkManager.GetCurrentActivePerks();

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
    
    private void SetupComponents()
    {
        // 动态设置必要的组件
        dragLine = gameObject.AddComponent<LineRenderer>();
        ballTracerLine = gameObject.AddComponent<LineRenderer>();
        
        // 设置必要的参数
        maxSpeed = 5f;
        maxDragDistance = 0.1f;
        speedFactor = 10f;
        timeSlowFactor = 0.3f;
        numPoints = 50;
        timeBetweenPoints = 0.1f;
        collidableLayers = LayerMask.GetMask("Brick", "Wall");
    }
    
    private void DisableDragControl()
    {
        // 禁用拖拽控制
        controlDisabled = true;
        
        // 禁用拖拽相关的组件
        if (dragLine != null)
        {
            dragLine.enabled = false;
        }
        if (ballTracerLine != null)
        {
            ballTracerLine.enabled = false;
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

    public void SetInvincible(bool invincible)
    {
        if(invincible)
        {
            //TODO: 添加无敌效果
        }
        else
        {
            //TODO: 移除无敌效果
        }
    }

    public void SetSpeedMultiplier(float multiplier)
    {
        maxSpeed *= multiplier;
    }
} 