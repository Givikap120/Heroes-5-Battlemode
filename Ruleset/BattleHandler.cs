using Godot;
using System;
using System.Collections;
using System.Linq;

public class BattleHandler
{
    public static BattleHandler Instance { get; private set; } = null!;

    private readonly Main parent;

    public readonly Bindable<Unit?> CurrentUnit = new(null);

    public readonly InitiativeHandler InitiativeHandler;

    public event Action<Player> PlayerAdded = delegate { };

    public event Action GameStarted = delegate { };
    public event Action GameEnded;

    public event Action<Unit?> NewTurnStarted = delegate { };

    public event Action<CreatureInstance> CreatureDied = delegate { };

    public BattleHandler(Main parent)
    {
        this.parent = parent;

        InitiativeHandler = new(this);
        GameEnded = delegate 
        {
            CurrentUnit.Value = null;
            NewTurnStarted.Invoke(null); }
        ;

        GD.Seed(0);

        Instance = this;
    }

    public void StartGame()
    {
        addPlayer(PlayerFactory.Preset1(this));
        addPlayer(PlayerFactory.Preset2(this, true));

        GameStarted.Invoke();
        startNewTurn();
    }

    private Player player1 = null!;
    private Player player2 = null!;

    private void addPlayer(Player player)
    {
        if (player1 == null)
        {
            player.Id = 1;
            player.Color = Colors.Red;
            player1 = player;
        }
        else if (player2 == null)
        {
            player.Id = 2;
            player.Color = Colors.Blue;
            player2 = player;
        }
        else return;

        player.CreatureDied += CreatureDied.Invoke;
        PlayerAdded.Invoke(player);
    }

    public bool IsTileOccupied(Vector2I tile) => player1.IsTileOccupiedByPlayer(tile) || player2.IsTileOccupiedByPlayer(tile);

    public Player? GetEnemyPlayer(Player player)
    {
        if (player == player1)
            return player2;
        else if (player == player2)
            return player1;
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
        int player1ArmyCount = player1.Army.Where(creature => creature != null && creature.Amount > 0).Count();
        int player2ArmyCount = player2.Army.Where(creature => creature != null && creature.Amount > 0).Count();

        // Check win condition
        if (player1ArmyCount == 0 && player2ArmyCount == 0) // Draw
        {
            callGameOverWindow("Draw");
        }
        else if (player2ArmyCount == 0) // Player 1 won
        {
            callGameOverWindow("Player 1 Won");
        }
        else if (player1ArmyCount == 0) // Player 2 won
        {
            callGameOverWindow("Player 2 Won");
        }
        else
        {
            InitiativeHandler.EndTurn(CurrentUnit.Value!, isWait);
            startNewTurn();
        }
    }

    private void callGameOverWindow(string text)
    {
        GD.Print($"Game Over. {text}");
        GameEnded.Invoke();

        var popup = parent.GetNode<PopupPanel>("GameOverPopup");

        var label = popup.GetNode<Label>("GameOverPopupText");
        label.Text = text;

        popup.Show();
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
        CurrentUnit.Value!.Defend();
        endTurn();
    }

    public void WaitAction()
    {
        endTurn(true);
    }

    #endregion
}
