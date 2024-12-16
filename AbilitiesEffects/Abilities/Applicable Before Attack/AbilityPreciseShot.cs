﻿public class AbilityPreciseShot : IAbility, IApplicableBeforeAttack
{
    public AttackParameters Apply(CreatureInstance owner, IAttackable target, AttackParameters parameters)
    {
        if (parameters.AttackType == AttackType.RangedStrong)
        {
            if (owner.DistanceTo(target) < 3)
                parameters.Defense = 0;
        }
            
        return parameters;
    }
}
