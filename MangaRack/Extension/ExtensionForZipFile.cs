// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using ICSharpCode.SharpZipLib.Zip;

namespace MangaRack {
	/// <summary>
	/// Represents the class providing extensions for the ZipFile class.
	/// </summary>
	static class ExtensionForZipFile {
		#region Methods
		/// <summary>
		/// Attempt to delete files matching the file name and extension.
		/// </summary>
		/// <param name="ZipFile">The zip file.</param>
		/// <param name="FileName">The file name.</param>
		/// <param name="Extensions">Each extension.</param>
		public static void TryDelete(this ZipFile ZipFile, string FileName, params string[] Extensions) {
			// Iterate through each extension.
			foreach (string Extension in Extensions) {
				// Find the zip file entry.
				ZipEntry ZipEntry = ZipFile.GetEntry(string.Format("{0}.{1}", FileName, Extension));
				// Check if the zip file entry is valid.
				if (ZipEntry != null) {
					// Delete the zip file entry.
					ZipFile.Delete(ZipEntry);
				}
			}
		}
		#endregion
	}
}