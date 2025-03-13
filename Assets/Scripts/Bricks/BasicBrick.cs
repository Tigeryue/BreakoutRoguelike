using UnityEngine;

public class BasicBrick : Brick
{
    public float score;
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            base.OnCollisionEnter2D(collision);
            Debug.Log("普通砖块被摧毁，增加分数！");
            GameManager.Instance.AddScore(score);
        }
    }
}
