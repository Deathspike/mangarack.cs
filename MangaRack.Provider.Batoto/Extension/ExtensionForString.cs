// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================

namespace MangaRack.Provider.Batoto {
	/// <summary>
	/// Represents the class providing extensions for the String class.
	/// </summary>
	static class ExtensionForString {
		#region Methods
		/// <summary>
		/// Convert an alphabetic suffix to a numeric suffix.
		/// </summary>
		/// <param name="value">The value.</param>
		public static string AlphabeticToNumeric(this string value) {
			// Retrieve the alphabetic value.
			var alphabetic = value.ToLowerInvariant()[value.Length - 1];
			// Check if the presumed alphabetic value is actually alphabetic.
			if (!char.IsDigit(alphabetic)) {
				// Remove the alphabetic value.
				value = value.Substring(0, value.Length - 1);
				// Check if the value does not values behind the digit.
				if (value.IndexOf('.') == -1) {
					// Append the digit separator.
					value += '.';
				}
				// Convert the alphabetic value to numeric and append it.
				value += ((int) alphabetic - 96).ToString();
			}
			// Return the value.
			return value;
		}
		#endregion
	}
}