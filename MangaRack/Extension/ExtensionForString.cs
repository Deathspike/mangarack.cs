// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System.IO;
using System.Linq;

namespace MangaRack {
	/// <summary>
	/// Represents the class providing extensions for the String class.
	/// </summary>
	public static class ExtensionForString {
		/// <summary>
		/// Remove invalid characters for a path.
		/// </summary>
		/// <param name="String">The path.</param>
		public static string PathInvalidate(this string String) {
			// Remove invalid characters for a path.
			return string.IsNullOrEmpty(String) ? String : string.Join(null, String.Where(x => !Path.GetInvalidFileNameChars().Contains(x)).Select(x => x.ToString()).ToArray());
		}
	}
}