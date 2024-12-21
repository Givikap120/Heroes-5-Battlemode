public static class AIExtensions
{
    public static void AttackAverage(ICanAttack attacker, IAttackable target)
    {
        var parameters = attacker.CalculateParameters(target, triggerEvents: false);

        if (attacker is IHasRandomDamage randomAttacker)
            parameters.BaseDamage = randomAttacker.AverageDamage;

        attacker.AttackFromParameters(target, parameters);
    }

    public static double CalculateStateValue(IUnit currentUnit, bool useDynamic)
    {
        return useDynamic ? DynamicStateCalculator.CalculateStateValue(currentUnit) : StaticStateCalculator.CalculateStateValue(currentUnit);
    }
}
