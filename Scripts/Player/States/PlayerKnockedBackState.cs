using Godot;
using static Helpers.TaskHelpers;

public class PlayerKnockedBackState : IPlayerState
{
    private const float KnockBackForce = 500;
    private const float KnockBackFriction = 0.7f;
    private const int InvincibilityCoolDown = 500;

    private Vector2 knockingBack;

    public void Enter(IPlayerState from, Player player)
    {
        player.Animations.Play("hurt");
        player.HitSound.Play();

        // knockingBack is a force that's applied after the movement and gradually decreases over time
        knockingBack = new Vector2(-player.FacingDirection.x, -0.5f) * KnockBackForce;

        player.Vulnerable = false;
        RunAfterDelay(() => player.Vulnerable = true, InvincibilityCoolDown);
    }

    public void Exit(IPlayerState to, Player player)
    {
    }

    public void UpdatePlayer(Player player, float delta)
    {
        player.Velocity += knockingBack;
        knockingBack = knockingBack.LinearInterpolate(Vector2.Zero, KnockBackFriction);
    }

    public IPlayerState UpdateState(Player player, float delta)
    {
        if (knockingBack.Length() < 1)
            return PlayerState.Falling;

        return this;
    }
}