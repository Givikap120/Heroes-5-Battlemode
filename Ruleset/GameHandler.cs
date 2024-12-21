using Godot;

public abstract class GameHandler
{
    public static GameHandler AnyHandler => BattleHandler.Instance.IsBattleStarted ? BattleHandler.Instance : PrePlanningHandler.Instance;

    public bool IsBattleStarted = false;

    public bool IsTileOccupied(Vector2I tile) => Player1!.IsTileOccupiedByPlayer(tile) || Player2!.IsTileOccupiedByPlayer(tile);

    public bool IsTileOccupied(Vector2I tile, IPlayfieldUnit except) => Player1!.IsTileOccupiedByPlayer(tile, except) || Player2!.IsTileOccupiedByPlayer(tile, except);

    public Player? Player1;
    public Player? Player2;

    public virtual Player? AddPlayer(Player player)
    {
        if (Player1 == null)
        {
            player.Id = 1;
            player.Color = Colors.Red;
            Player1 = player;
        }
        else if (Player2 == null)
        {
            player.Id = 2;
            player.Color = Colors.Blue;
            Player2 = player;
        }
        else return null;

        return player;
    }
}
