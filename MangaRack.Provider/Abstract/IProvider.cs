// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================

namespace MangaRack.Provider {
	/// <summary>
	/// Represents a provider.
	/// </summary>
	public interface IProvider {
		#region Methods
		/// <summary>
		/// Open a series.
		/// </summary>
		/// <param name="uniqueIdentifier">The unique identifier.</param>
		ISeries Open(string uniqueIdentifier);

		/// <summary>
		/// Search series.
		/// </summary>
		/// <param name="input">The input.</param>
		ISearch Search(string input);
		#endregion

		#region Properties
		/// <summary>
		/// Contains the unique identifier.
		/// </summary>
		string UniqueIdentifier { get; }
		#endregion
	}
}