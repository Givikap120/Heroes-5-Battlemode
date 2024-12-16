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

        initiativeHandler = BattleHandler.Instance.InitiativeHandler;
        BattleHandler.Instance.NewTurnStarted += _ => updateAtbScale();

        updateAtbScale();
    }

    private void updateAtbScale()
    {
        var units = initiativeHandler.Units;

        // Init values for simulation
        double[] atbValues = units.Select(u => u.ATB).ToArray();
        double[] initiatives = units.Select(u => u.Initiative).ToArray();

        double getATB(int i) => atbValues[i];
        void addATB(int i, double value) => atbValues[i] += value;
        double getInitiative(int i) => initiatives[i];

        // First step is finding out the first and the last creature to move
        double[] remainingTurns = InitiativeHandler.GetRemainingTurns(units.Count, getATB, getInitiative);
        int currentToMove = remainingTurns.MinIndex();
        int lastToMove = remainingTurns.MaxIndex();

        // We're skipping first because we're adding it to the big display
        rebuildBigDisplay(units[currentToMove]);

        // Clear all nodes from futureNodes container before adding new
        clearFutureNodes();

        // Add up until the last one
        while (currentToMove != lastToMove)
        {
            atbValues[currentToMove] = 0.0;
            currentToMove = InitiativeHandler.GetNextUnitIndex(atbValues.Length, getATB, addATB, getInitiative);
            addFutureNode(units[currentToMove]);
        }
    }

    private void rebuildBigDisplay(Unit unit)
    {
        // Remove old
        RemoveChild(currentNode);
        currentNode.QueueFree();

        // Add new
        currentNode = unit.CreateDrawableRepresentation();
        currentNode.BackgroundSize = 1.0;
        AddChild(currentNode);
    }

    private void clearFutureNodes()
    {
        foreach (var child in futureNodes.GetChildren())
        {
            futureNodes.RemoveChild(child);
            child.QueueFree();
        }
    }

    private void addFutureNode(Unit unit)
    {
        var drawableUnit = unit.CreateDrawableRepresentation();
        drawableUnit.BackgroundSize = 1.0;
        futureNodes.AddChild(drawableUnit);
    }
}
