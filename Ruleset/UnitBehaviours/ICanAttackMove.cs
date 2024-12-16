using Godot;

public interface ICanAttackMove : ICanAttack, ICanMove
{
    public bool AttackWithMove(IAttackable attackable, Vector2I movePosition)
    {
        bool moveResult = MoveTo(movePosition);
        if (Coords != movePosition && !moveResult) return false;

        return Attack(attackable);
    }
}
