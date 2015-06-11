using System;
using System.Collections.Generic;

namespace Common.Commands.Exceptions
{
    /// <summary>
    /// An exception thrown when trying to create an invalid PlayerCommand.
    /// </summary>
    class InvalidPlayerCommandException : Exception
    {
        #region Members

        private List<Object> _invalidResources;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of InvalidPlayerCommandException, based on the invalid resources
        /// </summary>
        /// <param name="invalidResources">The invalid resources which caused the Exception</param>
        public InvalidPlayerCommandException(List<Object> invalidResources)
        {
            _invalidResources = invalidResources;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The invalid resources which caused the Exception
        /// </summary>
        public List<Object> InvalidResources
        {
            get
            {
                return _invalidResources;
            }
        }

        #endregion
    }
}
