using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PerkManager : MonoBehaviour
{
    [Header("Perk 设置")]
    [SerializeField] private List<Perk> allPerks = new List<Perk>();  // 所有可用的 perk
    [SerializeField] private int perksToShow = 3;                    // 每次显示多少个 perk 选项
    [SerializeField] private float perkDuration = 10f;               // 临时 perk 的默认持续时间
    
    [Header("UI 引用")]
    [SerializeField] private GameObject perkSelectionUI;             // Perk 选择界面
    [SerializeField] private Transform perkButtonContainer;          // Perk 按钮容器
    
    private List<Perk> currentActivePerks = new List<Perk>();
    private GameObject ball;
    private bool isPerkSelectionActive = false;

    private void Start()
    {
        ball = GameObject.FindGameObjectWithTag("Ball");
        if (ball == null)
        {
            Debug.LogError("找不到小球对象！");
        }
    }

    public void ShowPerkSelection()
    {
        if (isPerkSelectionActive) return;
        
        isPerkSelectionActive = true;
        Time.timeScale = 0f; // 暂停游戏
        
        // 随机选择指定数量的 perk
        var availablePerks = allPerks.OrderBy(x => Random.value).Take(perksToShow).ToList();
        
        // 显示 perk 选择界面
        perkSelectionUI.SetActive(true);
        
        // 创建 perk 选择按钮
        foreach (var perk in availablePerks)
        {
            // 这里需要实现 UI 按钮的创建和设置
            // 可以使用 UI 预制体或动态创建
            CreatePerkButton(perk);
        }
    }

    private void CreatePerkButton(Perk perk)
    {
        // 这里需要实现具体的 UI 按钮创建逻辑
        // 可以使用 UI 预制体或动态创建
        // 按钮点击时调用 ApplyPerk(perk)
    }

    public void ApplyPerk(Perk perk)
    {
        if (perk.affectsBall)
        {
            perk.ApplyEffect(ball);
            
            // 如果是临时 perk，设置定时器
            if (perk.duration > 0)
            {
                StartCoroutine(RemovePerkAfterDelay(perk, perk.duration));
            }
        }
        
        currentActivePerks.Add(perk);
        
        // 关闭 perk 选择界面
        perkSelectionUI.SetActive(false);
        Time.timeScale = 1f;
        isPerkSelectionActive = false;
    }

    private System.Collections.IEnumerator RemovePerkAfterDelay(Perk perk, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        if (perk.affectsBall)
        {
            perk.RemoveEffect(ball);
        }
        
        currentActivePerks.Remove(perk);
    }

    // 在 GameManager 中调用此方法，当达到特定分数时
    public void CheckAndShowPerks(int currentScore, int scoreThreshold)
    {
        if (currentScore % scoreThreshold == 0)
        {
            ShowPerkSelection();
        }
    }
} 