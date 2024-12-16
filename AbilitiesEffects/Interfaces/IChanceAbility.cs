using Godot;
using System;

public interface IChanceAbility : IAbility
{
    public static bool TryTriggerProc(double userHP, double targetHP, double chanceFactor) => TryTriggerProc(GetChance(userHP, targetHP, chanceFactor));
    public static bool TryTriggerProc(double chance) => GD.Randf() <= chance;

    /// <summary>
    /// Calculates the probability of ability proc.
    /// </summary>
    /// <param name="userHP">Total HP of ability owner</param>
    /// <param name="targetHP">Total HP of ability target</param>
    /// <param name="chanceFactor">How many tries you have for ability to proc</param>
    /// <returns></returns>
    public static double GetChance(double userHP, double targetHP, double chanceFactor)
    {
        double baseChance = 0.25 + 0.03 * (userHP >= targetHP ? userHP / targetHP : -targetHP / userHP);
        baseChance = Math.Clamp(baseChance, 0.05, 0.75);
        double totalChance = 1 - Math.Pow(1 - baseChance, chanceFactor);
        return totalChance;
    }
}
