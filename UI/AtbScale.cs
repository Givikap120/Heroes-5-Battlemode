using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class AtbScale : Control
{
    private DrawableCreatureInstance currentNode= null!;
    private HBoxContainer futureNodes = null!;

    private InitiativeHandler initiativeHandler = null!;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        Main parent = GetNode<Main>("/root/Main");

        currentNode = GetNode<DrawableCreatureInstance>("CurrentUnitNode");
        futureNodes = GetNode<HBoxContainer>("ScrollContainer/FutureUnitsNode");

        initiativeHandler = parent.BattleHandler.InitiativeHandler;
        parent.BattleHandler.NewTurnStarted += updateAtbScale;

        updateAtbScale();
    }

    private void updateAtbScale()
    {
        var units = initiativeHandler.Units;

        // Init values for simulation
        double[] atbValues = units.Select(u => u.ATB).ToArray();
        double[] initiatives = units.Select(u => u.Initiative).ToArray();

        Func<int, double> getATB = i => atbValues[i];
        Action<int, double> addATB = (i, value) => atbValues[i] += value;
        Func<int, double> getInitiative = i => initiatives[i];

        // First step is finding out the first and the last creature to move
        double[] remainingTurns = InitiativeHandler.GetRemainingTurns(units.Count, getATB, getInitiative);
        int currentToMove = remainingTurns.MinIndex();
        int lastToMove = remainingTurns.MaxIndex();

        // We're skipping first because we're adding it to the big display
        currentNode.Parent = (CreatureInstance)units[currentToMove];

        // Clear all nodes from futureNodes container before adding new
        foreach (var child in futureNodes.GetChildren())
        {
            futureNodes.RemoveChild(child);
            child.QueueFree();
        }

        // Add up until the last one
        while (currentToMove != lastToMove)
        {
            atbValues[currentToMove] = 0.0;
            currentToMove = InitiativeHandler.GetNextUnitIndex(atbValues.Length, getATB, addATB, getInitiative);

            var drawableUnit = units[currentToMove].CreateDrawableRepresentation();
            drawableUnit.BackgroundSize = 1.0;
            futureNodes.AddChild(drawableUnit);
        }
    }
}
