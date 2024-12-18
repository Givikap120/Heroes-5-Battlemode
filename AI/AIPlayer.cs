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

    private static void makeMove(Unit unit)
    {
        var actions = GetAllPossibleActions(unit);

        foreach (SimulationAction action in actions)
            action.CalculateStateValue();

        var bestAction = actions.MaxBy(a => a.StateValue);

        Task.Run(() =>
        {
            Thread.Sleep(300);
            bestAction?.MakeMove();
        });
    }

    public static List<SimulationAction> GetAllPossibleActions(Unit currentUnit)
    {
        List<SimulationAction> actions = [];

        actions.Add(new ActionDefend(currentUnit));
        actions.Add(new ActionWait(currentUnit));
        AddAttackActions(actions, currentUnit);
        AddMoveAndAttackActions(actions, currentUnit);
        AddMoveActions(actions, currentUnit);

        return actions;
    }

    public static void AddMoveActions(List<SimulationAction> actions, Unit currentUnit)
    {
        if (currentUnit is not ICanMove movable)
            return;

        foreach (var cell in movable.GetPossibleMoveOptions())
        {
            actions.Add(new ActionMove(movable, cell));
        }
    }

    public static void AddAttackActions(List<SimulationAction> actions, Unit currentUnit)
    {
        if (currentUnit is not ICanAttack attacker)
            return;

        Player enemy = BattleHandler.Instance.GetEnemyPlayer(currentUnit.Player)!;

        if (!attacker.CanAttackRanged())
            return;

        foreach (var target in enemy.AliveArmy)
        {
            if (attacker.GetAttackType(target).IsRanged())
                actions.Add(new ActionAttack(attacker, target));
        }
    }

    public static void AddMoveAndAttackActions(List<SimulationAction> actions, Unit currentUnit)
    {
        if (currentUnit is not ICanMoveAttack attacker)
            return;

        Player enemy = BattleHandler.Instance.GetEnemyPlayer(currentUnit.Player)!;

        foreach (var target in enemy.AliveArmy)
        {
            var tilesAroundEnemy = target.Coords.GetNeighboring();
            foreach (var tile in tilesAroundEnemy)
            {
                if (attacker.CanMoveTo(tile) && !BattleHandler.Instance.IsTileOccupied(tile))
                    actions.Add(new ActionMoveAndAttack(attacker, tile, target));
            }
        }
    }
}
