using System;

public class AbilityBattleFrenzy : Ability, IApplicableBeforeAttack
{
    public AttackParameters Apply(CreatureInstance owner, IAttackable target, AttackParameters parameters)
    {
        if (!parameters.IsCounterAttack || owner.AttackedOnThisTurn == 0)
            return parameters;

        parameters.DamageMultiplier *= Math.Pow(1.5, owner.AttackedOnThisTurn - 1);

        return parameters;
    }

    public override double DefensePotentialMultiplier => 1.2;
}