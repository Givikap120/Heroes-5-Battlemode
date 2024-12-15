using Godot;

public interface ICanAttackMove : ICanAttack, ICanMove
{
    public bool AttackWithMove(IAttackable attackable, Vector2I movePosition)
    {
        //if (CanAttackRanged) return Attack(attackable);

        bool moveResult = MoveTo(movePosition);
        if (!moveResult) return false;

        return AttackInternal(attackable);
    }
}
