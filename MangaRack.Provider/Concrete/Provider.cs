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
		private IProvider _Provider;

		#region Constructor
		/// <summary>
		/// Initialize a new instance of the Provider class.
		/// </summary>
		/// <param name="Provider">The provider.</param>
		public Provider(IProvider Provider) {
			// Set the provider.
			_Provider = Provider;
		}
		#endregion

		#region IProvider:Methods
		/// <summary>
		/// Open a series.
		/// </summary>
		/// <param name="UniqueIdentifier">The unique identifier.</param>
		public ISeries Open(string UniqueIdentifier) {
			// Open a series.
			ISeries Series = _Provider.Open(UniqueIdentifier);
			// Return a series or null.
			return Series == null ? null : new Series(Series);
		}

		/// <summary>
		/// Search series.
		/// </summary>
		/// <param name="Input">The input.</param>
		public ISearch Search(string Input) {
			// Search series.
			ISearch Search = _Provider.Search(Input);
			// Return a series search or null.
			return Search == null ? null : new Search(Search);
		}
		#endregion

		#region IProvider:Properties
		/// <summary>
		/// Contains the unique identifier.
		/// </summary>
		public string UniqueIdentifier {
			get {
				// Return the unique identifier.
				return _Provider.UniqueIdentifier;
			}
		}
		#endregion
	}
}