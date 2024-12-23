using Godot;
using System;

public class AbilityChampionCharge : Ability, IApplicableBeforeAttack
{
    public AttackParameters Apply(CreatureInstance owner, IAttackable target, AttackParameters parameters)
    {
        if (parameters.MoveBeforeAttack == null)
            return parameters;

        double distance = parameters.MoveBeforeAttack.Value.Before.DistanceTo(parameters.MoveBeforeAttack.Value.After);
        if (distance <= 2)
            return parameters;

        // Calculate attack point
        var targetPoint = target.GetCenter();
        var attackPoint = owner.GetTheClosestPointTo(target.Coords);

        var attackVector = (targetPoint - attackPoint); // Get direction of the attack
        attackVector = attackVector.Clamp(-1, 1); // Clamp it because we can't attack for more than 1 cell
        attackVector = attackVector.Round(); // Round it because we operate with integer coords

        var penetrationCell = attackPoint + new Vector2I((int)attackVector.X * 2, (int)attackVector.Y * 2);
        IAttackable? penetrationTarget = target.Player.GetCreatureAt(penetrationCell);

        if (penetrationTarget == null || penetrationTarget == target)
            return parameters;

        // Calculate attack params
        var penetrationParams = parameters;
        penetrationParams.DamageMultiplier /= 2; // 2 times less damage for secondary target
        penetrationParams.WillCounterAttack = false; // Can't counterattack
        owner.AttackFromParameters(penetrationTarget, penetrationParams);

        return parameters;
    }
}
