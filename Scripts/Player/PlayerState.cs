public static class PlayerState
{
    public static PlayerFallingState Falling => new PlayerFallingState();
    public static PlayerStandingState Standing => new PlayerStandingState();
    public static PlayerJumpingState Jumping => new PlayerJumpingState();
    public static PlayerDisabledState Disabled => new PlayerDisabledState();
    public static PlayerDashingState Dashing => new PlayerDashingState();
    public static PlayerWallGlidingState WallGliding => new PlayerWallGlidingState();
    public static PlayerKnockedBackState KnockedBack => new PlayerKnockedBackState();
}