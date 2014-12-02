// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System;

namespace MangaRack.Provider.KissManga.Extension {
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

        /// <summary>
        /// Remove characters before and including the first occurrence of the subject.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="subject">The subject.</param>
        public static string RemoveToIncluding(this string value, string subject) {
            int index;
            return (index = value.IndexOf(subject, StringComparison.Ordinal)) != -1 ? value.Substring(index + subject.Length) : value;
        }

        /// <summary>
        /// Returns a new string in which all occurrences of a specified string in the current instance are replaced with another specified string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="oldValue">The string to be replaced.</param>
        /// <param name="newValue">The string to replace all occurrences of OldValue.</param>
        public static string ReplaceWhileWithDigit(this string value, string oldValue, string newValue) {
            int index;
            while ((index = value.IndexOf(oldValue, StringComparison.Ordinal)) != -1 && (index + oldValue.Length > value.Length || char.IsDigit(value[index + oldValue.Length]))) {
                value = value.Replace(oldValue, newValue);
            }
            return value;
        }
        #endregion
    }
}