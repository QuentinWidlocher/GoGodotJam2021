using System.Collections.Generic;
using Godot;

public struct PlayerStat
{
    public float HealthPoints;
    public float DamageDealt;
    public float HealingAmount;
    public int MaxHeals;
    public bool HasDash;
    public bool HasDoubleJump;
    public bool HasWallJump;

    public static PlayerStat Default = new PlayerStat
    {
        HealthPoints = 10,
        DamageDealt = 1,
        HealingAmount = 5,
        MaxHeals = 0,
    };
}

public class StatSystem : Node
{
    [Signal] public delegate void SpiritChange(float newValue);
    [Signal] public delegate void MaxHealthChange(float newValue);
    
    /**
     * Base player stats, should not move and only exist to compute actual stats
     */
    private readonly PlayerStat BasePlayerStat = PlayerStat.Default;

    /**
     * Actual player stats, readonly. Only the modifiers should change
     */
    public PlayerStat PlayerStat => new PlayerStat
    {
        HealthPoints = BasePlayerStat.HealthPoints + GetPlayerUpgrade(PlayerUpgradeId.HpMultiplier).Bonus,
        DamageDealt = BasePlayerStat.DamageDealt + GetPlayerUpgrade(PlayerUpgradeId.AttackMultiplier).Bonus,
        HealingAmount = (BasePlayerStat.HealthPoints + GetPlayerUpgrade(PlayerUpgradeId.HpMultiplier).Bonus) * (0.2f + GetPlayerUpgrade(PlayerUpgradeId.BetterHeal).Bonus),
        MaxHeals = (int) (BasePlayerStat.MaxHeals + GetPlayerUpgrade(PlayerUpgradeId.MoreHeal).Bonus),
        HasDash = GetPlayerUpgrade(PlayerUpgradeId.Dash).Bought > 0,
        HasDoubleJump = GetPlayerUpgrade(PlayerUpgradeId.DoubleJump).Bought > 0,
        HasWallJump = GetPlayerUpgrade(PlayerUpgradeId.WallJump).Bought > 0,
    };

    private float _spiritCount;
    public float SpiritCount
    {
        get => _spiritCount;
        set
        {
            EmitSignal(nameof(SpiritChange), value);
            _spiritCount = value;
        }
    }

    public static readonly List<PlayerUpgrade> PlayerUpgrades = new List<PlayerUpgrade>
    {
        new PlayerUpgrade
        {
            Id = PlayerUpgradeId.HpMultiplier,
            Name = "HP Multiplier",
            CostBase = 100f,
            CostMultiplier = 2f,
            CostFormula = g => g.CostBase * Mathf.Pow(g.CostMultiplier, g.Bought),
            BaseBonus = 5,
        },
        new PlayerUpgrade
        {
            Id = PlayerUpgradeId.AttackMultiplier,
            Name = "Attack Multiplier", 
            CostBase = 100f,
            CostMultiplier = 2f,
            CostFormula = g => g.CostBase * Mathf.Pow(g.CostMultiplier, g.Bought),
            BaseBonus = 1,
        },
        new PlayerUpgrade
        {
            Id = PlayerUpgradeId.BetterHeal,
            Name = "Better Heal",
            CostBase = 450f,
            CostMultiplier = 2f,
            CostFormula = g => g.CostBase * Mathf.Pow(g.CostMultiplier, g.Bought),
            BaseBonus = 0.1f,
            MaxQuantity = 8,
        },
        new PlayerUpgrade
        {
            Id = PlayerUpgradeId.MoreHeal,
            Name = "More Heal",
            CostBase = 500f,
            CostMultiplier = 3f,
            CostFormula = g => g.CostBase * Mathf.Pow(g.CostMultiplier, g.Bought),
            BaseBonus = 1,
            MaxQuantity = 5,
        },
        new PlayerUpgrade
        {
            Id = PlayerUpgradeId.DoubleJump,
            Name = "Double Jump",
            CostBase = 1000f,
            MaxQuantity = 1,
        },
        new PlayerUpgrade
        {
            Id = PlayerUpgradeId.Dash,
            Name = "Dash",
            CostBase = 2000f,
            MaxQuantity = 1,
        },
        new PlayerUpgrade
        {
            Id = PlayerUpgradeId.WallJump,
            Name = "Wall Jump",
            CostBase = 5000f,
            MaxQuantity = 1,
        },
    };
    
    public static bool CanAffordPlayerUpgrade(PlayerUpgradeId id) => CanAffordPlayerUpgrade(GetPlayerUpgrade(id));
    public static bool CanAffordPlayerUpgrade(PlayerUpgrade upgrade) => upgrade.Cost <= IdleSystem.Currency && upgrade.Bought < upgrade.MaxQuantity;

    public static PlayerUpgrade GetPlayerUpgrade(PlayerUpgradeId id) => PlayerUpgrades.Find(g => g.Id == id)!;
    
    public static void BuyPlayerUpgrade(PlayerUpgradeId id)
    {
        var upgrade = GetPlayerUpgrade(id);

        if (upgrade.Cost <= IdleSystem.Currency)
        {
            IdleSystem.Currency -= upgrade.Cost;
            upgrade.Bought++;
            IdleSystem.UpdateCurrency();
        }
    }
}