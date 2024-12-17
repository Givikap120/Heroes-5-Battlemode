﻿public struct AttackParameters
{
    public int Amount;

    public bool IsCounterAttack;
    public bool IsRanged;
    public bool WillCounterAttack;

    public double BaseDamage;
    public double Attack;
    public double Defense;

    public AttackType AttackType = AttackType.Melee;

    public AttackParameters()
    {
    }
}
