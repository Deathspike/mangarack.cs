// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System.Text.RegularExpressions;
using MangaRack.Provider.Abstract;

namespace MangaRack.Provider.KissManga.Concrete {
    /// <summary>
    /// Represents a KissManga provider.
    /// </summary>
    internal sealed class Provider : IProvider {
        #region Properties
        /// <summary>
        /// Contains the domain.
        /// </summary>
        public static string Domain {
            get { return "http://kissmanga.com/"; }
        }
        #endregion

        #region IProvider:Methods
        /// <summary>
        /// Open a series.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique identifier.</param>
        public ISeries Open(string uniqueIdentifier) {
            return Regex.Match(uniqueIdentifier, @"^http://kissmanga\.com/Manga/(.*)$", RegexOptions.IgnoreCase).Success ? new Series(uniqueIdentifier) : null;
        }

        /// <summary>
        /// Search series.
        /// </summary>
        /// <param name="input">The input.</param>
        public ISearch Search(string input) {
            return new Search(input);
        }
        #endregion

        #region IProvider:Properties
        /// <summary>
        /// Contains the location.
        /// </summary>
        public string Location {
            get { return Domain; }
        }
        #endregion
    }
}