// ======================================================================

namespace MangaRack.Provider.Batoto.Extensions {
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
			var alphabetic = value.ToLowerInvariant()[value.Length - 1];
			if (!char.IsDigit(alphabetic)) {
				value = value.Substring(0, value.Length - 1);
				if (value.IndexOf('.') == -1) {
					value += '.';
				}
				value += ((int) alphabetic - 96).ToString();
			}
			return value;
		}
		#endregion
	}
}