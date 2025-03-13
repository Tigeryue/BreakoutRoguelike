public abstract class Buff
{
    public abstract void Apply(BallController ball);
    public virtual void UpdateBuff(BallController ball) { }
}

public class SpeedBuff : Buff
{
    public override void Apply(BallController ball)
    {
        ball.maxSpeed *= 1.5f;
    }
}

public class SizeBuff : Buff
{
    public override void Apply(BallController ball)
    {
        ball.transform.localScale *= 1.2f;
    }
}

