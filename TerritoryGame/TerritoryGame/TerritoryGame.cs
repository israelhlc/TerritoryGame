using Common.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using TerritoryGame.Control;
using TerritoryGame.Control.Commands;
using TerritoryGame.Control.GamePlayers;
using TerritoryGame.Initialization;
using TerritoryGame.Log;
using TerritoryGame.UserInterface;

namespace TerritoryGame
{
    /// <summary>
    /// This is the main commandType for your game
    /// </summary>
    internal class TerritoryGame : Game
    {
        #region Members

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates the TerritoryGame, initializing graphics and any other resource needed at constructor time
        /// There is also an "Initialize" method for proper initialization
        /// </summary>
        public TerritoryGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            //initialize the game logger
            GameLogger.Initialize();

            //initialize the game settings
            GameInitializer.InitializeGameSettings();

            //initialize the game resources which are needed in the entire game
            GameInitializer.InitializeGameResources();

            //initialize the game players
            GameInitializer.InitializePlayers(CommandProcessor.ExecuteCommand);

            //initialize the currentBoard
            BoardManager.Initialize();

            //initialize the game logic controls
            GameInitializer.InitializeGameControls();

            //initialize the game UI with the first player being drawn - it can be changed if needed
            UIManager.Initialize(GraphicsDevice, Players.CurrentPlayer.ID);

            //starts the game with a new thread for the command processor
            CommandProcessor.StartGame();

            //calls base.Initialize
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        #endregion

        #region Logic Update

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Escape) || GameManager.GameOver)
            {
                //exits the game
                this.Exit();
            }
            else
            {
                //calls the base update method
                base.Update(gameTime);
            }
        }

        #endregion

        #region Drawing

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //draws the game through the UIManager
            UIManager.Draw(gameTime);

            //calls the base Draw method
            base.Draw(gameTime);
        }

        #endregion

        #region Ending

        /// <summary>
        /// Disposes the game resources when exiting
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            //aborts the CommandProcessor to make sure no thread is left running
            CommandProcessor.Abort();

            //closes the game logger
            GameLogger.Close();

            //calls base dispose to properly dispose
            base.Dispose(disposing);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {

        }

        #endregion
    }
}
