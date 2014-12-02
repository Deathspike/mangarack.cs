// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using MangaRack.Provider.Abstract;

namespace MangaRack.Provider.KissManga {
    /// <summary>
    /// Represents a KissManga provider.
    /// </summary>
    public sealed class KissManga : IProvider {
        /// <summary>
        /// Contains the provider.
        /// </summary>
        private readonly IProvider _provider;

        #region Constructor
        /// <summary>
        /// Initialize a new instance of the KissManga class.
        /// </summary>
        private KissManga() {
            _provider = new Concrete.Provider();
        }
        #endregion

        #region IProvider:Methods
        /// <summary>
        /// Open a series.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique identifier.</param>
        public ISeries Open(string uniqueIdentifier) {
            return _provider.Open(uniqueIdentifier);
        }

        /// <summary>
        /// Search series.
        /// </summary>
        /// <param name="input">The input.</param>
        public ISearch Search(string input) {
            return _provider.Search(input);
        }
        #endregion

        #region IProvider:Properties
        /// <summary>
        /// Contains the location.
        /// </summary>
        public string Location {
            get { return _provider.Location; }
        }
        #endregion
    }
}