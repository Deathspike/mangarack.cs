// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================

namespace MangaRack.Provider.KissManga {
	/// <summary>
	/// Represents the class providing extensions for the String class.
	/// </summary>
	public static class ExtensionForString {
		/// <summary>
		/// Returns a new string in which all occurrences of a specified string in the current instance are replaced with another specified string.
		/// </summary>
		/// <param name="OldValue">The string to be replaced.</param>
		/// <param name="NewValue">The string to replace all occurrences of OldValue.</param>
		public static string ReplaceWhile(this string Value, string OldValue, string NewValue) {
			// Iterate while the old value is available.
			while (Value.IndexOf(OldValue) != -1) {
				// Replace the old value with the new value.
				Value = Value.Replace(OldValue, NewValue);
			}
			// Return the modified value.
			return Value;
		}
	}
}