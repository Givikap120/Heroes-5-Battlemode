using Godot;

public class PrePlanningHandler
{
    public static PrePlanningHandler Instance { get; private set; } = new PrePlanningHandler();

    public Bindable<Player?> CurrentPlayer = new(null);

    public void StartPrePlanning()
    {
        addPlayer(PlayerFactory.Preset1());
        addPlayer(PlayerFactory.Preset2(true));
        NextStep();
    }

    public bool NextStep()
    {
        switch (currentPlayerIndex)
        {
            case 0:
                CurrentPlayer.Value = player1;
                break;
            case 1:
                BattleHandler.Instance.AddPlayer(player1);
                CurrentPlayer.Value = player2;
                break;
            case 2:
                BattleHandler.Instance.AddPlayer(player2);
                break;
        }

        currentPlayerIndex++;
        return currentPlayerIndex >= 3;
    }

    private int currentPlayerIndex = 0;
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
    }
}