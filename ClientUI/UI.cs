using Common.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ClientUI
{
    /// <summary>
    /// The User Interface base class
    /// It is used to show the game for a human player
    /// </summary>
    public static class UI
    {
        #region Properties

        /// <summary>
        /// The ID of the player for whom the game is being shown
        /// </summary>
        public static int PlayerID
        {
            get;
            set;
        }

        /// <summary>
        /// The GraphicsDevice used for drawing the game
        /// </summary>
        public static GraphicsDevice GraphicsDevice
        {
            get;
            set;
        }

        #endregion

        #region Graphics

        /// <summary>
        /// Draws the game in its current state
        /// </summary>
        /// <param name="gameTime">the current game time</param>
        /// <param name="board">the board to draw</param>
        public static void Draw(GameTime gameTime, Board board)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
        }

        #endregion

        #region Audio

        #endregion
    }
}
