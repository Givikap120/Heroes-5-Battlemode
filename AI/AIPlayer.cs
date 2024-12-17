using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class AIPlayer : Player
{
    public AIPlayer(BattleHandler battleHandler) : base(battleHandler)
    {
        battleHandler.NewTurnStarted += u =>
        {
            if (u?.Player == this)
                makeMove(u);
        };
    }

    public override bool UIDrawControls => false;

    private void makeMove(Unit unit)
    {
        var actions = getAllPossibleActions(unit);

        foreach (SimulationAction action in actions)
            action.CalculateStateValue();

        var bestAction = actions.MaxBy(a => a.StateValue);

        Task.Run(() =>
        {
            Thread.Sleep(300);
            bestAction?.MakeMove();
        });
    }

    private List<SimulationAction> getAllPossibleActions(Unit currentUnit)
    {
        List<SimulationAction> actions = [];

        actions.Add(new ActionDefend(currentUnit));
        actions.Add(new ActionWait(currentUnit));
        addAttackActions(actions, currentUnit);
        addMoveAndAttackActions(actions, currentUnit);
        addMoveActions(actions, currentUnit);

        return actions;
    }

    private void addMoveActions(List<SimulationAction> actions, Unit currentUnit)
    {
        if (currentUnit is not ICanMove movable)
            return;

        foreach (var cell in movable.GetPossibleMoveOptions(BattleHandler.IsTileOccupied))
        {
            actions.Add(new ActionMove(movable, cell));
        }
    }

    private void addAttackActions(List<SimulationAction> actions, Unit currentUnit)
    {
        if (currentUnit is not ICanAttack attacker)
            return;

        Player enemy = BattleHandler.GetEnemyPlayer(this)!;

        if (!attacker.CanAttackRanged())
            return;

        foreach (var target in enemy.AliveArmy)
        {
            if (attacker.GetAttackType(target).IsRanged())
                actions.Add(new ActionAttack(attacker, target));
        }
    }

    private void addMoveAndAttackActions(List<SimulationAction> actions, Unit currentUnit)
    {
        if (currentUnit is not ICanMoveAttack attacker)
            return;

        Player enemy = BattleHandler.GetEnemyPlayer(this)!;

        foreach (var target in enemy.AliveArmy)
        {
            var tilesAroundEnemy = target.Coords.GetNeighboring();
            foreach (var tile in tilesAroundEnemy)
            {
                if (attacker.CanMoveTo(tile) && !BattleHandler.IsTileOccupied(tile))
                    actions.Add(new ActionMoveAndAttack(attacker, tile, target));
            }
        }
    }
}
