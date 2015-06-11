using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Common.General;
using Common.Resources.Buildings;
using Common.Resources.Terrains;
using Common.Resources.Units;

[assembly: InternalsVisibleTo("TerritoryGame")]

namespace Common.Resources
{
    /// <summary>
    /// The class for a tile in the map.
    /// It contains a terrain, and the terrain properties must be accessed in the TileTerrain class
    /// </summary>
    public class Tile : GameElement
    {
        #region Constants

        /// <summary>
        /// The sum of the domains when the tile has been normalized
        /// </summary>
        public const double NORMALIZED_DOMAIN_SUM = 1.0;

        #endregion

        #region Properties

        /// <summary>
        /// Type of the terrain in the tile
        /// </summary>
        public TerrainType Terrain
        {
            get;
            internal set;
        }

        /// <summary>
        /// The units stationed in the tile
        /// </summary>
        public List<Unit> Units
        {
            get;
            internal set;
        }

        /// <summary>
        /// The building in the tile - it can be null
        /// </summary>
        public Building Building
        {
            get;
            internal set;
        }

        /// <summary>
        /// The tile domain, indexed by the player ID
        /// </summary>
        public Dictionary<int, double> Domain
        {
            get;
            internal set;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a tile with a random terrain typeand the specified number of players
        /// </summary>
        /// <param name="numberOfPlayers">Number of players in the game</param>
        public Tile(int numberOfPlayers) : this(numberOfPlayers, Terrains.Terrain.RandomTerrainType) { }

        /// <summary>
        /// Initializes a tile with the specified terrain type and number of players
        /// </summary>
        /// <param name="numberOfPlayers">Number of players in the game</param>
        /// <param name="terrain">The type of the terrain</param>
        public Tile(int numberOfPlayers, TerrainType terrain) : this(numberOfPlayers, terrain, null) { }

        /// <summary>
        /// Initializes a tile with the specified attributes
        /// </summary>
        /// <param name="numberOfPlayers">Number of players in the game</param>
        /// <param name="terrain">The type of the terrain</param>
        /// <param name="building">The building on the tile</param>
        public Tile(int numberOfPlayers, TerrainType terrain, Building building)
        {
            Terrain = terrain;
            Units = new List<Unit>();
            Building = building;
            Domain = new Dictionary<int, double>(numberOfPlayers);
        }

        /// <summary>
        /// Creates a new tile based on another one (deep copy)
        /// </summary>
        /// <param name="baseTile">The base tile</param>
        protected Tile(Tile baseTile)
            : base(baseTile)
        {
            //copies the attributes
            Terrain = baseTile.Terrain;
            Building = baseTile.Building;

            //copies the units list
            Units = new List<Unit>(baseTile.Units.Count);
            List<Unit> units = baseTile.Units.ToList();
            Units.AddRange(from Unit unit in units select (Unit)unit.Clone());

            //copies the domains
            Domain = new Dictionary<int, double>(baseTile.Domain.Count);
            List<KeyValuePair<int, double>> domains = baseTile.Domain.ToList();
            foreach (KeyValuePair<int, double> domain in domains)
                Domain.Add(domain.Key, domain.Value);

        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds the domain to the dicionary.
        /// If the player already has a domain, it will sum the current with the new domain
        /// </summary>
        /// <param name="playerID">The player ID</param>
        /// <param name="domain">The domain being added to the tile</param>
        /// <param name="normalizeAfterAdding">Indicates if the domains need to be normalized after adding</param>
        public void AddDomain(int playerID, double domain, bool normalizeAfterAdding = true)
        {
            //gets the current domain for the playerID
            double currentDomain;

            //if there is a value, get it and then remove it
            if (Domain.TryGetValue(playerID, out currentDomain))
                Domain.Remove(playerID);

            //adds up the domain to the current domain
            currentDomain += domain;

            //don't let it be less than 0
            if (currentDomain < 0)
                currentDomain = 0;

            //adds the domain back to the dictionary
            Domain.Add(playerID, currentDomain);

            //normalizes the domains
            if (normalizeAfterAdding)
                NormalizeDomains();
        }

        /// <summary>
        /// Normalizes the domains of the tile so the sum of domains is 1.
        /// If there is no domain, the sum will be obviously 0.
        /// </summary>
        public void NormalizeDomains()
        {
            //gets the list of keys/value
            List<KeyValuePair<int, double>> domainsList = new List<KeyValuePair<int, double>>();

            //the domains' sum
            double domainsSum = 0;

            //iterates through the keys of the dicionary
            foreach (int playerID in Domain.Keys)
            {
                //gets the domain value
                double domain;
                Domain.TryGetValue(playerID, out domain);

                //adds it to the domains' sum
                domainsSum += domain;

                //adds the key/value pair to the list
                domainsList.Add(new KeyValuePair<int, double>(playerID, domain));
            }

            //clears the Domains
            Domain.Clear();

            //if there is no domain, leave it cleared and do nothing
            if (domainsSum == 0)
                return;

            //gets the normalizer factor for the domains
            double normalizerFactor = NORMALIZED_DOMAIN_SUM / domainsSum;

            //iterates through the valued pairs to add the key/values back to the Domain
            foreach (KeyValuePair<int, double> domain in domainsList)
            {
                Domain.Add(domain.Key, domain.Value * normalizerFactor);
            }
        }

        /// <summary>
        /// Removes the domain of the player.
        /// It must be used when the player has been killed
        /// </summary>
        /// <param name="playerID">The player ID</param>
        public void RemoveDomain(int playerID)
        {
            //removes the player domain from this tile
            Domain.Remove(playerID);

            //normalizes the domains
            NormalizeDomains();
        }

        /// <summary>
        /// Checks if the given player has units or building in the tile
        /// </summary>
        /// <param name="playerID">The player ID</param>
        /// <returns>true iif the player has units or building in the tile</returns>
        public bool ExistsPlayer(int playerID)
        {
            //searches for units
            if (Units.Exists(unit => unit.Owner == playerID))
                return true;

            //checks the building
            if (Building != null && Building.Owner == playerID)
                return true;

            //if no unit or building was found, the player is not in the tile
            return false;
        }

        /// <summary>
        /// Gets the game elements (units and buildings) of a given player
        /// </summary>
        /// <param name="playerID">The player ID</param>
        /// <returns>The list containing all player's elements in the tile</returns>
        public List<GameElement> GetGameElements(int playerID)
        {
            //creates the return list
            List<GameElement> playerElements = new List<GameElement>();

            //add the units
            playerElements.AddRange(Units.FindAll(unit => unit.Owner == playerID));

            //add the building
            if (Building != null && Building.Owner == playerID)
                playerElements.Add(Building);

            //returns the list with the game elements
            return playerElements;
        }

        #region ICloneable

        /// <summary>
        /// Clones the tile, return a new deep copy
        /// </summary>
        /// <returns>A deep copy of the tile</returns>
        public override Object Clone()
        {
            return new Tile(this);
        }

        #endregion

        #endregion
    }
}
