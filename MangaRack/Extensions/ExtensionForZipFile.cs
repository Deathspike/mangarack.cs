// ======================================================================
using ICSharpCode.SharpZipLib.Zip;

namespace MangaRack.Extensions {
	/// <summary>
	/// Represents the class providing extensions for the ZipFile class.
	/// </summary>
	static class ExtensionForZipFile {
		#region Methods
		/// <summary>
		/// Attempt to delete files matching the file name and extension.
		/// </summary>
		/// <param name="zipFile">The zip file.</param>
		/// <param name="fileName">The file name.</param>
		/// <param name="extensions">Each extension.</param>
		public static void TryDelete(this ZipFile zipFile, string fileName, params string[] extensions) {
			foreach (var extension in extensions) {
				var zipEntry = zipFile.GetEntry(string.Format("{0}.{1}", fileName, extension));
				if (zipEntry != null) {
					zipFile.Delete(zipEntry);
				}
			}
		}
		#endregion
	}
}