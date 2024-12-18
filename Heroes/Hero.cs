public partial class Hero : Unit
{
    public int Level;

    public Hero(Player player) : base(player)
    {
    }
    public override DrawableCreatureInstance CreateDrawableRepresentation()
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Hero always has initiative of 10
    /// </summary>
    public override double Initiative => 10;

    public override int DecideTileChange(int tileType)
    {
        throw new System.NotImplementedException();
    }

    public override UnitState SaveState()
    {
        throw new System.NotImplementedException();
    }

    public override void LoadState(UnitState savedState, bool silent = true)
    {
        throw new System.NotImplementedException();
    }
}
