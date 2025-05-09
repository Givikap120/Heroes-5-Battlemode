﻿public class AbilityNoRangePenalty : Ability, IApplicableBeforeAttack
{
    public AttackParameters Apply(CreatureInstance owner, IAttackable target, AttackParameters parameters)
    {
        if (parameters.AttackType == AttackType.RangedWeak)
        {
            parameters.AttackType = AttackType.RangedStrong;
        }
            
        return parameters;
    }

    public override double OffensePotentialMultiplier => 1.5;
}
