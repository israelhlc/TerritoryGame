using ClientUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TerritoryGame.Control;

namespace TerritoryGame.UserInterface
{
    /// <summary>
    /// The user interface manager.
    /// It calls the drawing method in the UI client
    /// </summary>
    public static class UIManager
    {
        #region Initialization

        /// <summary>
        /// Initializes the UI Manager
        /// </summary>
        /// <param name="graphicsDevice">The graphics device used for drawing the game</param>
        /// <param name="playerID">The ID of the player which board is going to be shown</param>
        internal static void Initialize(GraphicsDevice graphicsDevice, int playerID)
        {
            UI.PlayerID = playerID;
            UI.GraphicsDevice = graphicsDevice;
        }

        #endregion

        #region Graphics

        /// <summary>
        /// Draws the game according to the player ID set up in the UI class
        /// </summary>
        /// <param name="gameTime"></param>
        internal static void Draw(GameTime gameTime)
        {
            UI.Draw(gameTime, BoardManager.GetPlayerBoard(UI.PlayerID));
        }

        #endregion
    }
}
