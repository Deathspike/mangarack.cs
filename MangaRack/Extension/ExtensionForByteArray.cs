// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System.Drawing;
using System.IO;
using System.Linq;

namespace MangaRack {
	/// <summary>
	/// Represents the class providing extensions for each byte array.
	/// </summary>
	static class ExtensionForByteArray {
		/// <summary>
		/// Contains the Bitmap (BMP) header.
		/// </summary>
		private static byte[] _BMP = new byte[] { 66, 77 };

		/// <summary>
		/// Contains the Graphics Interchange Format (GIF) header.
		/// </summary>
		private static byte[] _GIF = new byte[] { 71, 73, 70 };
		
		/// <summary>
		/// Contains the Joint Photographic Experts Group (JPEG) header.
		/// </summary>
		private static byte[] _JPG = new byte[] { 255, 216 };

		/// <summary>
		/// Contains the W3C Portable Network Graphics (PNG) header.
		/// </summary>
		private static byte[] _PNG = new byte[] { 137, 80, 78, 71 };

		#region Methods
		/// <summary>
		/// Detect an image format from the header.
		/// </summary>
		/// <param name="Buffer">Each byte.</param>
		public static string DetectImageFormat(this byte[] Buffer) {
			// Bitmap (BMP)
			if (_BMP.SequenceEqual(Buffer.Take(_BMP.Length))) {
				// Return the format.
				return "bmp";
			}
			// Graphics Interchange Format (GIF).
			if (_GIF.SequenceEqual(Buffer.Take(_GIF.Length))) {
				// Return the format.
				return "gif";
			}
			// Joint Photographic Experts Group (JPEG).
			if (_JPG.SequenceEqual(Buffer.Take(_JPG.Length))) {
				// Return the format.
				return "jpg";
			}
			// W3C Portable Network Graphics (JPEG).
			if (_PNG.SequenceEqual(Buffer.Take(_PNG.Length))) {
				// Return the format.
				return "png";
			}
			// Return null.
			return null;
		}

		/// <summary>
		/// Create an bitmap from the byte array.
		/// </summary>
		/// <param name="Buffer">Each byte.</param>
		public static Bitmap ToBitmap(this byte[] Buffer) {
			// Attempt the following code.
			try {
				// Initialize a new instance of the MemoryStream class.
				using (MemoryStream MemoryStream = new MemoryStream(Buffer)) {
					// Create the bitmap from the strea.
					using (Bitmap Bitmap = Bitmap.FromStream(MemoryStream) as Bitmap) {
						// Create the bitmap clone.
						return new Bitmap(Bitmap);
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