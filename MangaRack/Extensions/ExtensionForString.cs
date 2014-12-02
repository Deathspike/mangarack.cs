// ======================================================================
using System.IO;
using System.Linq;

namespace MangaRack.Extensions {
	/// <summary>
	/// Represents the class providing extensions for the String class.
	/// </summary>
	static class ExtensionForString {
		#region Methods
		/// <summary>
		/// Remove invalid path characters.
		/// </summary>
		/// <param name="localPath">The local path.</param>
		public static string InvalidatePath(this string localPath) {
			return string.IsNullOrEmpty(localPath) ? localPath : string.Join(null, localPath.Where(x => !Path.GetInvalidFileNameChars().Contains(x)).Select(x => x.ToString()).ToArray());
		}
		#endregion
	}
}