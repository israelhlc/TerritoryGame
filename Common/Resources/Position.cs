using System;
using System.Collections.Generic;
using System.Text;
using Common.Settings;
using Common.Util;

namespace Common.Resources
{
    /// <summary>
    /// The board position of any positionable game element (e. g. units)
    /// </summary>
    public class Position : ICloneable
    {
        #region Relative Neighbours

        /// <summary>
        /// The relative coordinate for a position to the north
        /// </summary>
        public static Position NORTH = new Position(0, -1);

        /// <summary>
        /// The relative coordinate for a position to the northeast when the current position is in an even column
        /// </summary>
        public static Position NORTHEAST_EVEN = new Position(1, -1);

        /// <summary>
        /// The relative coordinate for a position to the northeast when the current position is in an odd column
        /// </summary>
        public static Position NORTHEAST_ODD = new Position(1, 0);

        /// <summary>
        /// The relative coordinate for a position to the southeast when the current position is in an even column
        /// </summary>
        public static Position SOUTHEAST_EVEN = new Position(1, 0);

        /// <summary>
        /// The relative coordinate for a position to the southeast when the current position is in an odd column
        /// </summary>
        public static Position SOUTHEAST_ODD = new Position(1, 1);

        /// <summary>
        /// The relative coordinate for a position to the south
        /// </summary>
        public static Position SOUTH = new Position(0, 1);

        /// <summary>
        /// The relative coordinate for a position to the southwest when the current position is in an even column
        /// </summary>
        public static Position SOUTHWEST_EVEN = new Position(-1, 0);

        /// <summary>
        /// The relative coordinate for a position to the southwest when the current position is in an odd column
        /// </summary>
        public static Position SOUTHWEST_ODD = new Position(-1, 1);

        /// <summary>
        /// The relative coordinate for a position to the northwest when the current position is in an even column
        /// </summary>
        public static Position NORTHWEST_EVEN = new Position(-1, -1);

        /// <summary>
        /// The relative coordinate for a position to the northwest when the current position is in an odd column
        /// </summary>
        public static Position NORTHWEST_ODD = new Position(-1, 0);

        #endregion

        #region Properties

        /// <summary>
        /// The X axis coordinate
        /// </summary>
        public int X
        {
            get;
            private set;
        }

        /// <summary>
        /// The Y axis coordinate
        /// </summary>
        public int Y
        {
            get;
            private set;
        }

        /// <summary>
        /// Indicates if the position is in an even column.
        /// It is true if Y is even, and false if it is odd.
        /// </summary>
        public bool EvenColumn
        {
            get
            {
                return X % 2 == 0;
            }
        }

        /// <summary>
        /// The position to the North of this position
        /// </summary>
        public Position North
        {
            get
            {
                return this + NORTH;
            }
        }

        /// <summary>
        /// The position to the Northeast of this position
        /// </summary>
        public Position Northeast
        {
            get
            {
                return this + (EvenColumn ? NORTHEAST_EVEN : NORTHEAST_ODD);
            }
        }

        /// <summary>
        /// The position to the Southeast of this position
        /// </summary>
        public Position Southeast
        {
            get
            {
                return this + (EvenColumn ? SOUTHEAST_EVEN : SOUTHEAST_ODD);
            }
        }

        /// <summary>
        /// The position to the South of this position
        /// </summary>
        public Position South
        {
            get
            {
                return this + SOUTH;
            }
        }

        /// <summary>
        /// The position to the Southwest of this position
        /// </summary>
        public Position Southwest
        {
            get
            {
                return this + (EvenColumn ? SOUTHWEST_EVEN : SOUTHWEST_ODD);
            }
        }

        /// <summary>
        /// The position to the Northwest of this position
        /// </summary>
        public Position Northwest
        {
            get
            {
                return this + (EvenColumn ? NORTHWEST_EVEN : NORTHWEST_ODD);
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new board position based on a given position
        /// </summary>
        /// <param name="position">The position from which the attributes will be copied</param>
        public Position(Position position) : this(position.X, position.Y) { }

        /// <summary>
        /// Creates a new position for a given x and y
        /// </summary>
        /// <param name="x">The x axis</param>
        /// <param name="y">The y axis</param>
        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }

        #endregion

        #region Operators

        /// <summary>
        /// Adds two list as they were vectors
        /// </summary>
        /// <param name="firstPosition">The first position (operator left side)</param>
        /// <param name="secondPosition">The second position (operator right side)</param>
        /// <returns>The added (as a 2D Vector) position</returns>
        public static Position operator +(Position firstPosition, Position secondPosition)
        {
            return new Position(firstPosition.X + secondPosition.X, firstPosition.Y + secondPosition.Y);
        }

        /// <summary>
        /// Compares two position and returns true if they have the same coordinates
        /// </summary>
        /// <param name="firstPosition">The first position (operator left side)</param>
        /// <param name="secondPosition">The second position (operator right side)</param>
        /// <returns>true iif the two list have the same coordinates</returns>
        public static bool operator ==(Position firstPosition, Position secondPosition)
        {
            if (firstPosition as object == null)
            {
                if (secondPosition as object == null)
                    return true;
                else
                    return false;
            }
            else
                return firstPosition.Equals(secondPosition);
        }

        /// <summary>
        /// Compares two position and returns true if they have different coordinates
        /// </summary>
        /// <param name="firstPosition">The first position (operator left side)</param>
        /// <param name="secondPosition">The second position (operator right side)</param>
        /// <returns>true iif the two list do not have the same coordinates</returns>
        public static bool operator !=(Position firstPosition, Position secondPosition)
        {
            if (firstPosition as object == null)
            {
                if (secondPosition as object == null)
                    return false;
                else
                    return true;
            }
            else
                return !firstPosition.Equals(secondPosition);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Checks if a given position is adjacent to another one
        /// </summary>
        /// <param name="position">The position to check adjacency</param>
        /// <returns>true iif the given position is adjacent to this, i. e. it has a distance of 1</returns>
        public bool IsAdjacent(Position position)
        {
            //gets the neighbours of the position
            List<Position> neighbours = GetNeighbours();

            //returns true if the position is a neighbour and false otherwise
            return neighbours.Exists(pos => pos.Equals(position));
        }

        /// <summary>
        /// Gets the list of valid (inside board limits) neighbours of the position.
        /// </summary>
        /// <returns>The list of neighbours</returns>
        public List<Position> GetNeighbours()
        {
            //creates list with all the six neighbours
            List<Position> neighbours = new List<Position>();
            neighbours.Add(North);
            neighbours.Add(Northeast);
            neighbours.Add(Northwest);
            neighbours.Add(South);
            neighbours.Add(Southeast);
            neighbours.Add(Southwest);

            //removes invalid neighbours
            neighbours.RemoveAll(neighbour => neighbour.X < 0 || neighbour.X >= GameSettings.BoardWidth
                                           || neighbour.Y < 0 || neighbour.Y >= GameSettings.BoardHeight);

            //returns the list of neighbours
            return neighbours;
        }

        /// <summary>
        /// Gets the list of valid (inside board limits) neighbours of the position within the given range.
        /// </summary>
        /// <param name="range">The max range distance for the neighbours</param>
        /// <returns>The list of neighbours</returns>
        public List<Position> GetNeighbours(uint range)
        {
            //creates a dictionary to store the neighbours in each range - to reduce overprocessing
            Dictionary<uint, List<Position>> neighboursByDistance = new Dictionary<uint, List<Position>>();

            //adds the position (this) to the first range (0) list and adds it in the dictionary
            neighboursByDistance.Add(0, new List<Position>() { this });

            //iterate over the range, expanding the tiles
            for (uint i = 1; i <= range; i++)
            {
                //gets the list of neighbours in the last range (i - 1)
                List<Position> previousNeighbours;
                if (neighboursByDistance.TryGetValue(i - 1, out previousNeighbours))
                {
                    //gets the neighbours for all the previous neighbours
                    List<Position> newNeighbours = new List<Position>();
                    foreach (Position previousNeighbour in previousNeighbours)
                    {
                        newNeighbours.AddRange(previousNeighbour.GetNeighbours());
                    }

                    //remove duplicates
                    newNeighbours = ListUtil.GetListWithoutDuplicates(newNeighbours);

                    //remove already closed list
                    newNeighbours.RemoveAll(p => previousNeighbours.Contains(p));

                    //adds the new neighbours to the dictionary
                    neighboursByDistance.Add(i, newNeighbours);
                }
            }

            //creates a hash to store the neighbours without duplicates
            HashSet<Position> neighbours = new HashSet<Position>();

            //iterates through the dicionary keys
            for (uint i = 1; i <= range; i++)
            {
                //get the list in the dictionary
                List<Position> currentNeighbours;
                neighboursByDistance.TryGetValue(i, out currentNeighbours);

                //add all the items in the list to the hash
                foreach (Position neighbour in currentNeighbours)
                {
                    neighbours.Add(neighbour);
                }
            }

            //returns a list with the elements of the hash
            return new List<Position>(neighbours);
        }

        #region override

        /// <summary>
        /// Writes the position as a 2D Vector (e.g. '(x, y)')
        /// </summary>
        /// <returns>The vector representation of the position</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("(");
            builder.Append(X);
            builder.Append(", ");
            builder.Append(Y);
            builder.Append(")");
            return builder.ToString();
        }

        /// <summary>
        /// Compares two list
        /// </summary>
        /// <param name="obj">The other position</param>
        /// <returns>true iif the two list have the same coordinates</returns>
        public override bool Equals(object obj)
        {
            //verifies if parameter is null
            if (obj == null)
            {
                return false;
            }

            //tries to cast the object
            Position position = obj as Position;
            if (position == null)
            {
                return false;
            }

            //returns true if both coordinates are the same
            return (this.X == position.X && this.Y == position.Y);
        }

        /// <summary>
        /// Generates a hash code for the position. 
        /// It is based on both coordinates, using the 16 first bits of each coordinate to 
        /// create a new 32bit number as a hash code.
        /// </summary>
        /// <returns>The hash code for the Position object</returns>
        public override int GetHashCode()
        {
            return (X << 16) | ((Y << 16) >> 16);
        }

        #endregion

        #region ICloneable

        /// <summary>
        /// Clones the position, return a new deep copy
        /// </summary>
        /// <returns>A deep copy of the position</returns>
        public Object Clone()
        {
            return new Position(this);
        }

        #endregion

        #endregion
    }
}
