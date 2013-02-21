// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace MangaRack {
	/// <summary>
	/// Represents a storage.
	/// </summary>
	public sealed class Storage : IStorage {
		/// <summary>
		/// Contains the path.
		/// </summary>
		private readonly string _Path;

		/// <summary>
		/// Contains the storage.
		/// </summary>
		private Dictionary<string, HashSet<string>> _Storage;

		#region Constructor
		/// <summary>
		/// Initialize a new instance of the Storage class.
		/// </summary>
		/// <param name="Path">The path.</param>
		public Storage(string Path) {
			// Set the path.
			_Path = Path;
			// Refresh the persistence.
			Refresh();
		}
		#endregion

		#region IEnumerable
		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		public IEnumerator<string> GetEnumerator() {
			// Return an enumerator that iterates through the collection.
			return _Storage.Keys.GetEnumerator();
		}

		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		IEnumerator IEnumerable.GetEnumerator() {
			// Return an enumerator that iterates through the collection.
			return (IEnumerator) GetEnumerator();
		}
		#endregion

		#region IStorage
		/// <summary>
		/// Add a file name to an unique identifier.
		/// </summary>
		/// <param name="UniqueIdentifier">The unique identifier.</param>
		/// <param name="Name">The name.</param>
		public void Add(string UniqueIdentifier, string Name) {
			// Check if the unique identifier has not been added.
			if (!_Storage.ContainsKey(UniqueIdentifier)) {
				// Initialize and add a new instance of the HashSet class.
				_Storage.Add(UniqueIdentifier, new HashSet<string>());
			}
			// Check if the file name has not been added.
			if (!_Storage[UniqueIdentifier].Contains(Name)) {
				// Add the file name to the unique identifier.
				_Storage[UniqueIdentifier].Add(Name);
			}
		}

		/// <summary>
		/// Indicates whether a file name exists for the unique identifier.
		/// </summary>
		/// <param name="UniqueIdentifier">The unique identifier.</param>
		/// <param name="Name">The name.</param>
		public bool Contains(string UniqueIdentifier, string Name) {
			// Return the status indicating whether a file name exists for the unique identifier.
			return _Storage.ContainsKey(UniqueIdentifier) && _Storage[UniqueIdentifier].Contains(Name);
		}

		/// <summary>
		/// Remove an unique identifier.
		/// </summary>
		/// <param name="UniqueIdentifier">The unique identifier.</param>
		public void Remove(string UniqueIdentifier) {
			// Check if the unique identifier does exist.
			if (_Storage.ContainsKey(UniqueIdentifier)) {
				// Remove the unique identifier.
				_Storage.Remove(UniqueIdentifier);
			}
		}

		/// <summary>
		/// Remove an unique identifier.
		/// </summary>
		/// <param name="UniqueIdentifier">The unique identifier.</param>
		/// <param name="Name">The name.</param>
		public void Remove(string UniqueIdentifier, string Name) {
			// Check if the file name exists for the unique identifier.
			if (Contains(UniqueIdentifier, Name)) {
				// Remove the file name.
				_Storage[UniqueIdentifier].Remove(Name);
				// Check if the unique identifier has been emptied.
				if (_Storage[UniqueIdentifier].Count == 0) {
					// Remove the unique identifier.
					_Storage.Remove(UniqueIdentifier);
				}
			}
		}

		/// <summary>
		/// Refresh the persistence.
		/// </summary>
		public void Refresh() {
			// Attempt the following code.
			try {
				// Check if the file does exist.
				if (File.Exists(_Path)) {
					// Read the file and deserialize the storage.
					_Storage = JsonConvert.DeserializeObject<Dictionary<string, HashSet<string>>>(File.ReadAllText(_Path));
				}
			} finally {
				// Check if the storage is invalid.
				if (_Storage == null) {
					// Initialize a new instance of the Dictionary class.
					_Storage = new Dictionary<string, HashSet<string>>();
				}
			}
		}

		/// <summary>
		/// Save the persistence.
		/// </summary>
		public void Save() {
			// Serialize the storage and write the file.
			File.WriteAllText(_Path, JsonConvert.SerializeObject(_Storage, Formatting.Indented));
		}
		#endregion
	}
}