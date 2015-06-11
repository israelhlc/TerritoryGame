using System;

namespace TerritoryGame
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (TerritoryGame game = new TerritoryGame())
            {
                game.Run();
            }
        }
    }
#endif
}

