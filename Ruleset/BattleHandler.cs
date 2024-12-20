using Godot;
using System;
using System.Collections;
using System.Linq;

public class BattleHandler
{
    public static BattleHandler Instance { get; private set; } = new BattleHandler();

    public readonly Bindable<Unit?> CurrentUnit = new(null);

    public readonly InitiativeHandler InitiativeHandler;

    public event Action<Player> PlayerAdded = delegate { };

    public event Action GameStarted = delegate { };
    public event Action<string> GameEnded;

    public event Action<Unit?> NewTurnStarted = delegate { };

    public event Action<CreatureInstance> CreatureDied = delegate { };

    private BattleHandler()
    {
        InitiativeHandler = new(this);
        GameEnded = delegate 
        {
            CurrentUnit.Value = null;
            NewTurnStarted.Invoke(null); }
        ;

        GD.Seed(0);
    }

    public void StartGame()
    {
        if (Player1 == null || Player2 == null)
        {
            GD.PrintErr("Can't start game. Not enough player");
            return;
        }

        GameStarted.Invoke();
        startNewTurn();
    }

    public Player? Player1 { get; private set; } = null;
    public Player? Player2 { get; private set; } = null;

    public void AddPlayer(Player player)
    {
        if (Player1 == null)
        {
            player.Id = 1;
            player.Color = Colors.Red;
            Player1 = player;
        }
        else if (Player2 == null)
        {
            player.Id = 2;
            player.Color = Colors.Blue;
            Player2 = player;
        }
        else return;

        player.CreatureDied += CreatureDied.Invoke;
        PlayerAdded.Invoke(player);
    }

    public bool IsTileOccupied(Vector2I tile) => Player1!.IsTileOccupiedByPlayer(tile) || Player2!.IsTileOccupiedByPlayer(tile);

    public Player? GetEnemyPlayer(Player player)
    {
        if (player == Player1)
            return Player2;
        else if (player == Player2)
            return Player1;
        return null;
    }

    #region turn_handling

    private void startNewTurn()
    {
        CurrentUnit.Value = InitiativeHandler.GetNextUnit();
        NewTurnStarted.Invoke(CurrentUnit.Value);
    }

    private void endTurn(bool isWait = false)
    {
        // Check if any player have won
        int player1ArmyCount = Player1!.Army.Where(creature => creature != null && creature.Amount > 0).Count();
        int player2ArmyCount = Player2!.Army.Where(creature => creature != null && creature.Amount > 0).Count();

        // Check win condition
        if (player1ArmyCount == 0 && player2ArmyCount == 0) // Draw
        {
            GameEnded.Invoke("Draw");
        }
        else if (player2ArmyCount == 0) // Player 1 won
        {
            GameEnded.Invoke("Player 1 Won");
        }
        else if (player1ArmyCount == 0) // Player 2 won
        {
            GameEnded.Invoke("Player 2 Won");
        }
        else
        {
            InitiativeHandler.EndTurn(CurrentUnit.Value!, isWait);
            startNewTurn();
        }
    }

    #endregion

    #region actions
    public void EmptyCellAction(Vector2I cell)
    {
        if (CurrentUnit.Value == null) return;

        if (CurrentUnit.Value is ICanMove movable)
        {
            bool result = movable.MoveTo(cell);
            if (result) endTurn();
        }

        return;
    }

    public void UnitAction(IPlayfieldUnit unit)
    {
        if (CurrentUnit.Value == null) return;

        // If the same team - ignore for now
        if (CurrentUnit.Value.IsAlly(unit))
            return;

        if (CurrentUnit.Value is ICanAttack attacker && unit is IAttackable attackable)
        {
            bool result = attacker.Attack(attackable, triggerEvents: true);
            if (result) endTurn();
        }
    }


    public void UnitWithCellAction(IPlayfieldUnit unit, Vector2I cell)
    {
        if (CurrentUnit.Value == null) return;

        // If the same team - ignore for now
        if (CurrentUnit.Value.IsAlly(unit))
            return;

        if (CurrentUnit.Value is ICanMoveAttack attacker && unit is IAttackable attackable)
        {
            bool result = attacker.AttackWithMove(attackable, cell);
            if (result) endTurn();
        }
    }

    public void DefendAction()
    {
        CurrentUnit.Value?.Defend();
        endTurn();
    }

    public void WaitAction()
    {
        endTurn(true);
    }

    #endregion
}
