using Godot;

public interface ICanMoveAttack : ICanMove, ICanAttack
{
    public bool AttackWithMove(IAttackable attackable, Vector2I movePosition, bool triggetEvents = true)
    {
        var moveResult = MoveTo(movePosition, triggetEvents);
        if (moveResult == null) return false;

        return Attack(attackable, triggetEvents, moveBeforeAttack: moveResult);
    }
}
