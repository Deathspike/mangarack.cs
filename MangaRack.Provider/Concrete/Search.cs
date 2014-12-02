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
	sealed class Search : ISearch {
		/// <summary>
		/// Contains the search.
		/// </summary>
		private ISearch _search;

		#region Constructor
		/// <summary>
		/// Initialize a new instance of the Search class.
		/// </summary>
		/// <param name="search">The search.</param>
		public Search(ISearch search) {
			// Set the search.
			_search = search;
		}
		#endregion

		#region IAsync
		/// <summary>
		/// Populate asynchronously.
		/// </summary>
		/// <param name="done">The callback.</param>
		public void Populate(Action<ISearch> done) {
			// Populate asynchronously.
			_search.Populate(() => {
				// Set each child.
				Children = _search.Children.Select(x => new Series(x) as ISeries).ToArray();
				// Invoke the callback.
				done(this);
			});
		}
		#endregion

		#region IDisposable
		/// <summary>
		/// Dispose of the object.
		/// </summary>
		public void Dispose() {
			// Dispose of the object.
			_search.Dispose();
		}
		#endregion

		#region ISearch
		/// <summary>
		/// Contains each child.
		/// </summary>
		public IEnumerable<ISeries> Children { get; private set; }

		/// <summary>
		/// Contains the input.
		/// </summary>
		public string Input {
			get {
				// Return the input.
				return _search.Input;
			}
		}
		#endregion
	}
}