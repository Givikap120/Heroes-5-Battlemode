using Godot;

public class PrePlanningHandler : GameHandler
{
    public static PrePlanningHandler Instance { get; private set; } = new PrePlanningHandler();

    public Bindable<Player?> CurrentPlayer = new(null);

    public void StartPrePlanning()
    {
        AddPlayer(PlayerFactory.Preset1());
        AddPlayer(PlayerFactory.Preset2(true));
        NextStep();
    }

    public bool NextStep()
    {
        switch (currentPlayerIndex)
        {
            case 0:
                CurrentPlayer.Value = Player1;
                break;
            case 1:
                BattleHandler.Instance.AddPlayer(Player1!);
                CurrentPlayer.Value = Player2;
                break;
            case 2:
                BattleHandler.Instance.AddPlayer(Player2!);
                break;
        }

        currentPlayerIndex++;
        return currentPlayerIndex >= 3;
    }

    private int currentPlayerIndex = 0;
}