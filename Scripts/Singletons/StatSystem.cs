using Godot;

public struct PlayerStat
{
    public float HealthPoints;
    public float DamageDealt;
    public float Defense;
    public float MovingSpeed;
    public float HealingAmount;
    public int MaxHeals;

    public static PlayerStat Default = new PlayerStat
    {
        HealthPoints = 10,
        DamageDealt = 1,
        Defense = 1,
        MovingSpeed = 1,
        HealingAmount = 5,
        MaxHeals = 1,
    };
    
    public static PlayerStat BaseModifiers = new PlayerStat
    {
        HealthPoints = 1,
        DamageDealt = 1,
        Defense = 1,
        MovingSpeed = 1,
        HealingAmount = 1,
        MaxHeals = 1,
    };

    public override string ToString() =>
        $"{nameof(HealthPoints)}: {HealthPoints}, {nameof(DamageDealt)}: {DamageDealt}, {nameof(Defense)}: {Defense}, {nameof(MovingSpeed)}: {MovingSpeed}";
}

public class StatSystem : Node
{
    /**
     * Base player stats, should not move and only exist to compute actual stats
     */
    private static readonly PlayerStat BasePlayerStat = PlayerStat.Default;

    /**
     * Modifiers to apply to the base stats to get the actual stats. Start at 1 so nothing changes
     */
    public static PlayerStat StatModifiers = PlayerStat.BaseModifiers;

    /**
     * Actual player stats, readonly. Only the modifiers should change
     */
    public static PlayerStat PlayerStat => new PlayerStat
    {
        HealthPoints = BasePlayerStat.HealthPoints * StatModifiers.HealthPoints,
        DamageDealt = BasePlayerStat.DamageDealt * StatModifiers.DamageDealt,
        Defense = BasePlayerStat.Defense * StatModifiers.Defense,
        MovingSpeed = BasePlayerStat.MovingSpeed * StatModifiers.MovingSpeed,
        HealingAmount = BasePlayerStat.HealingAmount * StatModifiers.HealingAmount,
        MaxHeals = BasePlayerStat.MaxHeals * StatModifiers.MaxHeals
    };

    public static float ManaCount;
}