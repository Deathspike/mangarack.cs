// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System.Text.RegularExpressions;

namespace MangaRack.Provider.KissManga {
	/// <summary>
	/// Represents a KissManga provider.
	/// </summary>
	public sealed class Provider : IProvider {
		#region IProvider
		/// <summary>
		/// Get a series.
		/// </summary>
		/// <param name="UniqueIdentifier">The unique identifier.</param>
		public ISeries Get(string UniqueIdentifier) {
			// Check if the unique identifier can be handled.
			if (Regex.Match(UniqueIdentifier, @"^http://kissmanga\.com/Manga/(.*)$", RegexOptions.IgnoreCase).Success) {
				// Initialize a new instance of the Series class.
				return new Series(UniqueIdentifier);
			}
			// Return null.
			return null;
		}
		#endregion
	}
}