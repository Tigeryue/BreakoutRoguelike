using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class PerkManager : MonoBehaviour
{
    [Header("Perk 设置")]
    [SerializeField] private List<Perk> allPerks = new List<Perk>();  // 所有可用的 perk
    [SerializeField] private int perksToShow = 3;                    // 每次显示多少个 perk 选项
    //[SerializeField] private float perkDuration = 10f;               // 临时 perk 的默认持续时间
    
    [Header("UI 引用")]
    [SerializeField] private GameObject perkSelectionUI;             // Perk 选择界面
    //[SerializeField] private Transform perkButtonContainer;          // Perk 按钮容器
    [Header("Perk 按钮预制体")]
    [SerializeField] private List<PerkButton> perkButtons;
    
    private List<Perk> currentActivePerks = new List<Perk>();
    private BallController mainBall;                                // 主球
    private List<MonoBehaviour> splitBalls = new List<MonoBehaviour>();  // 分裂的小球
    private bool isPerkSelectionActive = false;
    private Vector2 savedBallVelocity;                              // 保存小球速度

    private void Start()
    {
        // 找到主球
        var mainBallObj = GameObject.FindGameObjectWithTag("Ball");
        if (mainBallObj != null)
        {
            mainBall = mainBallObj.GetComponent<BallController>();
            if (mainBall == null)
            {
                Debug.LogError("主球上没有 BallController 组件！");
            }
        }
    }

    // 添加分裂的小球
    public void RegisterSplitBall(MonoBehaviour ball)
    {
        if (!splitBalls.Contains(ball))
        {
            splitBalls.Add(ball);
        }
    }

    // 移除分裂的小球
    public void UnregisterSplitBall(MonoBehaviour ball)
    {
        if (splitBalls.Contains(ball))
        {
            splitBalls.Remove(ball);
        }
    }

    public void ShowPerkSelection()
    {
        if (isPerkSelectionActive) return;
        
        isPerkSelectionActive = true;
        
        // 保存当前速度并禁用控制器
        if (mainBall != null)
        {
            savedBallVelocity = mainBall.GetComponent<Rigidbody2D>().velocity;
            mainBall.controlDisabled = true;  // 禁用 BallController
        }
        
        Time.timeScale = 0f; // 暂停游戏
        
        // 随机选择3个perk
        var availablePerks = allPerks.OrderBy(p => Random.value).Take(perksToShow).ToList();
        
        // 显示 perk 选择界面
        perkSelectionUI.SetActive(true);
        
        // 创建 perk 选择按钮
        for (int i = 0; i < availablePerks.Count; i++)
        {
            var perk = availablePerks[i];
            var button = perkButtons[i].GetComponent<Button>();
            
            // 移除所有旧的点击事件
            button.onClick.RemoveAllListeners();
            
            // 添加新的点击事件
            button.onClick.AddListener(() => ApplyPerk(perk));
            perkButtons[i].GetComponent<PerkButton>().Initialize(perk);
        }
    }

    public void ApplyPerk(Perk perk)
    {
        // 只对主球应用新的 perk
        if (mainBall != null && perk.affectsBall)
        {
            // 应用 perk
            perk.ApplyEffect(mainBall.gameObject);
            
            // 只有非一次性 perk 才添加到当前激活列表
            if (!perk.isOneTime)
            {
                currentActivePerks.Add(perk);
                
                // 如果是临时 perk，设置定时器
                if (perk.duration > 0)
                {
                    StartCoroutine(RemovePerkAfterDelay(perk, perk.duration));
                }
            }
        }
        
        // 关闭 perk 选择界面
        perkSelectionUI.SetActive(false);
        Time.timeScale = 1f;
        isPerkSelectionActive = false;
        
        // 恢复保存的速度并重新启用控制器
        if (mainBall != null)
        {
            mainBall.GetComponent<Rigidbody2D>().velocity = savedBallVelocity;
            mainBall.controlDisabled = false; // 重新启用 BallController
        }
    }

    private System.Collections.IEnumerator RemovePerkAfterDelay(Perk perk, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        // 只从主球移除 perk
        if (mainBall != null && perk.affectsBall)
        {
            perk.RemoveEffect(mainBall.gameObject);
        }
        
        currentActivePerks.Remove(perk);
    }

    // 获取当前激活的 perk 列表（用于分裂小球继承）
    public List<Perk> GetCurrentActivePerks()
    {
        return new List<Perk>(currentActivePerks);
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