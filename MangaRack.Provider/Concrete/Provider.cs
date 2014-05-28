// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================

namespace MangaRack.Provider {
	/// <summary>
	/// Represents a provider.
	/// </summary>
	sealed class Provider : IProvider {
		/// <summary>
		/// Contains the provider.
		/// </summary>
		private IProvider _provider;

		#region Constructor
		/// <summary>
		/// Initialize a new instance of the Provider class.
		/// </summary>
		/// <param name="provider">The provider.</param>
		public Provider(IProvider provider) {
			// Set the provider.
			_provider = provider;
		}
		#endregion

		#region IProvider:Methods
		/// <summary>
		/// Open a series.
		/// </summary>
		/// <param name="uniqueIdentifier">The unique identifier.</param>
		public ISeries Open(string uniqueIdentifier) {
			// Open a series.
			ISeries Series = _provider.Open(uniqueIdentifier);
			// Return a series or null.
			return Series == null ? null : new Series(Series);
		}

		/// <summary>
		/// Search series.
		/// </summary>
		/// <param name="input">The input.</param>
		public ISearch Search(string input) {
			// Search series.
			ISearch Search = _provider.Search(input);
			// Return a series search or null.
			return Search == null ? null : new Search(Search);
		}
		#endregion

		#region IProvider:Properties
		/// <summary>
		/// Contains the location.
		/// </summary>
		public string Location {
			get {
				// Return the location.
				return _provider.Location;
			}
		}
		#endregion
	}
}