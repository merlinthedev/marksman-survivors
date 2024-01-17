namespace _Scripts.Champions.Abilities
{
    public enum AbilityType
    {
        BASIC,
        UNIQUE_PASSIVE,
        OFFENSE,
        DEFENSE,
        MOBILITY,
        ULTIMATE
    }
    

    /// <summary>
    /// May need this in the future who knows
    /// </summary>
    public enum AbilityUseType
    {
        PASSIVE, // Passive abilities are always on
        ACTIVE, // Utility abilities are used to provide utility, like buffs or debuffs
    }
}