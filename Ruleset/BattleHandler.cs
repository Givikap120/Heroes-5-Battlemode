using Godot;
using System;
using System.Linq;

public class BattleHandler
{
    private readonly Main parent;
    private Playfield playfield = null!;

    private readonly Bindable<Unit?> currentUnit = new(null);

    public readonly InitiativeHandler InitiativeHandler = new();

    public event Action GameStarted = delegate { };

    public BattleHandler(Main parent)
    {
        this.parent = parent;
        GD.Seed(0);
    }

    public void StartGame()
    {
        playfield = parent.GetNode<Playfield>("Playfield");

        if (playfield == null)
        {
            GD.PrintErr("Playfield node not found!");
            return;
        }

        playfield.OnEmptyCellClicked += emptyCellAction;
        playfield.OnUnitClicked += unitAction;
        playfield.OnUnitWithCellClicked += unitWithCellAction;

        playfield.CurrentUnit = currentUnit;

        test();
    }

    private Player player1 = null!;
    private Player player2 = null!;

    private void test()
    {
        addPlayer(Player.Preset1());
        addPlayer(Player.Preset2());

        GameStarted.Invoke();
        startTurn();
    }

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

        playfield.AddPlayer(player);
        InitiativeHandler.AddPlayer(player);
    }

    private void startTurn()
    {
        if (currentUnit.Value != null) currentUnit.Value.ATB = 0;
        var nextUnit = InitiativeHandler.GetNextUnit();

        // Trigger update anyway
        if (currentUnit.Value == nextUnit) currentUnit.TriggerChange();
        else currentUnit.Value = nextUnit;
    }

    private void emptyCellAction(Vector2I cell)
    {
        if (currentUnit.Value == null) return;

        if (currentUnit.Value is ICanMove movable)
        {
            bool result = movable.MoveTo(cell);
            if (result) endTurn();
        }

        return;
    }

    private void unitAction(Unit unit)
    {
        if (currentUnit.Value == null) return;
    }

    private void unitWithCellAction((Unit unit, Vector2I cell) target)
    {
        if (currentUnit.Value == null) return;

        // If the same team - ignore for now
        if (currentUnit.Value.IsAlly(target.unit))
            return;

        if (currentUnit.Value is ICanAttackMove attacker && target.unit is IAttackable attackable)
        {
            bool result = attacker.AttackWithMove(attackable, target.cell);
            if (result) endTurn();
        }
    }

    private void endTurn()
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
            startTurn();
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
}
