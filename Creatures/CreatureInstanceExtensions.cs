using System.Reflection;

public static class CreatureInstanceExtensions
{
    public static double CalculatePower(this CreatureInstance stack)
    {
        // Stack_Power = base_power * (1+C*Stack_Morale) * (1+C*Stack_Luck) * artifact_mod
        // » C = 0.0173, if Morale,Luck > 0
        // » C = 0.0122, if Morale,Luck < 0
        // artifact_mod = (1 + ΔInitiative/10)

        return stack.Creature.Stats.Power * stack.Amount;
    }

    public static CreatureInstance GetInitialStack(this CreatureInstance stack)
    {
        var player = stack.Player;
        return player.GetInitialStackFor(stack);
    }
}
