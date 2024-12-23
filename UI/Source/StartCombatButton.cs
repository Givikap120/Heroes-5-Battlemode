using Godot;

public partial class StartCombatButton : TextureButton
{
    public override void _Pressed()
    {
        base._Pressed();
        bool isFinished = PrePlanningHandler.Instance.NextStep();
        if (isFinished) GetTree().ChangeSceneToFile(SceneFactory.BattleScene);
    }
}
