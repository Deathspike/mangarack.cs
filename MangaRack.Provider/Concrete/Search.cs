// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System;
using System.Collections.Generic;
using System.Linq;

namespace MangaRack.Provider {
	/// <summary>
	/// Represents a search.
	/// </summary>
	class Search : ISearch {
		/// <summary>
		/// Contains the search.
		/// </summary>
		private ISearch _Search;

		#region Constructor
		/// <summary>
		/// Initialize a new instance of the Search class.
		/// </summary>
		/// <param name="Search">The search.</param>
		public Search(ISearch Search) {
			// Set the search.
			_Search = Search;
		}
		#endregion

		#region IAsync
		/// <summary>
		/// Populate asynchronously.
		/// </summary>
		/// <param name="Done">The callback.</param>
		public void Populate(Action<ISearch> Done) {
			// Populate asynchronously.
			_Search.Populate(() => {
				// Set the results.
				Results = _Search.Results.Select(x => new Series(x) as ISeries).ToList();
				// Invoke the callback.
				Done(this);
			});
		}
		#endregion

		#region IDisposable
		/// <summary>
		/// Dispose of the object.
		/// </summary>
		public void Dispose() {
			// Dispose of the object.
			_Search.Dispose();
		}
		#endregion

		#region ISearch
		/// <summary>
		/// Contains the results.
		/// </summary>
		public IEnumerable<ISeries> Results { get; private set; }
		#endregion
	}
}