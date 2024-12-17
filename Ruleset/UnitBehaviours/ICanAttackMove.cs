using Godot;

public interface ICanMoveAttack : ICanMove, ICanAttack
{
    public bool AttackWithMove(IAttackable attackable, Vector2I movePosition, bool triggetEvents = true)
    {
        bool moveResult = MoveTo(movePosition, triggetEvents);
        if (Coords != movePosition && !moveResult) return false;

        return Attack(attackable, triggetEvents);
    }
}
