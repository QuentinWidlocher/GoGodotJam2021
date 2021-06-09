using System;
using Godot;
using Helpers;
using static Inputs;
using static ServiceLocator;

public abstract class MovingState : IPlayerState
{
    private const float Acceleration = 150f;
    private const float MaxVelocity = 500f;
    private const float DashCoolDown = 0.5f;
    private const float Friction = 0.8f;

    protected bool CanDash(Player player) => player.DashCoolDownTimer <= 0;

    protected bool DashStateCondition(Player player) =>
        CanDash(player) && Input.IsActionJustPressed(Dash.String()) && StatSystemService.PlayerStat.HasDash;

    public virtual void Enter(IPlayerState from, Player player)
    {
        if (from is PlayerDashingState)
            player.DashCoolDownTimer = DashCoolDown;
    }

    public virtual void Exit(IPlayerState to, Player player)
    {
    }

    public virtual void UpdatePlayer(Player player, float delta)
    {
        if (Input.IsActionPressed("left"))
        {
            // Applying acceleration
            player.Velocity.x -= Input.GetActionStrength("left") * Acceleration;
            // Clamping _vel.x only inside the input event because that way only limit the max velocity caused by input
            // This means that we can go past the maximum velocity if we apply acceleration elsewhere (e.g. dashes, cannons etc.)
            Math.Clamp(player.Velocity.x, -MaxVelocity, MaxVelocity);

            player.IsFacingRight = false;
        }

        if (Input.IsActionPressed("right"))
        {
            // See above for explanations
            player.Velocity.x += Input.GetActionStrength("right") * Acceleration;
            Math.Clamp(player.Velocity.x, -MaxVelocity, MaxVelocity);

            player.IsFacingRight = true;
        }

        // Simulating air resistance and friction (WARNING: Extremely difficult partial differential equation being calculated, proceed with caution)
        player.Velocity.x *= Friction;

        player.Sprite.FlipH = !player.IsFacingRight;
    }

    public virtual IPlayerState UpdateState(Player player, float delta)
    {
        if (player.DashCoolDownTimer >= 0)
            player.DashCoolDownTimer -= delta;

        return this;
    }
}