// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System.Linq;
using ICSharpCode.SharpZipLib.Zip;

namespace MangaRack.Extension {
    /// <summary>
    /// Represents the class providing extensions for the ZipFile class.
    /// </summary>
    internal static class ExtensionForZipFile {
        #region Methods
        /// <summary>
        /// Attempt to delete files matching the file name and extension.
        /// </summary>
        /// <param name="zipFile">The zip file.</param>
        /// <param name="fileName">The file name.</param>
        /// <param name="extensions">Each extension.</param>
        public static void TryDelete(this ZipFile zipFile, string fileName, params string[] extensions) {
            foreach (var zipEntry in extensions.Select(extension => zipFile.GetEntry(string.Format("{0}.{1}", fileName, extension))).Where(zipEntry => zipEntry != null)) {
                zipFile.Delete(zipEntry);
            }
        }
        #endregion
    }
}