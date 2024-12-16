﻿using Godot;

public interface ICanAttackMove : ICanAttack, ICanMove
{
    public bool AttackWithMove(IAttackable attackable, Vector2I movePosition)
    {
        bool moveResult = MoveTo(movePosition);
        if (!moveResult) return false;

        return Attack(attackable);
    }
}
