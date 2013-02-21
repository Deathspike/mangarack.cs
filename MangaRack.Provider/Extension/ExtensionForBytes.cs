// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System.Drawing;
using System.IO;

namespace MangaRack.Provider {
	/// <summary>
	/// Represents the class providing extensions for a byte array.
	/// </summary>
	public static class ExtensionForBytes {
		/// <summary>
		/// Create a bitmap clone from the bytes.
		/// </summary>
		/// <param name="Bytes">The bytes.</param>
		public static Bitmap Bitmap(this byte[] Bytes) {
			// Attempt the following code.
			try {
				// Initialize a new instance of the MemoryStream class.
				using (MemoryStream MemoryStream = new MemoryStream(Bytes)) {
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
	}
}