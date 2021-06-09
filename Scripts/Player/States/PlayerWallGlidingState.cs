using Godot;
using Helpers;
using static Inputs;

public class PlayerWallGlidingState : MovingState
{
    public const float WallJumpKnockback = 1000;
    private const float Friction = 300f;

    public override void Enter(IPlayerState from, Player player)
    {
        base.Enter(from, player);

        player.Sprite.Play("wall_slide");

        player.Jumps = 0;
    }

    public override void Exit(IPlayerState to, Player player)
    {
        base.Exit(to, player);

        if (to is PlayerJumpingState)
            player.Velocity.x += -Mathf.Sign(player.Velocity.x) * WallJumpKnockback;
    }

    public override void UpdatePlayer(Player player, float delta)
    {
        base.UpdatePlayer(player, delta);

        player.Velocity.y = Friction;
    }

    public override IPlayerState UpdateState(Player player, float delta)
    {
        base.UpdateState(player, delta);

        if (Input.IsActionJustPressed(Jump.String()))
            return PlayerState.Jumping;

        if (!player.IsOnWall())
            return PlayerState.Falling;

        return this;
    }
}