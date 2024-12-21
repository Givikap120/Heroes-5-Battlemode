using System;

public class AbilityJousting : Ability, IApplicableBeforeAttack
{
    public AttackParameters Apply(CreatureInstance owner, IAttackable target, AttackParameters parameters)
    {
        if (parameters.MoveBeforeAttack == null)
            return parameters;

        double distance = parameters.MoveBeforeAttack.Value.Before.DistanceTo(parameters.MoveBeforeAttack.Value.After);
        parameters.BaseDamage *= Math.Pow(1.05, (int)distance);

        return parameters;
    }

    public override double OffensePotentialMultiplier => 1.25;
}
