using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BallController : MonoBehaviour
{
    public float maxSpeed = 5f; // Max speed limit for the ball
    protected Rigidbody2D rb;
    public float maxDragDistance = 0.1f;    // 最大拖拽距离（世界单位）
    public float speedFactor = 10f;       // 拖拽距离转换为速度的比例系数
    public float timeSlowFactor = 0.3f;        //拖拽时减慢时间系数
    private Vector3 dragStartPos;         // 拖拽起点（世界坐标）
    private bool isDragging; 
    public bool controlDisabled = false;
    private bool isStuck = false; // 新增：标记小球是否处于黏住状态
    public LineRenderer dragLine;

    [HideInInspector]
    public float ballRadius;
    private Vector2 hitLocation;
    private bool shouldStick;

    //小球发射方向轨迹相关：
    public LineRenderer ballTracerLine;
    public int numPoints = 50;
    public float timeBetweenPoints = 0.1f;
    public LayerMask collidableLayers;

    private bool justLaunched = false; // 新增：标记小球是否刚刚发射
    private float launchProtectionTime = 0.1f; // 新增：发射后的保护时间
    private float launchProtectionTimer = 0f; // 新增：保护时间计时器

    protected List<Perk> activePerks = new List<Perk>();
    public GameObject childBallPrefab;
    protected virtual void Start()
    {
        // Get the Rigidbody2D component attached to the ball
        rb = GetComponent<Rigidbody2D>();
        Physics2D.queriesStartInColliders = false;

        ballRadius = GetComponent<CircleCollider2D>().radius;
    }
    void Update()
    {
        if(controlDisabled == false)
        {
            // 更新发射保护计时器
            if (justLaunched)
            {
                launchProtectionTimer -= Time.deltaTime;
                if (launchProtectionTimer <= 0)
                {
                    justLaunched = false;
                }
            }

            // 鼠标左键按下时，开始记录拖拽起点
            if (Input.GetMouseButtonDown(0))
            {
                if (isStuck)
                {
                    // 如果小球处于黏住状态，重新启用物理模拟
                    rb.simulated = true;
                    isStuck = false;
                }
                StartDrag();
            }

            // 鼠标左键按住时，更新拖拽逻辑
            if (Input.GetMouseButton(0))
            {
                DuringDrag();
            }

            // 鼠标左键松开时，施加速度
            if (Input.GetMouseButtonUp(0))
            {
                EndDrag();
            }
        }
        
    }

    void FixedUpdate()
    {
        // Get the current velocity of the ball
        Vector2 currentVelocity = rb.velocity;

        // Check if the ball's velocity exceeds the max speed
        if (currentVelocity.magnitude > maxSpeed)
        {
            // If it exceeds, clamp the velocity to the max speed
            rb.velocity = currentVelocity.normalized * maxSpeed;
        }

        if (shouldStick)
        {
            // 将小球移动到正确的位置
            transform.position = hitLocation;

            // 重置标志位
            shouldStick = false;
            
            // 重新启用物理模拟
            rb.simulated = true;
            rb.velocity = Vector2.zero; // 确保重新启用时速度为零
        }
    }

    // 开始拖拽
    protected virtual void StartDrag()
    {
        Time.timeScale = timeSlowFactor;
        Time.fixedDeltaTime = 0.02f * timeSlowFactor;

        isDragging = true;
        dragStartPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        dragStartPos.z = 0; // 确保 z 坐标为 0
        if (dragLine != null) dragLine.enabled = true;

        //小球轨迹
        ballTracerLine.positionCount = numPoints;
    }

    // 拖拽过程中
    protected virtual void DuringDrag()
    {
        
        // 将鼠标位置转换为世界坐标
        Vector3 currentMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        currentMousePos.z = 0;

        // 计算拖拽方向（从起点到当前鼠标位置）
        Vector3 dragDirection = currentMousePos - dragStartPos;

        // 限制最大拖拽距离
        float dragDistance = Mathf.Clamp(dragDirection.magnitude, 0, maxDragDistance);
        dragDirection = dragDirection.normalized * dragDistance;

        // 可视化拖拽方向（例如 Debug 绘制线段）
        Debug.DrawLine(dragStartPos, dragStartPos + dragDirection, Color.gray);
        if (dragLine != null)
        {  
            dragLine.SetPosition(0, dragStartPos);
            dragLine.SetPosition(1, dragStartPos + dragDirection);
        }

        //可视化小球轨迹
        DrawProjection(rb.position, dragDirection);
    }

    // 结束拖拽，施加速度
    protected virtual void EndDrag()
    {
        Time.timeScale = 1.0f;
        Time.fixedDeltaTime = 0.02f;
        if (!isDragging) return;

        // 计算最终拖拽方向和距离
        Vector3 dragEndPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        dragEndPos.z = 0;
        Vector3 dragDirection = dragEndPos - dragStartPos;

        // 计算速度（方向相反，大小与拖拽距离成正比）
        Vector2 velocity = -dragDirection.normalized * (dragDirection.magnitude * speedFactor);

        // 确保重置所有黏住相关的状态
        shouldStick = false;
        isStuck = false;
        rb.simulated = true;

        // 设置发射保护
        justLaunched = true;
        launchProtectionTimer = launchProtectionTime;

        // 施加速度
        rb.velocity = velocity;

        // 重置拖拽状态
        isDragging = false;
        if (dragLine != null) dragLine.enabled = false;
        ballTracerLine.positionCount = 0;
    }

    protected virtual void DrawProjection(Vector2 start, Vector2 velocity)
    {
        Vector2 position = start;
        Vector2 direction = -velocity.normalized;
        float speed = velocity.magnitude;

        for (int i = 0; i < numPoints; i++)
        {
            //Debug.Log("line num:" + i + "line position" + position);
            ballTracerLine.SetPosition(i, position);

            RaycastHit2D hit = Physics2D.Raycast(position, direction, speed * timeBetweenPoints, collidableLayers);
            //RaycastHit2D hit = Physics2D.CircleCast(position, ballRadius, direction, speed * timeBetweenPoints, collidableLayers);
            if (hit.collider != null && hit.collider.tag != "Ball")
            {
                // 计算反射方向
                direction = Vector2.Reflect(direction, hit.normal);
                position = hit.point + direction * 0.01f; // 稍微偏移一点避免重复碰撞
            }
            else
            {
                position += direction * speed * timeBetweenPoints;
            }
        }
    }

//-----------------------------------------各种碰撞-----------------------------------------

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        // 如果小球刚发射，不处理黏住效果
        if (justLaunched) return;

        Brick brick = collision.gameObject.GetComponent<Brick>();
        if (brick != null && brick.CompareTag("StickyBrick"))
        {
            // 获取碰撞点
            ContactPoint2D contact = collision.contacts[0];

            // 计算小球应该停住的位置
            hitLocation = contact.point + contact.normal * ballRadius;

            // 立即停止小球并设置位置
            rb.velocity = Vector2.zero;
            rb.simulated = false; // 暂时禁用物理模拟
            transform.position = hitLocation;

            // 设置标志位
            shouldStick = true;
            isStuck = true;

            Physics2D.SyncTransforms();
        }
    }

//------------------------------------------死亡逻辑-------------------------------
    public void DisableControl()
    {
        controlDisabled = true;
    }
/*
    //黏黏砖块相关
    public void Stick()
    {
        rb.velocity = Vector2.zero;
        rb.position = hitLocation;
    }
    public void SetHitLocation(Vector2 location)
    {
        hitLocation = location;
    }
*/
//------------perk相关
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

    public void Split(float splitAngle, int splitCount = 2)
    {
        // 获取当前速度
        Vector2 currentVelocity = rb.velocity;
        float currentSpeed = currentVelocity.magnitude;
        
        // 计算分裂角度间隔
        float angleStep = splitAngle * 2 / splitCount;
        
        // 创建分裂小球
        for (int i = 0; i <= splitCount; i++)
        {
            // 计算分裂后的速度方向
            float angle = -splitAngle + angleStep * i;
            if(angle == 0) continue;

            Vector2 splitDirection = Quaternion.Euler(0, 0, angle) * currentVelocity.normalized;
            Vector2 splitVelocity = splitDirection * currentSpeed;
            
            // 创建分裂小球
            GameObject childBallObj = Instantiate(childBallPrefab, transform.position, Quaternion.identity);
            ChildBall childBall = childBallObj.GetComponent<ChildBall>();
            
            if (childBall != null)
            {
                // 初始化分裂小球
                childBall.Initialize(splitVelocity);
            }
        }
    }
    public void SetSpeedMultiplier(float multiplier)
    {
        maxSpeed *= multiplier;
    }
}
