namespace ThreeCardPoker
{
    using System;

    /// <summary>
    /// Represents a list of game actions available to a player.
    /// </summary>
    [Flags]
    public enum GameAction : byte
    {
        // TODO: Add support of other actions
        None  = 1,
        Deal  = 2,
        Stand = 4,
        Hit   = 8
    }
}
