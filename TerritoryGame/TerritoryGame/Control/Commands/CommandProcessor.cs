using System;
using System.Collections.Generic;
using System.Threading;
using System.Timers;
using Common.Commands;
using Common.Resources;
using Common.Resources.Buildings;
using Common.Resources.Terrains;
using Common.Resources.Units;
using Common.Settings;
using Common.Util;
using TerritoryGame.Control.Commands.Exceptions;
using TerritoryGame.Control.Commands.UnitActions;
using TerritoryGame.Control.GamePlayers;
using TerritoryGame.Control.GamePlayers.Exceptions;
using TerritoryGame.Log;
using TerritoryGame.Settings;

namespace TerritoryGame.Control.Commands
{
    /// <summary>
    /// The static class which coordenates the game's turns and player control
    /// </summary>
    internal static class CommandProcessor
    {
        #region Constants

        /// <summary>
        /// Interval (in miliseconds) before calling the next player's turn
        /// </summary>
        private const int INTERVAL_BETWEEN_PLAYERS = 1000;

        /// <summary>
        /// Interval (in miliseconds) to wait before running the old threads cleaner process
        /// </summary>
        private const int THREADS_CLEANER_INTERVAL = 5000;

        /// <summary>
        /// Maximum time (in miliseconds) to wait the player's turn before considering timeout and giving the turn to the next player
        /// </summary>
        private const int PLAYER_TURN_TIMEOUT = 30000;

        #endregion

        #region Properties

        /// <summary>
        /// The dictionary for the commands of the game
        /// The key is the command commandType, and the value is the command itself
        /// </summary>
        private static Dictionary<CommandType, CommandExecuter> Commands
        {
            get;
            set;
        }

        /// <summary>
        /// The thread of the current player turn
        /// </summary>
        private static Thread TurnThread
        {
            get;
            set;
        }

        /// <summary>
        /// The timer which manages the current turn timeout
        /// </summary>
        private static System.Timers.Timer CurrentTurnTimeoutTimer
        {
            get;
            set;
        }

        /// <summary>
        /// List of threads, kept for finalizing purposes
        /// </summary>
        private static List<Thread> Threads
        {
            get;
            set;
        }

        /// <summary>
        /// The validation code for the turn.
        /// It is used to assure no player can cheat playing in other player's turn
        /// </summary>
        private static long CurrentTurnValidationCode
        {
            get;
            set;
        }

        #endregion

        #region CommandType Delegates

        /// <summary>
        /// Delegate methods for the different commands processors of the game
        /// </summary>
        /// <param name="command">The command given by the player</param>
        /// <returns>The result of the command processing - OK iif the command was succesfully executed</returns>
        private delegate void CommandExecuter(PlayerCommand command);

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes the CommandProcessor and all the threads needed for controlling the command processor
        /// </summary>
        internal static void Initialize()
        {
            //initializes the commands dictionary
            Commands = new Dictionary<CommandType, CommandExecuter>();
            Commands.Add(CommandType.EndOfTurn, ExecuteEndOfTurnCommand);
            Commands.Add(CommandType.MoveTo, ExecuteMoveToCommand);
            Commands.Add(CommandType.Attack, ExecuteAttackCommand);
            Commands.Add(CommandType.InfluenceAt, ExecuteInfluenceAt);
            Commands.Add(CommandType.EquipUnit, ExecuteEquipUnit);
            Commands.Add(CommandType.SetProduction, ExecuteSetProduction);
            Commands.Add(CommandType.CreateBuilding, ExecuteCreateBuilding);

            //initializes the threads list
            Threads = new List<Thread>();

            //initialize thread to clear the threads from the list periodically
            Thread threadsCleaner = new Thread(new ThreadStart(RemoveOldThreads));
            threadsCleaner.IsBackground = true;
            Threads.Add(threadsCleaner);
            threadsCleaner.Start();

            //initializes the turns timeout timer
            CurrentTurnTimeoutTimer = new System.Timers.Timer(PLAYER_TURN_TIMEOUT);
            CurrentTurnTimeoutTimer.Elapsed += new ElapsedEventHandler(CallTurnTimeout);
            CurrentTurnTimeoutTimer.Enabled = true;
            CurrentTurnTimeoutTimer.Start();
        }

        /// <summary>
        /// Starts the game by calling the first player's turn
        /// </summary>
        internal static void StartGame()
        {
            //sets the first turn
            GameState.Turn = Rules.Default.FirstTurn;

            //calls the first player's turn to start the game
            CallPlayerTurnThread(Players.CurrentPlayer);
        }

        #endregion

        #region Threads Management

        /// <summary>
        /// Forces all the processing to be finalized.
        /// All the remaining threads receive an abort command
        /// </summary>
        internal static void Abort()
        {
            //locks the TurnThread object to abort it
            lock (TurnThread)
            {
                TurnThread.Abort(new CommandProcessorAbortedException());
            }

            //locks the Threads list
            lock (Threads)
            {
                //clear the threads which are not alive anymore
                Threads.RemoveAll(thread => thread == null || !thread.IsAlive);

                //iterates the old threads to call abort on them
                foreach (Thread thread in Threads)
                {
                    //if the thread ended since the RemoveAll above, ignore it
                    if (thread.IsAlive)
                    {
                        //forces the thread to run in backgroud and tries to abort it
                        thread.IsBackground = true;
                        thread.Abort(new CommandProcessorAbortedException());
                    }
                }
            }
        }

        /// <summary>
        /// Background thread to remove all the threads which are not alive anymore
        /// </summary>
        private static void RemoveOldThreads()
        {
            try
            {
                //repeats until it is called to be aborted with a ThreadAbortException
                while (true)
                {
                    //sleeps the thread for the parameterized interval
                    Thread.Sleep(THREADS_CLEANER_INTERVAL);

                    //removes all the old thread which are not alive
                    lock (Threads)
                    {
                        Threads.RemoveAll(thread => thread == null || !thread.IsAlive);
                    }
                }
            }
            //safely allows the thread to be aborted
            catch (ThreadAbortException)
            {
                Thread.ResetAbort();
            }
        }

        #endregion

        #region Main Control

        /// <summary>
        /// The method called by the players to execute the commands
        /// </summary>
        /// <param name="command">The command to be executed</param>
        /// <param name="turnValidationCode">The validation code sent to the player in the beginning of the turn</param>
        /// <returns>The result of the command execution - true iif the command was correct AND was processed by the game</returns>
        public static PlayerCommandResponse ExecuteCommand(PlayerCommand command, long turnValidationCode)
        {
            //validate the player ID to see if it is the correct player
            if (Players.CurrentPlayer.ID != command.PlayerID)
                return new PlayerCommandResponse(null, new IncorrectPlayerIDException());

            //validate the turn validation code to see if it is the correct player sending the command
            if (CurrentTurnValidationCode != turnValidationCode)
                return new PlayerCommandResponse(null, new IncorrectTurnValidationCodeException());

            //gets the respective command executer
            CommandExecuter commandExecuter;
            if (!Commands.TryGetValue(command.Command, out commandExecuter))
                return new PlayerCommandResponse(BoardManager.GetPlayerBoard(Players.CurrentPlayer.ID), new CommandExecuterNotDefinedException());

            //before calling the command executer, resets the thread timeout
            ResetTurnTimeout();

            try
            {
                //calls the specific command validation/executor
                commandExecuter(command);

                //if no exception was thrown, everything was OK
                return new PlayerCommandResponse(BoardManager.GetPlayerBoard(Players.CurrentPlayer.ID));
            }
            catch (Exception ex)
            {
                //Sends the response to the player with the exception
                //This way it is possible to identify the reason for the NOK result
                return new PlayerCommandResponse(BoardManager.GetPlayerBoard(Players.CurrentPlayer.ID), ex);
            }
        }

        /// <summary>
        /// Resets the timeout for the player's turn. It must be called after any valid command received
        /// </summary>
        static void ResetTurnTimeout()
        {
            lock (CurrentTurnTimeoutTimer)
            {
                //restarts the timer
                CurrentTurnTimeoutTimer.Stop();
                CurrentTurnTimeoutTimer.Start();
            }
        }

        #region Turn Callers

        /// <summary>
        /// Calls the next player to play its turn.
        /// It will call the next player and call a new turn for him
        /// </summary>
        private static void CallNextPlayerTurn()
        {
            //gets the next player
            Player nextPlayer;
            try
            {
                nextPlayer = Players.NextPlayer;
            }
            //if there is no next player available (all the others are dead), ends the game
            catch (NoNextPlayerAvailableException ex)
            {
                GameManager.EndGame(ex);
                return;
            }

            //calls the next player's turn
            CallPlayerTurnThread(nextPlayer);
        }

        /// <summary>
        /// Calls the player to play its turn in a new thread
        /// </summary>
        /// <param name="player">The player to execute the turn</param>
        private static void CallPlayerTurnThread(Player player)
        {
            //if there is no turn thread yet, instantiates it
            if (TurnThread == null)
            {
                TurnThread = new Thread(new ParameterizedThreadStart(CallPlayerTurn));
            }
            else
            {
                lock (TurnThread)
                {
                    //locks the Threads list to add the old TurnThread to it
                    lock (Threads)
                    {
                        Threads.Add(TurnThread);
                    }

                    //creates a new background thread calling the Player turn
                    TurnThread = new Thread(new ParameterizedThreadStart(CallPlayerTurn));
                }
            }

            //sets the new thread for background and starts it
            lock (TurnThread)
            {
                TurnThread.IsBackground = true;
                TurnThread.Start(player);
            }
        }

        /// <summary>
        /// Calls the player to play its turn
        /// </summary>
        /// <param name="playerAsObject">The player to execute the turn. It must be a Player</param>
        /// <exception cref="InvalidPlayerException">Thrown if the argument is not of the Player type</exception>
        private static void CallPlayerTurn(object playerAsObject)
        {
            try
            {
                //tries to cast the player
                Player player = playerAsObject as Player;

                //if it is not a player, throws the InvalidPlayerException
                if (player == null)
                    throw new InvalidPlayerException(playerAsObject);

                //sleeps before calling the player turn - it allows the older threads to finalize properly
                Thread.Sleep(INTERVAL_BETWEEN_PLAYERS);

                //generates a new turn code and calls the player instance to play the turn
                CurrentTurnValidationCode = RandomUtil.NextLong;
                player.Instance.PlayTurn(BoardManager.GetPlayerBoard(player.ID), CurrentTurnValidationCode);
            }
            catch (ThreadAbortException ex)
            {
                //If needed, the object sent with the Abort can be recovered as follows
                Exception stateInfoException = ex.ExceptionState as Exception;

                //understands that the game does not want the player to play anymore, so ends the thread safely
                //cancels the abort so no exception is thrown anymore - the thread will end after this
                Thread.ResetAbort();
            }
        }

        /// <summary>
        /// Calls the turn timeout - gives the turn to the next player
        /// </summary>
        /// <param name="sender">The event sender - not used</param>
        /// <param name="e">The event arguments - not used</param>
        private static void CallTurnTimeout(object sender, ElapsedEventArgs e)
        {
            //log the message on the game
            GameLogger.Log(LogMessages.PlayerTurnTimeout, new List<string>() { Players.CurrentPlayer.ID.ToString() });

            //aborts the current turn thread
            TurnThread.Abort(new TurnTimeoutException());

            //resets the timer
            ResetTurnTimeout();

            //calls next player turn
            CallNextPlayerTurn();
        }

        #endregion

        #endregion

        #region Common Validation

        /// <summary>
        /// Validates if the given command has the given commandType
        /// </summary>
        /// <param name="command">The player command</param>
        /// <param name="commandType">The desired command commandType</param>
        /// <exception cref="InvalidCommandExecuterException">Thrown when the given command was different than the expected</exception>
        private static void ValidateCommandType(PlayerCommand command, CommandType commandType)
        {
            //validates if the command commandType of the player command is the same of the desired command commandType
            if (command.Command != commandType)
                throw new InvalidCommandExecuterException(commandType, command);
        }

        /// <summary>
        /// Validates if the tile has only units owned by the given player
        /// </summary>
        /// <param name="position">The position of the tile</param>
        /// <param name="playerID">The player ID</param>
        /// <param name="allowEmpty">Indicates if it must allow the tile to be empty</param>
        /// <exception cref="InvalidCommandDestinationException">Thrown when the given position is invalid in the game currentBoard</exception>
        /// <exception cref="TileNotOwnedByException">Thrown when the destination tile has units not belonging to the given player</exception>
        private static void ValidateTileUnitsOwnedOnlyBy(Position position, int playerID, bool allowEmpty)
        {
            //validates the position
            ValidatePosition(position);

            //gets the tile
            Tile tile = BoardManager.Board.GetTile(position);

            //validates if the tile is empty and it is not allowed
            if ((!allowEmpty) && tile.Units.Count == 0)
                throw new TileNotOwnedByException();

            //validates if the tile has units belonging to another player
            if (tile.Units.Exists(unit => unit.Owner != playerID))
                throw new TileNotOwnedByException();
        }

        /// <summary>
        /// Validates if the tile has units not owned by the given player
        /// </summary>
        /// <param name="position">The position of the tile</param>
        /// <param name="playerID">The player ID</param>
        /// <param name="allowEmpty">Indicates if it must allow the tile to be empty</param>
        /// <exception cref="InvalidCommandDestinationException">Thrown when the given position is invalid in the game currentBoard</exception>
        /// <exception cref="TileNotOwnedByException">Thrown when the destination tile has units belonging to the given player</exception>
        private static void ValidateTileUnitsNotOwnedBy(Position position, int playerID, bool allowEmpty)
        {
            //validates the position
            ValidatePosition(position);

            //gets the tile
            Tile tile = BoardManager.Board.GetTile(position);

            //validates if the tile is empty and it is not allowed
            if ((!allowEmpty) && tile.Units.Count == 0)
                throw new TileOwnedByException();

            //validates if the tile has units belonging to the player
            if (tile.Units.Exists(unit => unit.Owner == playerID))
                throw new TileOwnedByException();
        }

        /// <summary>
        /// Validates if the tile at the given position has majority domain by the given player
        /// The majority of domination is considered when greater than 0,5
        /// </summary>
        /// <param name="position">The position of the tile</param>
        /// <param name="playerID">The ID of the player to check domination</param>
        private static void ValidateTileMajorityDomain(Position position, int playerID)
        {
            //gets the tile at the given position
            Tile tile = BoardManager.Board.GetTile(position);

            //normalizes the tile's domain, in case it is not normalized
            tile.NormalizeDomains();

            //gets the player's current domain
            double currentDomain;
            tile.Domain.TryGetValue(playerID, out currentDomain);

            //checks if the player has more than half of the domain
            if (currentDomain > (Tile.NORMALIZED_DOMAIN_SUM / 2))
                throw new NoMajorityDominationException(position, currentDomain);
        }

        /// <summary>
        /// Validates if the given buildingType is valid
        /// </summary>
        /// <param name="buildingType">The BuildingType given for creation</param>
        /// <exception cref="InvalidBuildingTypeException">Thrown when the building type is invalid</exception>
        private static void ValidateBuildingType(BuildingType? buildingType)
        {
            //checks if the building type is valid
            if (buildingType == null)
                throw new InvalidBuildingTypeException(buildingType);
        }

        /// <summary>
        /// Validates the position, checking if it is valid inside the game
        /// </summary>
        /// <param name="position">The given position</param>
        /// <exception cref="InvalidCommandDestinationException">Thrown when the given position is invalid in the game currentBoard</exception>
        private static void ValidatePosition(Position position)
        {
            //validates if the position is null
            if (position == null)
                throw new InvalidCommandDestinationException();

            //validates if there is a tile at the given position
            if (BoardManager.Board.GetTile(position) == null)
                throw new InvalidCommandDestinationException();
        }

        /// <summary>
        /// Validates if there is no building in the given position
        /// </summary>
        /// <param name="position">The position where must not have a building</param>
        /// <exception cref="BuildingAlreadyInTileException">Thrown when there is already a building in the given position</exception>
        private static void ValidateNoBuildingInTile(Position position)
        {
            //checks if the tile at the given position has a building
            if (BoardManager.Board.GetTile(position).Building != null)
                throw new BuildingAlreadyInTileException(position);
        }

        /// <summary>
        /// Validates if the tile can receive a building
        /// </summary>
        /// <param name="position">The position of the tile</param>
        private static void ValidateTileCanReceiveBuilding(Position position)
        {
            //validates if the terrain type allows building creation
            if (!Terrains.Get(BoardManager.Board.GetTile(position).Terrain).CanReceiveBuilding)
                throw new TileCannotReceiveBuildingException(position);
        }

        #endregion

        #region Commands

        #region EndOfTurn

        /// <summary>
        /// Executes the end of turn, giving the turn to the next alive player
        /// </summary>
        /// <param name="command">The player command</param>
        private static void ExecuteEndOfTurnCommand(PlayerCommand command)
        {
            //validates the end of turn command
            ValidateEndOfTurnCommand(command);

            //logs the end of turn
            GameLogger.Log(LogMessages.PlayerEndedTurn, new List<String>() { command.PlayerID.ToString() });

            //calls the next players' turn
            CallNextPlayerTurn();
        }

        /// <summary>
        /// Validates the End of Turn Command
        /// </summary>
        /// <param name="command">The player command</param>
        /// <exception cref="InvalidCommandExecuterException">Thrown when the command executer was not found</exception>
        private static void ValidateEndOfTurnCommand(PlayerCommand command)
        {
            //validates if the command commandType is valid
            ValidateCommandType(command, CommandType.EndOfTurn);
        }

        #endregion

        #region MoveTo

        /// <summary>
        /// Executes the MoveTo command, moving the unit to the tile
        /// </summary>
        /// <param name="command">The player command</param>
        /// <exception cref="GameElementNotFoundException">Thrown when the unit is not found in the currentBoard</exception>
        private static void ExecuteMoveToCommand(PlayerCommand command)
        {
            //validates the move to command
            ValidateMoveToCommand(command);

            //gets the real unit in the game elements manager
            Unit realUnit = GameElementsManager.Get<Unit>(command.Unit.ID);
            if (realUnit == null)
                throw new GameElementNotFoundException(command.Unit);

            //changes the unit object to the destination
            UnitActionsPerformer.MoveTo(BoardManager.Board, realUnit, command.Destination);

            //logs the movement
            GameLogger.Log(LogMessages.PlayerMovedUnitTo,
                new List<String>() { 
                    command.PlayerID.ToString(),
                    command.Unit.ID.ToString(),
                    command.Destination.ToString()
                });
        }

        /// <summary>
        /// Validates the MoveTo Command by validating the command type and the availability of the destination tile
        /// </summary>
        /// <param name="command">The Player Command</param>
        private static void ValidateMoveToCommand(PlayerCommand command)
        {
            //validates the move to command
            ValidateCommandType(command, CommandType.MoveTo);
            ValidateTileUnitsOwnedOnlyBy(command.Destination, command.PlayerID, true);

            //important: the movement cost is validated in the Unit class, so it does not need to be done here again
        }

        #endregion

        #region AttackAt

        /// <summary>
        /// Executes the AttackAt command
        /// </summary>
        /// <param name="command">The player command</param>
        /// <exception cref="GameElementNotFoundException">Thrown when the unit is not found in the currentBoard</exception>
        private static void ExecuteAttackCommand(PlayerCommand command)
        {
            //validates the attack command
            ValidateAttackCommand(command);

            //gets the real unit in the game elements manager
            Unit realUnit = GameElementsManager.Get<Unit>(command.Unit.ID);
            if (realUnit == null)
                throw new GameElementNotFoundException(command.Unit);

            //performs the attack
            UnitActionsPerformer.AttackAt(BoardManager.Board, realUnit, command.Destination);

            //logs the attack
            GameLogger.Log(LogMessages.PlayerAttackedAt,
                new List<String>() { 
                    command.PlayerID.ToString(),
                    command.Destination.ToString(),
                    command.Unit.ID.ToString()
                });
        }

        /// <summary>
        /// Validates the AttackAt Command
        /// </summary>
        /// <param name="command">The Player Command</param>
        private static void ValidateAttackCommand(PlayerCommand command)
        {
            //validates the AttackAt command
            ValidateCommandType(command, CommandType.Attack);
            ValidateTileUnitsNotOwnedBy(command.Destination, command.PlayerID, false);
        }

        #endregion

        #region InfluenceAt

        /// <summary>
        /// Executes the InfluenceAt command
        /// </summary>
        /// <param name="command">The player command</param>
        /// <exception cref="GameElementNotFoundException">Thrown when the unit is not found in the currentBoard</exception>
        private static void ExecuteInfluenceAt(PlayerCommand command)
        {
            //validates the influence at command
            ValidateInfluenceAtCommand(command);

            //gets the real unit in the game elements manager
            Unit realUnit = GameElementsManager.Get<Unit>(command.Unit.ID);
            if (realUnit == null)
                throw new GameElementNotFoundException(command.Unit);

            //performs the territory influence
            UnitActionsPerformer.InfluenceAt(BoardManager.Board, realUnit, command.Destination);

            //logs the influence
            GameLogger.Log(LogMessages.PlayerInfluencedAt,
                new List<String>() { 
                    command.PlayerID.ToString(),
                    command.Destination.ToString(),
                    command.Unit.ID.ToString()
                });
        }

        /// <summary>
        /// Validates the InfluenceAt command
        /// </summary>
        /// <param name="command">The Player Command</param>
        private static void ValidateInfluenceAtCommand(PlayerCommand command)
        {
            //validates the InfluenceAt command
            ValidateCommandType(command, CommandType.InfluenceAt);
            ValidatePosition(command.Destination);
        }

        #endregion

        #region EquipUnit

        /// <summary>
        /// Executes the EquipUnit command
        /// </summary>
        /// <param name="command">The Player command</param>
        /// <exception cref="GameElementNotFoundException">Thrown when the unit is not found in the currentBoard</exception>
        /// <exception cref="NoBuildingInTileException">Thrown when there is no building in the tile of the unit</exception>
        /// <exception cref="NoSuchItemInInventory">Thrown when the desired item is not in the building's inventory</exception>
        /// <exception cref="UnitAlreadyHasItemException">Thrown when the unit already has the given item</exception>
        private static void ExecuteEquipUnit(PlayerCommand command)
        {
            //validates the EquipUnit command
            ValidateEquipUnitCommand(command);

            //gets the real unit in the game elements manager
            Unit realUnit = GameElementsManager.Get<Unit>(command.Unit.ID);
            if (realUnit == null)
                throw new GameElementNotFoundException(command.Unit);

            //gets the building in the unit's tile
            Building building = BoardManager.Board.GetTile(realUnit.Position).Building;
            if (building == null)
                throw new NoBuildingInTileException(realUnit.Position);

            //gets the desired item in the building
            if (!building.UnitItemsInventory.Contains((ItemType)command.UnitItem))
                throw new NoSuchItemInInventory(realUnit.Position, (ItemType)command.UnitItem);
            ItemType item = (ItemType)command.UnitItem;

            //checks if the unit already has the item
            if (realUnit.Items.Contains(item))
                throw new UnitAlreadyHasItemException(command.Unit, item);

            //adds the item to the unit's list
            realUnit.Items.Add(item);

            //removes the itemtype from building's inventory
            building.UnitItemsInventory.Remove(item);

            //logs the equipunit command
            GameLogger.Log(LogMessages.PlayerEquippedUnitWith,
                new List<String>() { 
                    command.PlayerID.ToString(),
                    command.Unit.ID.ToString(),
                    command.UnitItem.ToString()
                });
        }

        /// <summary>
        /// Validates the EquipUnit command
        /// </summary>
        /// <param name="command">The Player Command</param>
        private static void ValidateEquipUnitCommand(PlayerCommand command)
        {
            //validates the InfluenceAt command
            ValidateCommandType(command, CommandType.EquipUnit);
        }

        #endregion

        #region SetProduction

        /// <summary>
        /// Executes the SetProduction command
        /// </summary>
        /// <param name="command">The Player command</param>
        /// <exception cref="GameElementNotFoundException">Thrown when the building is not found in the currentBoard</exception>
        private static void ExecuteSetProduction(PlayerCommand command)
        {
            //Validates the SetProduction command
            ValidateSetProductionCommand(command);

            //gets the building in the game elements manager
            Building building = GameElementsManager.Get<Building>(command.Building.ID);
            if (building == null)
                throw new GameElementNotFoundException(command.Building);

            //if the building is already producing the given production, don't do anything
            if (building.CurrentProduction != null && building.CurrentProduction.Equals(command.Production))
                return;

            //changes the building's production to the given one
            building.CurrentProduction = command.Production;

            //logs the production set
            GameLogger.Log(LogMessages.PlayerSetProductionOf,
                new List<String>() { 
                    command.PlayerID.ToString(),
                    command.Production.ToString(),
                    command.Building.ID.ToString()
                });
        }

        /// <summary>
        /// Validates the SetProduction command
        /// </summary>
        /// <param name="command">The Player Command</param>
        private static void ValidateSetProductionCommand(PlayerCommand command)
        {
            //validates the SetProduction command
            ValidateCommandType(command, CommandType.SetProduction);
        }

        #endregion

        #region CreateBuilding

        /// <summary>
        /// Executes the CreateBuilding command
        /// </summary>
        /// <param name="command">The Player command</param>
        /// <exception cref="GameElementNotFoundException">Thrown when the building is not found in the currentBoard</exception>
        private static void ExecuteCreateBuilding(PlayerCommand command)
        {
            //validates the CreateBuilding command
            ValidateCreateBuildingCommand(command);

            //gets the real unit in the game elements manager
            Unit realUnit = GameElementsManager.Get<Unit>(command.Unit.ID);
            if (realUnit == null)
                throw new GameElementNotFoundException(command.Unit);

            //performs the building creation
            UnitActionsPerformer.CreateBuilding(BoardManager.Board, realUnit, (BuildingType)command.BuildingCreating);

            //logs the building creation
            GameLogger.Log(LogMessages.PlayerCreatedBuilding,
                new List<String>() { 
                    command.PlayerID.ToString(),
                    command.BuildingCreating.ToString(),
                    command.Unit.ID.ToString()
                });
        }

        /// <summary>
        /// Validates the CreateBuilding command
        /// </summary>
        /// <param name="command">The Player Command</param>
        private static void ValidateCreateBuildingCommand(PlayerCommand command)
        {
            //validates the CreateBuilding command
            ValidateCommandType(command, CommandType.CreateBuilding);
            ValidateTileMajorityDomain(command.Unit.Position, command.PlayerID);
            ValidateBuildingType(command.BuildingCreating);
            ValidateNoBuildingInTile(command.Unit.Position);
            ValidateTileCanReceiveBuilding(command.Unit.Position);
        }

        #endregion

        #endregion
    }
}
