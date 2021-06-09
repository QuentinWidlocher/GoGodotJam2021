public interface IPlayerState
{
    public void Enter(IPlayerState from, Player player);
    public void Exit(IPlayerState to, Player player);
    public void UpdatePlayer(Player player, float delta);
    public IPlayerState UpdateState(Player player, float delta);
}