using System;

public class Generator
{
    // Used to find the generator in the list
    public GeneratorId Id;
    // Used to display the generator's name
    public string Name = "";
    // How much Mana the generator produces by default
    public float ProductionBase;
    // How much Mana the generator costs by default
    public float CostBase;
    // How much the cost of the generator goes up when bought
    public float CostMultiplier;
    // How many upgrades the generator got
    public int Upgrades = 1;
    // How much Mana the upgrade code by default
    public float UpgradeCostBase = 1;
    // How many generators like this are bought
    public int Bought;
    
    // Compute the production by using the parameters 
    public Func<Generator, float> ProductionFormula = g => g.ProductionBase;
    // Compute the cost by using the parameters
    public Func<Generator, float> CostFormula = g => g.CostBase;
    // Compute the upgrade cost by using the parameters
    public Func<Generator, float> UpgradeCostFormula = g => g.UpgradeCostBase;

    /**
     * Computed values for the costs and the production
     */
    public float Cost => CostFormula(this);
    public float Production => ProductionFormula(this);
    public float UpgradeCost => UpgradeCostFormula(this);

    public override string ToString() => $"{nameof(Generator)} | {nameof(Id)}: {Id}, {nameof(Name)}: {Name}, {nameof(ProductionBase)}: {ProductionBase}, {nameof(CostBase)}: {CostBase}, {nameof(CostMultiplier)}: {CostMultiplier}, {nameof(Upgrades)}: {Upgrades}, {nameof(UpgradeCostBase)}: {UpgradeCostBase}, {nameof(Bought)}: {Bought}";
}