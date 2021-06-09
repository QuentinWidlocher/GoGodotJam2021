using Godot;

public class PlayerDisabledState : IPlayerState
{
    private IPlayerState? OldState;
    private Vector2? OldPosition;

    public void Enter(IPlayerState from, Player player)
    {
        player.FootstepSound.Stop();
        player.HitSound.Stop();
        player.JumpSound.Stop();
        player.LandingSound.Stop();
        player.ShootSound.Stop();

        OldPosition = player.Position;
        player.Position = Vector2.Zero;

        OldState = from;
    }

    public void Exit(IPlayerState to, Player player)
    {
        if (OldPosition.HasValue)
            player.Position = OldPosition.Value;
    }

    public void UpdatePlayer(Player player, float delta)
    {
    }

    public IPlayerState UpdateState(Player player, float delta)
    {
        if (player.Enabled)
        {
            return OldState ?? PlayerState.Falling;
        }

        return this;
    }
}