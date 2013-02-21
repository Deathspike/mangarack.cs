// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System.Drawing;
using System.Drawing.Imaging;

namespace MangaRack.Provider {
	/// <summary>
	/// Represents the class providing extensions for the Bitmap class.
	/// </summary>
	public static class ExtensionForBitmap {
		/// <summary>
		/// Frame to the last available frame.
		/// </summary>
		/// <param name="Bitmap">The bitmap.</param>
		public static Bitmap Frame(this Bitmap Bitmap) {
			// Initialize a new instance of the FrameDimension class.
			FrameDimension FrameDimension = new FrameDimension(Bitmap.FrameDimensionsList[0]);
			// Retrieve the number of frames.
			int FrameCount = Bitmap.GetFrameCount(FrameDimension);
			// Check if more than one frame is available.
			if (FrameCount > 1) {
				// Initialize the result.
				Bitmap Result = null;
				// Select the last frame containing the appropriate image.
				Bitmap.SelectActiveFrame(FrameDimension, FrameCount - 1);
				// Create the image for the frame.
				Result = new Bitmap(Bitmap);
				// Dispose of the original image.
				Bitmap.Dispose();
				// Set the bitmap.
				Bitmap = Result;
			}
			// Return the bitmap.
			return Bitmap;
		}
	}
}