using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MoveOnDrag : MonoBehaviour
{
    private bool dragging = false;
    private Vector3 offset;
    private Vector2 directionToMouse;
    public float rotationSpeed = 5f; // 旋转缓冲速度
    public float smoothTime = 0.1f; // 平滑时间
    public float mouseThreshold = 0.1f;

    private Rigidbody2D rb2d;
    private float currentAngle; // 当前角度
    private float targetAngle; // 目标角度
    private float angleVelocity; // 用于平滑的角度速度
    private Vector3 lastMousePosition; // 上一帧的鼠标位置

    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        currentAngle = rb2d.rotation;
        lastMousePosition = Input.mousePosition; // 初始化上一帧的鼠标位置
    }

    // Update is called once per frame
    void Update()
    {
        // 获取当前鼠标位置
        Vector3 currentMousePosition = Input.mousePosition;

        // 检查鼠标是否移动
        if (Vector3.Distance(currentMousePosition, lastMousePosition) > mouseThreshold)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;

            // 计算方向
            Vector3 direction = mousePosition - transform.position;

            // 计算目标角度
            targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        }
        
        lastMousePosition = currentMousePosition;
    }
    void  FixedUpdate()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        gameObject.GetComponent<Rigidbody2D>().MovePosition(mousePos);

        currentAngle = Mathf.SmoothDampAngle(currentAngle, targetAngle, ref angleVelocity, smoothTime);

        // 使用 Rigidbody2D.MoveRotation 应用旋转
        rb2d.MoveRotation(currentAngle);
    }
    void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log(other.gameObject.tag);
        if (other.gameObject.tag == "Ball")
        {
            Rigidbody playerRigidbody = other.gameObject.GetComponent<Rigidbody>();
            if (playerRigidbody != null)
            {
                Debug.Log("hit ball");
                // Adjust player velocity to match platform's movement
                playerRigidbody.velocity = Vector3.zero; // Reset any physics-based movement
                playerRigidbody.MovePosition(transform.position); // Move with the platform
            }
        }
    }


    private void  OnMouseDown()
    {
        dragging = true;
    }
    void OnMouseUp()
    {
        dragging = false;
    }
}
