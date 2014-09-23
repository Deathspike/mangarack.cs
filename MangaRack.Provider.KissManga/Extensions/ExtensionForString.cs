// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================

namespace MangaRack.Provider.KissManga.Extensions {
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

		/// <summary>
		/// Remove characters before and including the first occurrence of the subject.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="subject">The subject.</param>
		public static string RemoveToIncluding(this string value, string subject) {
			// Declare the index.
			int index;
			// Remove characters before and including the first occurrence of the subject.
			return (index = value.IndexOf(subject)) != -1 ? value.Substring(index + subject.Length) : value;
		}

		/// <summary>
		/// Returns a new string in which all occurrences of a specified string in the current instance are replaced with another specified string.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="oldValue">The string to be replaced.</param>
		/// <param name="newValue">The string to replace all occurrences of OldValue.</param>
		public static string ReplaceWhileWithDigit(this string value, string oldValue, string newValue) {
			// Declare the index.
			int index;
			// Iterate while the old value is available.
			while ((index = value.IndexOf(oldValue)) != -1 && (index + oldValue.Length > value.Length || char.IsDigit(value[index + oldValue.Length]))) {
				// Replace the old value with the new value.
				value = value.Replace(oldValue, newValue);
			}
			// Return the modified value.
			return value;
		}
		#endregion
	}
}