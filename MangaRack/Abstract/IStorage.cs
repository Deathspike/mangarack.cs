// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System.Collections.Generic;

namespace MangaRack {
	/// <summary>
	/// Represents a storage.
	/// </summary>
	public interface IStorage : IEnumerable<string> {
		/// <summary>
		/// Add a file name to an unique identifier.
		/// </summary>
		/// <param name="UniqueIdentifier">The unique identifier.</param>
		/// <param name="Name">The name.</param>
		void Add(string UniqueIdentifier, string Name);

		/// <summary>
		/// Indicates whether a file name exists for the unique identifier.
		/// </summary>
		/// <param name="UniqueIdentifier">The unique identifier.</param>
		/// <param name="Name">The name.</param>
		bool Contains(string UniqueIdentifier, string Name);

		/// <summary>
		/// Remove an unique identifier.
		/// </summary>
		/// <param name="UniqueIdentifier">The unique identifier.</param>
		void Remove(string UniqueIdentifier);

		/// <summary>
		/// Remove an unique identifier.
		/// </summary>
		/// <param name="UniqueIdentifier">The unique identifier.</param>
		/// <param name="Name">The name.</param>
		void Remove(string UniqueIdentifier, string Name);

		/// <summary>
		/// Refresh the persistence.
		/// </summary>
		void Refresh();

		/// <summary>
		/// Save the persistence.
		/// </summary>
		void Save();
	}
}