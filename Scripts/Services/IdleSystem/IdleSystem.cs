using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using static ServiceLocator;

public class IdleSystem : Timer
{
    public static float Currency; // In Mana
    public static float Production; // In Mana/Sec

    /**
     * Big list of all the Generators.
     * This is where the magic happens
     */
    public static readonly List<Generator> Generators = new List<Generator>
    {
        new Generator
        {
            Id = GeneratorId.Generator01,
            Name = "Sap Tap",
            ProductionBase = 0.4f,
            CostBase = 0.6f,
            CostMultiplier = 1.5f,
            ProductionFormula = g => Mathf.Pow(Mathf.Pow(g.Bought, g.Upgrades + 1), g.ProductionBase)/2,
            CostFormula = g => g.CostBase * Mathf.Pow(g.CostMultiplier, g.Bought),
            Bought = 1,
        },
        new Generator
        {
            Id = GeneratorId.Generator02,
            Name = "Leaf Composter",
            ProductionBase = 1f,
            CostBase = 20f,
            CostMultiplier = 1.6f,
            ProductionFormula = g => Mathf.Pow(Mathf.Pow(g.Bought, g.Upgrades + 1), g.ProductionBase)/1.75f,
            CostFormula = g => g.CostBase * Mathf.Pow(g.CostMultiplier, g.Bought),
        },
        new Generator
        {
            Id = GeneratorId.Generator03,
            Name = "Bark Peeler",
            ProductionBase = 2f,
            CostBase = 100f,
            CostMultiplier = 1.7f,
            ProductionFormula = g => Mathf.Pow(Mathf.Pow(g.Bought, g.Upgrades + 1), g.ProductionBase)/1.5f,
            CostFormula = g => g.CostBase * Mathf.Pow(g.CostMultiplier, g.Bought),
        }
    };

    public override void _Ready()
    {
        // We update the currency each seconds (by default)
        Connect("timeout", this, nameof(UpdateCurrency));
        WaitTime = .5f;
        Start();

        ComputeProduction();
    }

    public static void UpdateCurrency() => Currency += Production;

    public static void BuyGenerator(GeneratorId id)
    {
        var generator = GetGenerator(id);

        if (generator?.Cost <= StatSystemService.SpiritCount)
        {
            StatSystemService.SpiritCount -= generator.Cost;
            generator.Bought++;
            ComputeProduction();
        }
    }

    public static bool CanAffordGenerator(GeneratorId id) => GetGenerator(id)?.Cost <= StatSystemService.SpiritCount;
    public static bool CanAffordGenerator(Generator generator) => generator.Cost <= StatSystemService.SpiritCount;

    public static Generator? GetGenerator(GeneratorId id) => Generators.Find(g => g.Id == id);

    private static void ComputeProduction()
    {
        // Sum the generators production
        Production = Generators.Select(generator => generator.Production).Sum();
    }
}