using Godot;
public static class SceneFactory
{
    private static readonly PackedScene drawableUnitScene = GD.Load<PackedScene>("res://UI/Scenes/DrawableUnit.tscn");
    private static readonly PackedScene drawableCreatureScene = GD.Load<PackedScene>("res://UI/Scenes/DrawableCreatureInstance.tscn");
    public static DrawableUnit CreateDrawableUnit(Unit unit)
    {
        var instance = (DrawableUnit)drawableUnitScene.Instantiate();
        instance.Parent = unit;
        return instance;
    }

    public static DrawableCreatureInstance CreateDrawableCreatureInstance(CreatureInstance creatureInstance)
    {
        var instance = (DrawableCreatureInstance)drawableCreatureScene.Instantiate();
        instance.Parent = creatureInstance;
        return instance;
    }

    public const string PrePlanningScene = "res://UI/Scenes/PrePlanning.tscn";
    public const string BattleScene = "res://UI/Scenes/Battle.tscn";
}

