using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TerritoryGame")]

namespace Common.Settings
{
    /// <summary>
    /// Static class containing information on the current game state
    /// </summary>
    public static class GameState
    {
        #region Properties

        /// <summary>
        /// The current turn of the game
        /// </summary>
        public static ulong Turn
        {
            get;
            internal set;
        }

        #endregion
    }
}
