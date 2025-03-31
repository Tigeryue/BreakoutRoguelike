using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("GameManager");
                    _instance = go.AddComponent<GameManager>();
                }
            }
            return _instance;
        }
    }

    public Image fadeImage; // 用于淡入淡出的 UI Image
    public float fadeDuration = 1f; // 淡入淡出持续时间
    private float score = 0f;

    [Header("Triple Brick Settings")]
    public int tripleBricksNeeded = 3; // 需要连续击中的TripleBrick数量

    private int currentTripleBrickHits = 0; // 当前连续击中的TripleBrick数量
    private int cameraMovementChances = 0; // 可用的相机移动机会
//------------------------perk--------------------------------
    [Header("Perk Settings")]
    [SerializeField] private PerkManager perkManager;
    [SerializeField] private int perkScoreThreshold = 5; // 分数阈值
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Update()
    {
        
    }

    private void OnEnable()
    {
        // 监听场景加载完成事件
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // 取消监听场景加载完成事件
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 场景加载完成后，重新查找 fadeImage
        if (fadeImage == null)
        {
            GameObject fadeObject = GameObject.Find("FadeImage"); // 假设 fadeImage 的 GameObject 名字是 "FadeImage"
            if (fadeObject != null)
            {
                fadeImage = fadeObject.GetComponent<Image>();
            }
        }
    }

    public void AddScore(float points)
    {
        score += points;
        Debug.Log("当前分数: " + score);
        perkManager.CheckAndShowPerks((int)score, perkScoreThreshold);
        UIManager.Instance.UpdateScoreUI(score); // 更新 UI（如果有）
    }

    public void Death()
    {
        StartCoroutine(FadeAndResetScene());
    }

    private System.Collections.IEnumerator FadeAndResetScene()
    {
        float elapsedTime = 0f;
        Color color = fadeImage.color;

        // 淡出效果
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsedTime / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        // 重置场景
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    public float GetScore()
    {
        return score;
    }

    public void OnTripleBrickHit()
    {
        currentTripleBrickHits++;
        Debug.Log($"Triple Brick hit! Current hits: {currentTripleBrickHits}");

        if (currentTripleBrickHits >= tripleBricksNeeded)
        {
            // 获得一次相机移动机会
            cameraMovementChances++;
            currentTripleBrickHits = 0; // 重置计数
            Debug.Log($"Earned a camera movement chance! Total chances: {cameraMovementChances}");
        }
    }

    public bool HasCameraMovementChance()
    {
        return cameraMovementChances > 0;
    }

    public void UseCameraMovementChance()
    {
        if (cameraMovementChances > 0)
        {
            cameraMovementChances--;
            Debug.Log($"Used a camera movement chance. Remaining chances: {cameraMovementChances}");
        }
    }

    public void AddCameraMovementChance()
    {
        cameraMovementChances++;
        Debug.Log($"Added a camera movement chance. Total chances: {cameraMovementChances}");
    }

    // 当球掉落或开始新关卡时重置连续击中计数
    public void ResetTripleBrickHits()
    {
        currentTripleBrickHits = 0;
        Debug.Log("Triple Brick hits reset");
    }

    // 获取当前连续击中数和需要的总数
    public (int current, int needed) GetTripleBrickProgress()
    {
        return (currentTripleBrickHits, tripleBricksNeeded);
    }

    // 获取当前可用的相机移动机会数量
    public int GetCameraMovementChances()
    {
        return cameraMovementChances;
    }
}
