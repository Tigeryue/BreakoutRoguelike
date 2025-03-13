using UnityEngine;
public class DeadBrick : Brick
{
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            BallController ball = collision.gameObject.GetComponent<BallController>();
            if (ball != null)
            {
                ball.DisableControl();
                Debug.Log("死亡砖块");
                GameManager.Instance.Death();
            }
        }
    }
}
