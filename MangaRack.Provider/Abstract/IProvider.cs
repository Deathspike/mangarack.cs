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
		/// <summary>
		/// Get a series.
		/// </summary>
		/// <param name="UniqueIdentifier">The unique identifier.</param>
		ISeries Get(string UniqueIdentifier);
	}
}