public interface IUnit
{

    public string Name { get; }
    public Player Player { get; }

    public double ATB { get; set; }

    public UnitState SaveState();
    public void LoadState(UnitState savedState, bool silent = true);
}
