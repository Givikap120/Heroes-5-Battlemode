using Godot;
using System;
using System.Linq;

public class BattleHandler
{
    private readonly Main parent;

    public readonly Bindable<Unit?> CurrentUnit = new(null);

    public readonly InitiativeHandler InitiativeHandler;

    public event Action<Player> PlayerAdded = delegate { };
    public event Action GameStarted = delegate { };
    public event Action NewTurnStarted = delegate { };

    public BattleHandler(Main parent)
    {
        InitiativeHandler = new(this);
        this.parent = parent;
        GD.Seed(0);
    }

    public void StartGame()
    {
        addPlayer(Player.Preset1());
        addPlayer(Player.Preset2());

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

        PlayerAdded.Invoke(player);
    }

    #region turn_handling

    private void startNewTurn()
    {
        var nextUnit = InitiativeHandler.GetNextUnit();

        NewTurnStarted.Invoke();

        // Trigger update anyway
        if (CurrentUnit.Value == nextUnit) CurrentUnit.TriggerChange();
        else CurrentUnit.Value = nextUnit;
    }

    private void endTurn(bool isWait = false)
    {
        // Check if any player have won
        int player1ArmyCount = player1.Army.Where(creature => creature != null).Count();
        int player2ArmyCount = player2.Army.Where(creature => creature != null).Count();

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

    public void UnitAction(Unit unit)
    {
        if (CurrentUnit.Value == null) return;
    }

    public void UnitWithCellAction((Unit unit, Vector2I cell) target)
    {
        if (CurrentUnit.Value == null) return;

        // If the same team - ignore for now
        if (CurrentUnit.Value.IsAlly(target.unit))
            return;

        if (CurrentUnit.Value is ICanAttackMove attacker && target.unit is IAttackable attackable)
        {
            bool result = attacker.AttackWithMove(attackable, target.cell);
            if (result) endTurn();
        }
    }

    public void DefendAction()
    {
        endTurn();
    }

    public void WaitAction()
    {
        endTurn(true);
    }

    #endregion
}
