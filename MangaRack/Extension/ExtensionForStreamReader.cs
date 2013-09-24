// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System.Collections.Generic;
using System.IO;

namespace MangaRack {
	/// <summary>
	/// Represents the class providing extensions for the ExtensionForStreamReader class.
	/// </summary>
	static class ExtensionForStreamReader {
		#region Methods
		/// <summary>
		/// Read all lines from the current stream.
		/// </summary>
		/// <param name="StreamReader">The stream reader.</param>
		public static IEnumerable<string> ReadLines(this StreamReader StreamReader) {
			// Initialize the line.
			string Line;
			// Continue iterating while a line is available.
			while ((Line = StreamReader.ReadLine()) != null) {
				// Yield return the line.
				yield return Line;
			}
		}
		#endregion
	}
}