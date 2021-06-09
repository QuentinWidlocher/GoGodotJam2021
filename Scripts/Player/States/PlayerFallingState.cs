using Godot;
using Helpers;
using static Inputs;

public class PlayerFallingState : MovingState
{
    private const float Gravity = 40f;

    public override void UpdatePlayer(Player player, float delta)
    {
        base.UpdatePlayer(player, delta);

        player.Velocity.y += Gravity;

        if (player.Velocity.y > 0)
            player.Sprite.Play("fall");

        if (player.Velocity.y > 400)
            player.Sprite.Play("fall_fast");
    }

    public override IPlayerState UpdateState(Player player, float delta)
    {
        base.UpdateState(player, delta);

        if (player.OnGround)
            return PlayerState.Standing;

        if (Input.IsActionJustPressed(Jump.String()) && player.HaveJumpsLeft)
            return PlayerState.Jumping;

        if (DashStateCondition(player))
            return PlayerState.Dashing;

        if (player.IsOnWall())
            return PlayerState.WallGliding;

        return this;
    }
}