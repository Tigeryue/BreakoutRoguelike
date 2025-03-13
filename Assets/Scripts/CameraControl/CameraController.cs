using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera Control Settings")]
    public KeyCode cameraControlKey = KeyCode.Space; // 切换相机控制模式的按键
    public float cameraMovementSpeed = 5f; // 相机移动速度

    [Header("Wall Color Settings")]
    public Color activeWallColor = Color.green; // 激活时的墙壁颜色
    public Color hitColor = Color.white; // 碰撞时的闪光颜色
    public float hitFlashDuration = 0.2f; // 闪光持续时间
    public AnimationCurve hitColorCurve = AnimationCurve.EaseInOut(0, 1, 1, 0); // 闪光颜色变化曲线
    private Color[] originalWallColors; // 存储墙壁原始颜色
    private Dictionary<GameObject, Coroutine> activeFlashCoroutines; // 存储每个墙体当前的闪光协程

    [Header("Ball Centering Settings")]
    public float ballCenteringSpeed = 3f; // 球体居中速度
    public float centeringThreshold = 0.01f; // 居中判定阈值

    [Header("References")]
    public GameObject ball; // 球体引用
    public GameObject upperWall;
    public GameObject bottomWall;
    public GameObject leftWall;
    public GameObject rightWall;

    private bool isCameraControlMode = false; // 相机控制模式状态
    private bool isMoving = false; // 相机是否正在移动
    private bool isCentering = false; // 球体是否正在居中
    private bool needsReactivation = false; // 是否需要重新激活相机控制模式
    private Vector3 targetPosition; // 目标位置
    private Vector3 ballTargetPosition; // 球体目标位置
    private float screenWidthInWorldUnits; // 屏幕宽度对应的世界单位
    private float screenHeightInWorldUnits; // 屏幕高度对应的世界单位
    private Vector3 initialBallScreenPosition; // 开始移动时球在屏幕中的位置
    private int originalBallLayer; // 球体的原始层级

    //cheatcode
    [Header("Cheatcode")]
    public bool isCheatMode = false;
    public KeyCode cheatAddCameraMovementChance = KeyCode.Equals;

    void Start()
    {
        if (ball == null)
        {
            ball = GameObject.FindGameObjectWithTag("Ball");
        }

        // 保存球体的原始层级
        originalBallLayer = ball.layer;

        // 计算屏幕尺寸对应的世界单位
        CalculateScreenDimensions();

        // 初始化闪光协程字典
        activeFlashCoroutines = new Dictionary<GameObject, Coroutine>();

        // 为每个墙体设置触发器脚本
        SetupWallTrigger(upperWall);
        SetupWallTrigger(bottomWall);
        SetupWallTrigger(leftWall);
        SetupWallTrigger(rightWall);

        // 保存墙壁原始颜色
        originalWallColors = new Color[4];
        SaveOriginalWallColor(upperWall, 0);
        SaveOriginalWallColor(bottomWall, 1);
        SaveOriginalWallColor(leftWall, 2);
        SaveOriginalWallColor(rightWall, 3);
    }

    void CalculateScreenDimensions()
    {
        // 获取屏幕边界点在世界空间中的位置
        Vector3 bottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector3 topRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0));
        
        // 计算世界空间中的屏幕尺寸
        screenWidthInWorldUnits = topRight.x - bottomLeft.x;
        screenHeightInWorldUnits = topRight.y - bottomLeft.y;
        Debug.Log($"Screen dimensions in world units - Width: {screenWidthInWorldUnits}, Height: {screenHeightInWorldUnits}");
    }

    void SetupWallTrigger(GameObject wall)
    {
        if (wall == null)
        {
            Debug.LogError("Wall reference is missing!");
            return;
        }

        // 检查是否已经有WallTrigger组件
        WallTrigger trigger = wall.GetComponent<WallTrigger>();
        
        // 如果没有，则添加
        if (trigger == null)
        {
            trigger = wall.AddComponent<WallTrigger>();
        }
        
        // 设置引用
        trigger.cameraController = this;
        
        // 确保有碰撞器
        Collider2D wallCollider = wall.GetComponent<Collider2D>();
        if (wallCollider == null)
        {
            Debug.LogError($"Wall {wall.name} is missing a Collider2D component!");
        }
    }

    void SaveOriginalWallColor(GameObject wall, int index)
    {
        if (wall != null)
        {
            SpriteRenderer renderer = wall.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                originalWallColors[index] = renderer.color;
            }
        }
    }

    void SetWallColor(GameObject wall, Color color)
    {
        if (wall != null)
        {
            SpriteRenderer renderer = wall.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                renderer.color = color;
            }
        }
    }

    void SetAllWallsColor(Color color)
    {
        SetWallColor(upperWall, color);
        SetWallColor(bottomWall, color);
        SetWallColor(leftWall, color);
        SetWallColor(rightWall, color);
    }

    void RestoreWallColors()
    {
        SetWallColor(upperWall, originalWallColors[0]);
        SetWallColor(bottomWall, originalWallColors[1]);
        SetWallColor(leftWall, originalWallColors[2]);
        SetWallColor(rightWall, originalWallColors[3]);
    }

    // 开始墙体闪光效果
    void StartWallFlash(GameObject wall)
    {
        // 如果已经有正在进行的闪光效果，先停止它
        if (activeFlashCoroutines.ContainsKey(wall) && activeFlashCoroutines[wall] != null)
        {
            StopCoroutine(activeFlashCoroutines[wall]);
        }

        // 开始新的闪光效果
        activeFlashCoroutines[wall] = StartCoroutine(WallFlashCoroutine(wall));
    }

    // 墙体闪光效果协程
    IEnumerator WallFlashCoroutine(GameObject wall)
    {
        SpriteRenderer renderer = wall.GetComponent<SpriteRenderer>();
        if (renderer == null) yield break;

        // 获取当前颜色作为原始颜色（这样就能正确处理绿色高亮的情况）
        Color originalColor = renderer.color;
        float elapsedTime = 0f;

        while (elapsedTime < hitFlashDuration)
        {
            float normalizedTime = elapsedTime / hitFlashDuration;
            float curveValue = hitColorCurve.Evaluate(normalizedTime);
            
            // 在当前颜色和闪光颜色之间插值
            renderer.color = Color.Lerp(originalColor, hitColor, curveValue);
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 确保最后恢复到原始颜色
        renderer.color = originalColor;
        activeFlashCoroutines[wall] = null;
    }

    void Update()
    {
        // 切换相机控制模式
        if (Input.GetKeyDown(cameraControlKey))
        {
            if (needsReactivation)
            {
                if (GameManager.Instance.HasCameraMovementChance())
                {
                    needsReactivation = false;
                    isCameraControlMode = true;
                    SetAllWallsColor(activeWallColor);
                    Debug.Log("Camera Control Mode reactivated");
                }
                else
                {
                    Debug.Log("No camera movement chances available!");
                }
            }
            else if (!isMoving && !isCentering)
            {
                if (!isCameraControlMode && !GameManager.Instance.HasCameraMovementChance())
                {
                    Debug.Log("No camera movement chances available!");
                    return;
                }
                isCameraControlMode = !isCameraControlMode;
                if (isCameraControlMode && GameManager.Instance.HasCameraMovementChance())
                {
                    SetAllWallsColor(activeWallColor);
                }
                else
                {
                    RestoreWallColors();
                }
                Debug.Log($"Camera Control Mode: {isCameraControlMode}");
            }
        }

        // 相机移动
        if (isMoving)
        {
            MoveCameraToTarget();
        }

        // 球体居中
        if (isCentering)
        {
            CenterBall();
        }

        if (Input.GetKeyDown(cheatAddCameraMovementChance) && isCheatMode)
        {
            GameManager.Instance.AddCameraMovementChance();
        }
    }

    void DisableBallCollisions()
    {
        // 将球体移动到"IgnoreCollisions"层
        ball.layer = LayerMask.NameToLayer("IgnoreCollisions");
        
        // 停止物理模拟
        Rigidbody2D rb = ball.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.simulated = false;
        }
    }

    void EnableBallCollisions()
    {
        // 恢复球体的原始层级
        ball.layer = originalBallLayer;
        
        // 恢复物理模拟
        Rigidbody2D rb = ball.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.simulated = true;
        }
    }

    public void OnBallHitWall(GameObject wall, Vector2 ballVelocity)
    {
        // 无论是否在相机控制模式，都触发闪光效果
        StartWallFlash(wall);

        if (!isCameraControlMode || isMoving || isCentering) return;
        if (!GameManager.Instance.HasCameraMovementChance()) return;

        Debug.Log($"Processing ball hit on {wall.name} with velocity: {ballVelocity}");
        Vector3 moveDirection = Vector3.zero;
        float moveDistance = 0f;

        // 保存球体在屏幕中的初始位置
        initialBallScreenPosition = Camera.main.WorldToViewportPoint(ball.transform.position);

        // 根据撞击的墙体确定移动方向和距离
        if (wall == upperWall)
        {
            moveDirection = Vector3.up;
            moveDistance = screenHeightInWorldUnits;
        }
        else if (wall == bottomWall)
        {
            moveDirection = Vector3.down;
            moveDistance = screenHeightInWorldUnits;
        }
        else if (wall == leftWall)
        {
            moveDirection = Vector3.left;
            moveDistance = screenWidthInWorldUnits;
        }
        else if (wall == rightWall)
        {
            moveDirection = Vector3.right;
            moveDistance = screenWidthInWorldUnits;
        }

        // 立即恢复所有墙壁的颜色（但不影响闪光效果）
        RestoreWallColors();

        // 设置相机目标位置
        targetPosition = transform.position + moveDirection * moveDistance;
        isMoving = true;
        isCameraControlMode = false;
        needsReactivation = true;
        DisableBallCollisions();
        GameManager.Instance.UseCameraMovementChance(); // 使用一次移动机会
        Debug.Log($"Camera moving to: {targetPosition} with distance: {moveDistance}");
    }

    void MoveCameraToTarget()
    {
        Vector3 previousCameraPosition = transform.position;
        
        // 平滑移动相机
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * cameraMovementSpeed);

        // 计算相机移动的偏移量
        Vector3 cameraDelta = transform.position - previousCameraPosition;
        
        // 保持球体在屏幕中的相对位置
        if (!isCentering)
        {
            // 将球体从当前视口位置转换回世界位置
            Vector3 targetScreenPos = initialBallScreenPosition;
            targetScreenPos.z = Camera.main.WorldToViewportPoint(ball.transform.position).z;
            Vector3 targetWorldPos = Camera.main.ViewportToWorldPoint(targetScreenPos);
            
            // 更新球体位置
            ball.transform.position = targetWorldPos;
        }

        // 检查是否到达目标位置
        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            isMoving = false;
            StartBallCentering(); // 开始将球体移动到屏幕中心
            RestoreWallColors();
        }
    }

    void StartBallCentering()
    {
        // 计算屏幕中心点的世界坐标
        Vector3 screenCenter = new Vector3(0.5f, 0.5f, Camera.main.WorldToViewportPoint(ball.transform.position).z);
        ballTargetPosition = Camera.main.ViewportToWorldPoint(screenCenter);
        
        // 保持z轴位置不变
        ballTargetPosition.z = ball.transform.position.z;
        
        isCentering = true;
    }

    void CenterBall()
    {
        // 平滑移动球体到屏幕中心
        ball.transform.position = Vector3.Lerp(ball.transform.position, ballTargetPosition, Time.deltaTime * ballCenteringSpeed);

        // 检查是否完成居中
        if (Vector3.Distance(ball.transform.position, ballTargetPosition) < centeringThreshold)
        {
            isCentering = false;
            EnableBallCollisions(); // 恢复球体碰撞
        }
    }
}
