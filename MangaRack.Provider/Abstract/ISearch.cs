// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System.Collections.Generic;

namespace MangaRack.Provider {
	/// <summary>
	/// Represents a search.
	/// </summary>
	public interface ISearch : IAsync<ISearch> {
		#region Properties
		/// <summary>
		/// Contains each child.
		/// </summary>
		IEnumerable<ISeries> Children { get; }

		/// <summary>
		/// Contains the input.
		/// </summary>
		string Input { get; }
		#endregion
	}
}