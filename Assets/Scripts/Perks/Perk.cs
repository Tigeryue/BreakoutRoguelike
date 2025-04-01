using UnityEngine;

public abstract class Perk : ScriptableObject
{
    [Header("基础属性")]
    public string perkName;        // Perk 名称
    public string description;     // Perk 描述
    public Sprite icon;            // Perk 图标
    public float duration = 0f;    // 持续时间（0表示永久效果）
    
    [Header("目标类型")]
    public bool affectsBall = true;    // 是否影响小球
    public bool affectsBrick = false;   // 是否影响砖块
    public bool isOneTime = false;  // 是否是一次性 perk
    
    public abstract void ApplyEffect(GameObject target);
    public virtual void RemoveEffect(GameObject target) { }
} 