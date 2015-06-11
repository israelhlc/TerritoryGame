using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TerritoryGame")]

namespace Common.Util
{
    /// <summary>
    /// An utility class for random numbers
    /// </summary>
    public static class RandomUtil
    {
        #region Static Members

        private static Random _randomInstance = new Random();

        #endregion

        #region Properties

        /// <summary>
        /// A unique random number generator
        /// </summary>
        public static Random Random
        {
            get
            {
                return _randomInstance;
            }
        }

        /// <summary>
        /// Returns a new random integer
        /// </summary>
        public static int NextInt
        {
            get
            {
                return _randomInstance.Next();
            }
        }

        /// <summary>
        /// Returns a new random long
        /// </summary>
        public static long NextLong
        {
            get
            {
                var buffer = new byte[sizeof(Int64)];
                _randomInstance.NextBytes(buffer);
                return BitConverter.ToInt64(buffer, 0);
            }
        }

        /// <summary>
        /// Returns a new random uint
        /// </summary>
        public static uint NextUInt
        {
            get
            {
                var buffer = new byte[sizeof(UInt32)];
                _randomInstance.NextBytes(buffer);
                return BitConverter.ToUInt32(buffer, 0);
            }
        }

        /// <summary>
        /// Returns a new random double
        /// </summary>
        public static double NextDouble
        {
            get
            {
                return _randomInstance.NextDouble();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Reinstantiates the randomizer with the default seed
        /// </summary>
        internal static void Reinstantiate()
        {
            _randomInstance = new Random();
        }

        /// <summary>
        /// Reinstantiates the randomizer with the given seed
        /// </summary>
        /// <param name="seed"></param>
        internal static void Reinstantiate(int seed)
        {
            _randomInstance = new Random(seed);
        }

        /// <summary>
        /// Gets the next double normalized (between the minimum and maximum values)
        /// </summary>
        /// <param name="minimumValue">The minimum value for the double</param>
        /// <param name="maximumValue">The maximum value for the double</param>
        /// <returns>A normlized double</returns>
        public static double NextNormalizedDouble(double minimumValue, double maximumValue = 1)
        {
            return maximumValue - ((1 - NextDouble) * (maximumValue - minimumValue));
        }

        #endregion
    }
}
