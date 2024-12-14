﻿public class Hero : Unit
{
    public int Level;

    public Hero(Player player) : base(player)
    {
    }

    /// <summary>
    /// Hero always has initiative of 10
    /// </summary>
    public override double Initiative => 10;

    public override int DecideTileChange(int tileType)
    {
        throw new System.NotImplementedException();
    }
}
