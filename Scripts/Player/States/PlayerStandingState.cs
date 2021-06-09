using Godot;
using Helpers;
using static Inputs;

public class PlayerStandingState : MovingState
{
    public override void Enter(IPlayerState from, Player player)
    {
        base.Enter(from, player);

        player.Sprite.Play("idle");
        player.Jumps = 0;

        if (from is PlayerFallingState)
            player.LandingSound.Play();
    }

    public override void UpdatePlayer(Player player, float delta)
    {
        base.UpdatePlayer(player, delta);

        if (Input.IsActionPressed(Left.String()) || Input.IsActionPressed(Right.String()))
        {
            player.Sprite.Play("run");
            if (!player.FootstepSound.Playing)
                player.FootstepSound.Play();
        }
        else
        {
            player.Sprite.Play("idle");
            player.FootstepSound.Stop();
        }
    }

    public override IPlayerState UpdateState(Player player, float delta)
    {
        base.UpdateState(player, delta);

        if (!player.OnGround)
            return PlayerState.Falling;

        if (DashStateCondition(player))
            return PlayerState.Dashing;

        if (Input.IsActionJustPressed(Jump.String()))
            return PlayerState.Jumping;

        return this;
    }
}