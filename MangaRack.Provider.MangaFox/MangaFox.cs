// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================

namespace MangaRack.Provider.MangaFox {
	/// <summary>
	/// Represents a MangaFox provider.
	/// </summary>
	public sealed class MangaFox : IProvider {
		/// <summary>
		/// Contains the provider.
		/// </summary>
		private readonly IProvider _Provider;

		#region Constructor
		/// <summary>
		/// Initialize a new instance of the MangaFox class.
		/// </summary>
		private MangaFox() {
			// Set the provider.
			_Provider = new Provider();
		}
		#endregion

		#region IProvider:Methods
		/// <summary>
		/// Open a series.
		/// </summary>
		/// <param name="UniqueIdentifier">The unique identifier.</param>
		public ISeries Open(string UniqueIdentifier) {
			// Open a series.
			return _Provider.Open(UniqueIdentifier);
		}

		/// <summary>
		/// Search series.
		/// </summary>
		/// <param name="Input">The input.</param>
		public ISearch Search(string Input) {
			// Search series.
			return _Provider.Search(Input);
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