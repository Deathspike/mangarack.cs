// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
namespace MangaRack.Provider.Batoto.Extension {
    /// <summary>
    /// Represents the class providing extensions for the String class.
    /// </summary>
    internal static class ExtensionForString {
        #region Methods
        /// <summary>
        /// Convert an alphabetic suffix to a numeric suffix.
        /// </summary>
        /// <param name="value">The value.</param>
        public static string AlphabeticToNumeric(this string value) {
            var alphabetic = value.ToLowerInvariant()[value.Length - 1];
            if (char.IsDigit(alphabetic)) return value;
            value = value.Substring(0, value.Length - 1);
            if (value.IndexOf('.') == -1) {
                value += '.';
            }
            value += (alphabetic - 96).ToString();
            return value;
        }
        #endregion
    }
}