using Godot;
using Helpers;
using static Inputs;
using static ServiceLocator;

public class PlayerJumpingState : MovingState
{
    private const float JumpTime = 0.15f;
    private const float JumpVelocity = 500f;
    private float JumpTimer;
    private bool HasJumpedFromWall;

    public override void Enter(IPlayerState from, Player player)
    {
        base.Enter(from, player);

        player.JumpSound.Play();

        HasJumpedFromWall = from is PlayerWallGlidingState;
    }

    public override void Exit(IPlayerState to, Player player)
    {
        base.Exit(to, player);

        player.Jumps++;
    }

    public override void UpdatePlayer(Player player, float delta)
    {
        base.UpdatePlayer(player, delta);

        // When the jump button is held, _jumpTimer counts up to only let the player gain y velocity until JumpTime is reached
        if (Input.IsActionPressed(Jump.String()))
        {
            JumpTimer += delta;
            if (JumpTimer < JumpTime && (player.Jumps < player.MaxJumps || HasJumpedFromWall))
            {
                player.Velocity.y = -JumpVelocity;
                player.Sprite.Play("jump");
            }
        }

        if (player.IsOnCeiling())
            JumpTimer = JumpTime;
    }

    public override IPlayerState UpdateState(Player player, float delta)
    {
        base.UpdateState(player, delta);

        if (Input.IsActionJustReleased(Jump.String()) || JumpTimer >= JumpTime)
            return PlayerState.Falling;

        if (DashStateCondition(player))
            return PlayerState.Dashing;

        return this;
    }
}