// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
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
			// Bitmap (BMP)
			if (_bmp.SequenceEqual(buffer.Take(_bmp.Length))) {
				// Return the format.
				return "bmp";
			}
			// Graphics Interchange Format (GIF).
			if (_gif.SequenceEqual(buffer.Take(_gif.Length))) {
				// Return the format.
				return "gif";
			}
			// Joint Photographic Experts Group (JPEG).
			if (_jpg.SequenceEqual(buffer.Take(_jpg.Length))) {
				// Return the format.
				return "jpg";
			}
			// W3C Portable Network Graphics (JPEG).
			if (_png.SequenceEqual(buffer.Take(_png.Length))) {
				// Return the format.
				return "png";
			}
			// Return null.
			return null;
		}

		/// <summary>
		/// Create an bitmap from the byte array.
		/// </summary>
		/// <param name="buffer">Each byte.</param>
		public static Bitmap ToBitmap(this byte[] buffer) {
			// Attempt the following code.
			try {
				// Initialize a new instance of the MemoryStream class.
				using (var memoryStream = new MemoryStream(buffer)) {
					// Create the bitmap from the strea.
					using (var bitmap = Bitmap.FromStream(memoryStream) as Bitmap) {
						// Create the bitmap clone.
						return new Bitmap(bitmap);
					}
				}
			} catch {
				// Return null.
				return null;
			}
		}
		#endregion
	}
}