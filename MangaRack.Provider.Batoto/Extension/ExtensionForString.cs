// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================

namespace MangaRack.Provider.Batoto {
	/// <summary>
	/// Represents the class providing extensions for the String class.
	/// </summary>
	public static class ExtensionForString {
		/// <summary>
		/// Convert an alphabetic suffix to a numeric suffix.
		/// </summary>
		/// <param name="Value">The value.</param>
		public static string AlphabeticToNumeric(this string Value) {
			// Retrieve the alphabetic value.
			char Alphabetic = Value.ToLowerInvariant()[Value.Length - 1];
			// Check if the presumed alphabetic value is actually alphabetic.
			if (!char.IsDigit(Alphabetic)) {
				// Remove the alphabetic value.
				Value = Value.Substring(0, Value.Length - 1);
				// Check if the value does not values behind the digit.
				if (Value.IndexOf('.') == -1) {
					// Append the digit separator.
					Value += '.';
				}
				// Convert the alphabetic value to numeric and append it.
				Value += ((int) Alphabetic - 96).ToString();
			}
			// Return the value.
			return Value;
		}
	}
}