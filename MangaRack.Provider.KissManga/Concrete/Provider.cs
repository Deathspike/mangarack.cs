// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MangaRack.Provider.Interfaces;

namespace MangaRack.Provider.KissManga {
	/// <summary>
	/// Represents a KissManga provider.
	/// </summary>
	class Provider : IProvider {
		#region Properties
		/// <summary>
		/// Contains the domain.
		/// </summary>
		public static string Domain {
			get {
				// Return the domain.
				return "http://kissmanga.com/";
			}
		}
		#endregion

		#region IProvider:Methods
		/// <summary>
		/// Open a series.
		/// </summary>
		/// <param name="location">The unique identifier.</param>
		public ISeries Open(string location) {
			// Check if the unique identifier can be handled.
			if (Regex.Match(location, @"^http://kissmanga\.com/Manga/(.*)$", RegexOptions.IgnoreCase).Success) {
				// Initialize a new instance of the Series class.
				return new Series(location);
			}
			// Return null.
			return null;
		}

		/// <summary>
		/// Search series.
		/// </summary>
		/// <param name="input">The input.</param>
        public void Search(string input, Action<IEnumerable<ISeries>> done)
        {
			// Initialize a new instance of the Search class.
			var search = new Search(input);
		    search.Populate(() => done(search));
        }
		#endregion

		#region IProvider:Properties
		/// <summary>
		/// Contains the location.
		/// </summary>
		public string Location {
			get {
				// Return the domain.
				return Domain;
			}
		}
		#endregion
	}
}