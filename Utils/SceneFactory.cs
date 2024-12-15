using Godot;
using System;
using System.Collections.Generic;
public static class SceneFactory
{
    public static DrawableCreatureInstance CreateDrawableCreatureInstance(CreatureInstance creatureInstance)
    {
        var scene = GD.Load<PackedScene>("res://UI/DrawableCreatureInstance.tscn");
        var instance = (DrawableCreatureInstance)scene.Instantiate();
        instance.Parent = creatureInstance;
        return instance;
    }
}

