using System;

public class AbilityEnraged : AbilityApplicableAfterCreatureDeath
{
    public int CurrentAttackBonus = 0;

    public override void Apply(CreatureInstance owner, CreatureInstance died)
    {
        if (!owner.IsAlly(died))
            return;

        double armyPower = owner.Player.CalculateInitialArmyPower();
        double stackPower = died.GetInitialStack().CalculatePower();

        int attackBonus = (int)(died.Creature.Stats.Attack * stackPower / armyPower);
        attackBonus = Math.Max(1, attackBonus);

        owner.CurrentStats.Attack += attackBonus;
        CurrentAttackBonus += attackBonus;
    }

    public override double OffensePotentialMultiplier => 1.1;
}
