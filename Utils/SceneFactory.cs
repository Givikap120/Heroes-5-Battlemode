using Godot;
using System;
using System.Collections.Generic;
public static class SceneFactory
{
    private static PackedScene drawableCreatureScene = GD.Load<PackedScene>("res://UI/DrawableCreatureInstance.tscn");
    public static DrawableCreatureInstance CreateDrawableCreatureInstance(CreatureInstance creatureInstance)
    {
        var instance = (DrawableCreatureInstance)drawableCreatureScene.Instantiate();
        instance.Parent = creatureInstance;
        return instance;
    }
}

