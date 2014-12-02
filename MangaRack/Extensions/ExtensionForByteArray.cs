// ======================================================================
using System.Drawing;
using System.IO;
using System.Linq;

namespace MangaRack.Extensions {
	/// <summary>
	/// Represents the class providing extensions for each byte array.
	/// </summary>
	public static class ExtensionForByteArray {
		/// <summary>
		/// Contains the Bitmap (BMP) header.
		/// </summary>
		private static byte[] _bmp = new byte[] { 66, 77 };

		/// <summary>
		/// Contains the Graphics Interchange Format (GIF) header.
		/// </summary>
		private static byte[] _gif = new byte[] { 71, 73, 70 };
		
		/// <summary>
		/// Contains the Joint Photographic Experts Group (JPEG) header.
		/// </summary>
		private static byte[] _jpg = new byte[] { 255, 216 };

		/// <summary>
		/// Contains the W3C Portable Network Graphics (PNG) header.
		/// </summary>
		private static byte[] _png = new byte[] { 137, 80, 78, 71 };

		#region Methods
		/// <summary>
		/// Detect an image format from the header.
		/// </summary>
		/// <param name="buffer">Each byte.</param>
		public static string DetectImageFormat(this byte[] buffer) {
			if (_bmp.SequenceEqual(buffer.Take(_bmp.Length))) {
				return "bmp";
			}
			if (_gif.SequenceEqual(buffer.Take(_gif.Length))) {
				return "gif";
			}
			if (_jpg.SequenceEqual(buffer.Take(_jpg.Length))) {
				return "jpg";
			}
			if (_png.SequenceEqual(buffer.Take(_png.Length))) {
				return "png";
			}
			return null;
		}

		/// <summary>
		/// Create an bitmap from the byte array.
		/// </summary>
		/// <param name="buffer">Each byte.</param>
		public static Bitmap ToBitmap(this byte[] buffer) {
			try {
				using (var memoryStream = new MemoryStream(buffer)) {
					using (var bitmap = Bitmap.FromStream(memoryStream) as Bitmap) {
						return new Bitmap(bitmap);
					}
				}
			} catch {
				return null;
			}
		}
		#endregion
	}
}