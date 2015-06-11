using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TerritoryGame.Log
{
    /// <summary>
    /// The logger class for the TerritoryGame.
    /// It is not an application log for errors, but a game log to be used for actions taken etc
    /// </summary>
    internal static class GameLogger
    {
        #region Properties

        /// <summary>
        /// The stream o write on the local file when properly set up in the settings
        /// </summary>
        private static StreamWriter File
        {
            get;
            set;
        }

        #endregion

        #region Initializion and Disposal

        /// <summary>
        /// Initializes the logger, opening the files if needed
        /// </summary>
        internal static void Initialize()
        {
            //if set up to log on local file, open its path
            if (Logger.Default.LogLocalFile)
            {
                File = new StreamWriter(@Logger.Default.LocalFile, true);
                File.AutoFlush = true;
            }
        }

        /// <summary>
        /// Closes the log connections on local files
        /// </summary>
        internal static void Close()
        {
            //if the stream was initialized, close it
            if (File != null)
                File.Close();
        }

        #endregion

        #region Log Methods

        /// <summary>
        /// Logs a message from the game.
        /// It may be logged in a local file and/or console
        /// </summary>
        /// <param name="message">The message to be logged</param>
        internal static void Log(String message)
        {
            //adds the current time to the message
            StringBuilder finalMessage = new StringBuilder();
            finalMessage.Append(DateTime.Now.ToString(LogMessages.DateTimeFormat));
            finalMessage.Append(" ");
            finalMessage.Append(message);

            //logs message in the console
            if (Logger.Default.LogConsole)
                Console.WriteLine(finalMessage.ToString());

            //logs message in the local file
            if (Logger.Default.LogLocalFile)
            {
                File.WriteLine(finalMessage.ToString());
            }
        }

        /// <summary>
        /// Logs a message from the game.
        /// It may be logged in a local file and/or console
        /// </summary>
        /// <param name="message">The message to be logged</param>
        /// <param name="parameters">The parameters of the message</param>
        internal static void Log(String message, List<String> parameters)
        {
            //the final message
            StringBuilder finalMessage = new StringBuilder(message);

            //iterate through the parameters to replace them into the message
            for (int i = 0; i < parameters.Count; i++)
                finalMessage.Replace("{" + i + "}", parameters[i]);

            //log the message
            Log(finalMessage.ToString());
        }

        #endregion
    }
}
