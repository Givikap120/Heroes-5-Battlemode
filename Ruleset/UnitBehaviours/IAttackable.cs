using Godot;
using System;

public interface IAttackable : IPlayfieldUnit
{
    public double TotalHP { get; }
    public double Defense { get; }

    /// <summary>
    /// Handle dealing damage to IAttackable.
    /// Don't forget to increment AttackedOnThisTurn.
    /// </summary>
    /// <param name="damage">Amount of damage dealt</param>
    public void TakeDamage(double damage);

    public int AttackedOnThisTurn { get; set; }

    public bool WillCounterattack(ICanAttack attacker)
    {
        // Can't attack - can't counterattack
        if (this is not ICanAttack)
            return false;

        // Only can counterattack to playfield units
        if (attacker is not IPlayfieldUnit playfieldUnit)
            return false;

        bool isNeighbor = playfieldUnit.IsNeighboring(this);

        // Can only counterattack neighbors and only once in the turn
        bool result = isNeighbor && AttackedOnThisTurn == 0;

        // For Abilities OfType<IApplicableToCounterAttack> .Apply(result)
        return result;
    }
}
