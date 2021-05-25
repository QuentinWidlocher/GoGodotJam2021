using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class IdleSystem : Timer
{
    public static float Currency; // In Mana
    public static float Production; // In Mana/Sec

    /**
     * Big list of all the Generators.
     * This is where the magic happens
     */
    private static readonly List<Generator> Generators = new List<Generator>
    {
        new Generator
        {
            Id = GeneratorId.Generator01,
            Name = "Mana Sap Tap",
            ProductionBase = 0.3f,
            ProductionFormula = g => Mathf.Pow(g.Bought, g.ProductionBase),
            CostBase = 0.5f,
            CostMultiplier = 1.3f,
            CostFormula = g => g.CostBase * Mathf.Pow(g.CostMultiplier, g.Bought),
            UpgradeCostBase = 25f,
            UpgradeCostFormula = g => Mathf.Pow(g.UpgradeCostBase, g.Upgrades),
            Bought = 1,
        }
    };

    public override void _Ready()
    {
        // We update the currency each seconds (by default)
        Connect("timeout", this, nameof(UpdateCurrency));
        Start();

        ComputeProduction();
    }

    public void UpdateCurrency() => Currency += Production;

    public static void BuyGenerator(GeneratorId id)
    {
        var generator = GetGenerator(id);

        if (generator?.Cost <= Currency)
        {
            Currency -= generator.Cost;
            generator.Bought++;
            ComputeProduction();
        }
    }

    public static void BuyUpgrade(GeneratorId id)
    {
        var generator = GetGenerator(id);

        if (generator?.UpgradeCost <= Currency)
        {
            Currency -= generator.UpgradeCost;
            generator.Bought = 0;
            generator.Upgrades++;
            ComputeProduction();
        }
    }

    public static bool CanAffordGenerator(GeneratorId id) => GetGenerator(id)?.Cost <= Currency;
    public static bool CanAffordUpgrade(GeneratorId id) => GetGenerator(id)?.UpgradeCost <= Currency;

    public static Generator? GetGenerator(GeneratorId id) => Generators.Find(g => g.Id == id);

    private static void ComputeProduction()
    {
        // Sum the generators production
        Production = Generators.Select(generator => generator.Production).Sum();
    }
}