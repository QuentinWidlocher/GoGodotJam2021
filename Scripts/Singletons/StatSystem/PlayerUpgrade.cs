using System;

public class PlayerUpgrade
{
    public PlayerUpgradeId Id;
    public string Name = "";
    public float BaseBonus;
    public float CostBase;
    public float CostMultiplier;
    public int Bought;
    public int MaxQuantity = 99;
    
    public Func<PlayerUpgrade, float> CostFormula = g => g.CostBase;

    /**
     * Computed values for the costs and the production
     */
    public float Cost => CostFormula(this);

    public float Bonus => BaseBonus * Bought;
}