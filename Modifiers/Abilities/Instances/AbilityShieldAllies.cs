using System.Linq;

public class AbilityShieldAllies : AbilityApplicableAfterTurnBegin
{
    public override void Apply(CreatureInstance owner)
    {
        var allCreatures = owner.Player.AliveArmy;

        var withoutEffect = allCreatures.Where(c => !c.HasEffect<EffectShieldAllies>());
        var withEffect = allCreatures.Where(c => c.HasEffect<EffectShieldAllies>());
        var withAbility = allCreatures.Where(c => c.Creature.HasAbility<AbilityShieldAllies>());

        // Remove outdated ShieldAllies effects
        foreach (var creature in withEffect)
        {
            bool hasProtection = false;

            foreach (var protector in withAbility)
            {
                if (creature == protector) continue;

                if (creature.IsNeighboring(protector))
                {
                    hasProtection = true;
                    break;
                }
            }

            if (!hasProtection)
                creature.Effects.RemoveAll(e => e.GetType() == typeof(EffectShieldAllies));
        }

        // Add neighbors to this
        foreach (var creature in withoutEffect)
        {
            if (creature.IsNeighboring(owner))
                creature.Effects.Add(new EffectShieldAllies(creature));
        }
    }

    public override double DefensePotentialMultiplier => 1.1;
}
