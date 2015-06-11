using System;
using System.Collections.Generic;
using Common.Resources;
using Common.Resources.Terrains;
using Common.Resources.Units.Exceptions;

namespace Common.Util
{
    /// <summary>
    /// Class with some methods to help movement and routing in the game.
    /// It is, of course, based on a hexagonal board
    /// </summary>
    public static class MovementUtil
    {
        #region A* Algorithm

        /// <summary>
        /// Gets the best (minimum movement cost) route between two list for a given board.
        /// It uses the A* algorithm.
        /// </summary>
        /// <param name="board">The game board</param>
        /// <param name="start">The start position</param>
        /// <param name="end">The end position (goal)</param>
        /// <param name="playerID">The ID of the player for which the route will be calculated</param>
        /// <returns>The list with the best route between the positions</returns>
        public static List<Position> CalculateBestRouteTo(Board board, Position start, Position end, int playerID)
        {
            return CalculateBestRouteTo(board, start, end, playerID, true, false, false);
        }

        /// <summary>
        /// Gets the best (minimum movement cost) route between two list for a given board.
        /// It uses the A* algorithm.
        /// </summary>
        /// <param name="board">The game board</param>
        /// <param name="start">The start position</param>
        /// <param name="end">The end position (goal)</param>
        /// <param name="playerID">The ID of the player for which the route will be calculated</param>
        /// <param name="groundTerrain">Include ground terrains in the route</param>
        /// <param name="waterTerrain">Include water terrains in the route</param>
        /// <param name="aerialTerrain">Include aerial terrains in the route</param>
        /// <returns>The list with the best route between the positions</returns>
        public static List<Position> CalculateBestRouteTo(Board board, Position start, Position end, int playerID,
            bool groundTerrain, bool waterTerrain, bool aerialTerrain)
        {
            //creates the open and closed node sets
            List<Node> openNodes = new List<Node>();
            HashSet<Node> closedNodes = new HashSet<Node>();

            //map between list and nodes
            Dictionary<Position, Node> nodesDictionary = new Dictionary<Position, Node>();

            //creates the start node
            Node startNode = new Node(start);
            nodesDictionary.Add(start, startNode);

            //sets the start node values
            startNode.SetAttributes(g: 0, h: CalculateManhattanDistance(start, end), parent: null);

            //adds the start node to the openNodes
            openNodes.Add(startNode);

            //the current node
            Node node;

            //iterates while there are open nodes
            while (openNodes.Count > 0)
            {
                //pop node from open nodes
                node = openNodes[0];
                openNodes.Remove(node);

                //checks if reached the goal
                if (node.Equals(end))
                {
                    //creates the return path
                    return CreateRoute(node);
                }

                //open the neighbours
                ExpandNeighbours(board, end, playerID, openNodes, closedNodes, nodesDictionary, node, groundTerrain, waterTerrain, aerialTerrain);

                //sorts the open nodes
                openNodes.Sort();

                //adds the node to the closed list
                closedNodes.Add(node);
            }

            //no path was found - throw an exception
            throw new ImpossibleRouteException(board, start, end);
        }

        /// <summary>
        /// Expands (opens) the neighbours of a node in the A* algorithm on method CalculateBestRouteTo
        /// </summary>
        /// <param name="board">The game board</param>
        /// <param name="end">The end position (goal)</param>
        /// <param name="playerID">The ID of the player for which the route will be calculated</param>
        /// <param name="openNodes">The list of the open nodes</param>
        /// <param name="closedNodes">The set of closed nodes</param>
        /// <param name="nodesDictionary">The dictionary of </param>
        /// <param name="node">The node for which the neighbours will be expanded</param>
        /// <param name="groundTerrain">Include ground terrains in the route</param>
        /// <param name="waterTerrain">Include water terrains in the route</param>
        /// <param name="aerialTerrain">Include aerial terrains in the route</param>
        private static void ExpandNeighbours(Board board, Position end, int playerID, List<Node> openNodes, HashSet<Node> closedNodes,
            Dictionary<Position, Node> nodesDictionary, Node node, bool groundTerrain, bool waterTerrain, bool aerialTerrain)
        {
            uint newG;                  //the new real cost to reach the node
            Node newNode;               //the new node

            //gets the node's neighbours
            List<Position> neighbours = node.GetNeighbours();

            //iterates through the neighbours
            foreach (Position position in neighbours)
            {
                //if the tile is unknown or has other players' units, skip it
                TerrainType terrainType = board.GetTile(position).Terrain;
                if ((terrainType == TerrainType.Unknown) || board.GetTile(position).Units.Exists(unit => unit.Owner != playerID))
                    continue;

                //if the terrain does not allow groud/water/aerial movement wanted, skip it
                Terrain terrain = Terrains.Get(terrainType);
                if (!((terrain.CanReceiveGroundUnits && groundTerrain) ||
                      (terrain.CanReceiveWaterUnits && waterTerrain) ||
                      (terrain.CanReceiveAerialUnits && aerialTerrain)))
                    continue;

                //calculates the new real cost until the node
                newG = node.G + terrain.MovementCost;

                //checks if the new node is already found
                if (nodesDictionary.TryGetValue(position, out newNode))
                {
                    //checks if reached an expanded node with a lower real cost
                    if (newNode.G <= newG)
                        continue;
                }
                //if the node was not found, creates the new node
                else
                {
                    newNode = new Node(position);
                    nodesDictionary.Add(position, newNode);
                }

                //sets the new node values
                newNode.SetAttributes(g: newG, h: CalculateManhattanDistance(position, end), parent: node);

                //checks if new node had been closed to set it as open again
                if (closedNodes.Contains(newNode))
                    closedNodes.Remove(newNode);

                //checks if new node is not yet in open nodes to add it
                if (!openNodes.Contains(newNode))
                    openNodes.Add(newNode);
            }
        }

        /// <summary>
        /// Creates the route to the node, walking backwards through its parents
        /// </summary>
        /// <param name="node">The end node of the route</param>
        /// <returns>The route to the given node</returns>
        private static List<Position> CreateRoute(Node node)
        {
            //creates a linked list to make operations faster
            LinkedList<Position> bestRoute = new LinkedList<Position>();

            //iterate on parents
            do
            {
                //adds the node to the beginning of the list
                bestRoute.AddFirst(node);
                node = node.Parent;
            } while (node != null);

            //returns the list
            return new List<Position>(bestRoute);
        }

        #endregion

        #region General Utility Methods

        /// <summary>
        /// Calculates the route cost by addings the terrains' movement cost.
        /// It does not validate if the given route is possible or not.
        /// </summary>
        /// <param name="board">The game board</param>
        /// <param name="route">The route, including the start and end nodes</param>
        /// <returns>The movements cost for the given route</returns>
        public static uint CalculateRouteCost(Board board, List<Position> route)
        {
            //the total movement cost of the route
            uint routeCost = 0;

            //iterate through the list beginning in the second (there is no movement to the start position)
            for (int i = 1; i < route.Count; i++)
            {
                //increments the route cost
                routeCost += Terrains.Get(board.GetTile(route[i]).Terrain).MovementCost;
            }

            //returns the route cost
            return routeCost;
        }

        /// <summary>
        /// Calculates the hexagonal manhattan distance between two Positions.
        /// </summary>
        /// <param name="start">The start position</param>
        /// <param name="end">The end position</param>
        /// <returns>The calculated manhattan distance between the given list</returns>
        public static uint CalculateManhattanDistance(Position start, Position end)
        {
            int manhattanDistance = 0;
            Position currentPosition = start;

            while (currentPosition != end)
            {
                //calculates the xDelta and yDelta
                int xDelta = Math.Abs(currentPosition.X - end.X);
                int yDelta = Math.Abs(currentPosition.Y - end.Y);

                //the x coordinate is the same, must get only the y difference to move up/down
                if (currentPosition.X == end.X)
                {
                    manhattanDistance += yDelta;
                    break;
                }

                //the y coordinate is the same, must get only the x difference to move northeast/southwest
                if (currentPosition.Y == end.Y)
                {
                    manhattanDistance += xDelta;
                    break;
                }

                //won't go up or down, go to nearest diagonal neighbour
                currentPosition = GetNearestDiagonalNeighbour(currentPosition, end);

                //increment the movement
                manhattanDistance++;
            }

            //returns the manhattan distance
            return (uint)manhattanDistance;
        }

        /// <summary>
        /// Gets the nearest neighbour using only diagonal movements:
        /// Southeast, Southwest, Northeast, Northwest
        /// </summary>
        /// <param name="start">The start position</param>
        /// <param name="end">The destination position</param>
        /// <returns></returns>
        public static Position GetNearestDiagonalNeighbour(Position start, Position end)
        {
            //x is smaller -> move east
            if (start.X < end.X)
            {
                //y is smaller -> move southeast
                if (start.Y < end.Y)
                {
                    return start.Southeast;
                }
                //y is greater -> move northeast
                else
                {
                    return start.Northeast;
                }
            }

            //x is greater -> move west
            else
            {
                //y is smaller -> move southwest
                if (start.Y < end.Y)
                {
                    return start.Southwest;
                }
                //y is greater -> move northwest
                else
                {
                    return start.Northwest;
                }
            }
        }

        #region Class Node

        /// <summary>
        /// Class representing a node (position) in the graph.
        /// It is used in the routing A* algorithim
        /// </summary>
        private class Node : Position, IComparable
        {
            #region Properties

            /// <summary>
            /// The real cost from the start to the current node
            /// </summary>
            public uint G
            {
                get;
                set;
            }

            /// <summary>
            /// The estimated cost (heuristic) to reach the goal from the current node
            /// </summary>
            public uint H
            {
                get;
                set;
            }

            /// <summary>
            /// The evaluation function for the cost to reach the goal through the current node. It is the same as G + H.
            /// </summary>
            public uint F
            {
                get
                {
                    return G + H;
                }
            }

            /// <summary>
            /// The parent node in the route, i. e. the from which the current node was expanded
            /// </summary>
            public Node Parent
            {
                get;
                set;
            }

            #endregion

            #region Constructor

            /// <summary>
            /// Creates a new node based on a position
            /// </summary>
            /// <param name="position"></param>
            public Node(Position position) : base(position) { }

            #endregion

            #region Methods

            /// <summary>
            /// Sets the attributes for the node - only to reduce code lines
            /// </summary>
            /// <param name="g">The real cost from the start to the current node</param>
            /// <param name="h">The estimated cost (heuristic) to reach the goal from the current node</param>
            /// <param name="parent">The parent node in the route, i. e. the from which the current node was expanded</param>
            public void SetAttributes(uint g, uint h, Node parent)
            {
                G = g;
                H = h;
                Parent = parent;
            }

            #region IComparable

            /// <summary>
            /// Compares two nodes based on the F attribute
            /// </summary>
            /// <param name="obj">The node to compare this to</param>
            /// <returns>The comparison result between F attributes of this and obj</returns>
            public int CompareTo(object obj)
            {
                //casts the node - if it is not a Node an exception will be thrown
                Node otherNode = (Node)obj;

                //returns the comparison of the F attribute
                return this.F.CompareTo(otherNode.F);
            }

            #endregion

            #endregion
        }

        #endregion

        #endregion
    }
}
