// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using MangaRack.Provider.Abstract;

namespace MangaRack.Provider.Concrete {
    /// <summary>
    /// Represents a provider.
    /// </summary>
    internal sealed class Provider : IProvider {
        /// <summary>
        /// Contains the provider.
        /// </summary>
        private readonly IProvider _provider;

        #region Constructor
        /// <summary>
        /// Initialize a new instance of the Provider class.
        /// </summary>
        /// <param name="provider">The provider.</param>
        public Provider(IProvider provider) {
            _provider = provider;
        }
        #endregion

        #region IProvider:Methods
        /// <summary>
        /// Open a series.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique identifier.</param>
        public ISeries Open(string uniqueIdentifier) {
            var series = _provider.Open(uniqueIdentifier);
            return series == null ? null : new Series(series);
        }

        /// <summary>
        /// Search series.
        /// </summary>
        /// <param name="input">The input.</param>
        public ISearch Search(string input) {
            var search = _provider.Search(input);
            return search == null ? null : new Search(search);
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