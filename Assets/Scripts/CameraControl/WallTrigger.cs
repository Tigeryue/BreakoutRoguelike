using UnityEngine;

public class WallTrigger : MonoBehaviour
{
    public CameraController cameraController;

    private void Start()
    {
        if (cameraController == null)
        {
            Debug.LogError($"WallTrigger on {gameObject.name} is missing CameraController reference!");
        }
        
        Collider2D collider = GetComponent<Collider2D>();
        if (collider == null)
        {
            Debug.LogError($"Wall {gameObject.name} is missing a Collider2D component!");
        }
        else
        {
            Debug.Log($"WallTrigger initialized on {gameObject.name}");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"Collision detected on {gameObject.name} with {collision.gameObject.name}");
        
        if (collision.gameObject.CompareTag("Ball"))
        {
            Debug.Log($"Ball hit wall: {gameObject.name}");
            Rigidbody2D ballRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (ballRb != null)
            {
                if (cameraController != null)
                {
                    cameraController.OnBallHitWall(gameObject, ballRb.velocity);
                }
                else
                {
                    Debug.LogError($"WallTrigger on {gameObject.name} has lost its CameraController reference!");
                }
            }
            else
            {
                Debug.LogError($"Ball is missing Rigidbody2D component!");
            }
        }
    }
} 