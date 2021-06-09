using Godot;

public class PlayerDashingState : MovingState
{
    private const float DashForce = 1500f;
    private const float DashDuration = 0.15f;

    private float timer = DashDuration;

    public override void Enter(IPlayerState from, Player player)
    {
        player.Sprite.Play("dash");
        player.Vulnerable = false;
    }

    public override void Exit(IPlayerState to, Player player)
    {
        player.Vulnerable = true;
    }

    public override void UpdatePlayer(Player player, float delta)
    {
        // We don't call base.UpdatePlayer because we don't want to move while dashing

        player.Velocity = player.FacingDirection * DashForce;
    }

    public override IPlayerState UpdateState(Player player, float delta)
    {
        timer -= delta;

        if (timer <= 0)
            return PlayerState.Falling;

        return this;
    }
}