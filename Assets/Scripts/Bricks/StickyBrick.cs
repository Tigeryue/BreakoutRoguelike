using UnityEngine;

public class StickyBrick : Brick
{
    public float score;
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            BallController ball = collision.gameObject.GetComponent<BallController>();
            if (ball != null)
            {
                //ball.Stick();
                Debug.Log("黏黏砖块");
                //GameManager.Instance.AddScore(score);
            }
        }
    }
}
