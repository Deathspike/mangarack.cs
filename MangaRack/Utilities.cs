// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using ICSharpCode.SharpZipLib.Zip;
using MangaRack.Core;
using MangaRack.Provider;
using System.Collections.Generic;
using System.Drawing;

namespace MangaRack {
	/// <summary>
	/// Represents the collection of utilities.
	/// </summary>
	static class Utilities {
		#region Methods
		/// <summary>
		/// Process the image according to the collection of options.
		/// </summary>
		/// <param name="Options">The collection of options.</param>
		/// <param name="Provider">The provider.</param>
		/// <param name="Image">The image.</param>
		public static byte[] Image(Options Options, IProvider Provider, byte[] Image) {
			// Create a bitmap from the byte array.
			Bitmap Bitmap = Image.ToBitmap();
			// Attempt the following code.
			try {
				// Check if the bitmap is valid.
				if (Bitmap != null) {
					// Indicating whether saving is required.
					bool RequiresSave = false;
					// Check if animation framing is not disabled.
					if (!Options.DisableAnimationFraming) {
						// Frame to the last available frame.
						Bitmap Result = Bitmap.Frame();
						// Check if the result is modified.
						if (Bitmap != Result) {
							// Set the bitmap.
							Bitmap = Result;
							// Set the required save state.
							RequiresSave = true;
						}
					}
					// Check if footer incision is not disabled.
					if (!Options.DisableFooterIncision && string.Equals(Provider.UniqueIdentifier, "MangaFox")) {
						// Crop the image to remove a textual addition.
						Bitmap Result = Bitmap.Crop();
						// Check if the result is modified.
						if (Bitmap != Result) {
							// Set the bitmap.
							Bitmap = Result;
							// Set the required save state.
							RequiresSave = true;
						}
					}
					// Check if image processing is not disabled.
					if (!Options.DisableImageProcessing) {
						// Linear correction in RGB colour space.
						Bitmap = Bitmap.Colour();
						// Adjust contrast in RGB colour space.
						Bitmap = Bitmap.Contrast();
						// Sharpen using a gaussian sharpen filter.
						Bitmap = Bitmap.Sharpen();
						// Set the required save state.
						RequiresSave = true;
					}
					// Check if saving is required.
					if (RequiresSave) {
						// Convert RGB colour space to grayscale when applicable.
						Bitmap = !Options.DisableGrayscaleSizeComparisonAndSave ? Bitmap.Grayscale() : Bitmap;
						// Create a byte array from the bitmapy.
						Image = Bitmap.ToByteArray(Image.DetectImageFormat());
					} else if (!Options.DisableGrayscaleSizeComparisonAndSave && Image.DetectImageFormat().Equals("png")) {
						// Create a byte array from the bitmapy.
						byte[] CompareImage = (Bitmap = Bitmap.Grayscale()).ToByteArray(Image.DetectImageFormat());
						// Check if the image to compare has a smaller file size.
						if (CompareImage.Length < Image.Length) {
							// Set the image.
							Image = CompareImage;
						}
					}
				}
				// Return the image.
				return Image;
			} finally {
				// Check if the bitmap is valid.
				if (Bitmap != null) {
					// Dispose of the bitmap.
					Bitmap.Dispose();
				}
			}
		}

		/// <summary>
		/// Write the image to the stream and add meta-information to the collection.
		/// </summary>
		/// <param name="MetaInformation">The meta-information.</param>
		/// <param name="ZipOutputStream">The output stream.</param>
		/// <param name="Image">The image.</param>
		public static bool Write(IList<ComicInfoPage> MetaInformation, ZipOutputStream ZipOutputStream, byte[] Image) {
			// Create a bitmap from the byte array.
			using (Bitmap Bitmap = Image.ToBitmap()) {
				// Initialize the image height.
				int? ImageHeight = null;
				// Initialize the image width.
				int? ImageWidth = null;
				// Initialize the page valid status.
				bool IsValid = true;
				// Check if the bitmap is invalid.
				if (Bitmap == null) {
					// Set the page valid status to false.
					IsValid = false;
					// Initialize a new instance of the Bitmap class.
					using (Bitmap BrokenBitmap = new Bitmap(128, 32)) {
						// Initialize a new instance of the Graphics class.
						using (Graphics Graphics = Graphics.FromImage(BrokenBitmap)) {
							// Initialize a new instance of the RectangleF class.
							RectangleF RectangleF = new RectangleF(0, 0, BrokenBitmap.Width, BrokenBitmap.Height);
							// Initialize a new instance of the Font class.
							Font Font = new Font("Tahoma", 10);
							// Initialize a new instance of the StringFormat class.
							StringFormat StringFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
							// Fill the rectangle with a white brush.
							Graphics.FillRectangle(Brushes.White, RectangleF);
							// Write the broken page information with a black brush.
							Graphics.DrawString(string.Format("Broken page #{0}", MetaInformation.Count.ToString("000")), Font, Brushes.Black, RectangleF, StringFormat);
						}
						// Set the image.
						Image = BrokenBitmap.ToByteArray("png");
						// Set the image height.
						ImageHeight = BrokenBitmap.Height;
						// Set the image width.
						ImageWidth = BrokenBitmap.Width;
					}
				}
				// Save the image.
				if (true) {
					// Initialize the file name.
					string Key = string.Format("{0}.{1}", MetaInformation.Count.ToString("000"), Image.DetectImageFormat());
					// Write a file for the bitmap.
					ZipOutputStream.PutNextEntry(new ZipEntry(Key));
					// Write the image.
					ZipOutputStream.Write(Image, 0, Image.Length);
					// Close the file entry.
					ZipOutputStream.CloseEntry();
					// Add meta-information ...
					MetaInformation.Add(new ComicInfoPage {
						// ... with the image height ...
						ImageHeight = ImageHeight ?? Bitmap.Height,
						// ... with the key ...
						Key = Key,
						// ... with the image size ...
						ImageSize = Image.Length,
						// ... with the image width.
						ImageWidth = ImageWidth ?? Bitmap.Width
					});
				}
				// Return whether the page is valid.
				return IsValid;
			}
		}

		/// <summary>
		/// Write the meta-information to the stream.
		/// </summary>
		/// <param name="ZipOutputStream">The output stream.</param>
		/// <param name="Series">The series.</param>
		/// <param name="Chapter">The chapter.</param>
		/// <param name="Pages">Each page.</param>
		public static void Write(ZipOutputStream ZipOutputStream, ISeries Series, IChapter Chapter, IEnumerable<ComicInfoPage> Pages) {
			// Initialize a new instance of the ComicInfo class.
			ComicInfo ComicInfo = new ComicInfo();
			//  Write a file entry for the comic information.
			ZipOutputStream.PutNextEntry(new ZipEntry("ComicInfo.xml"));
			// Transcribe the series, chapter and pages information.
			ComicInfo.Transcribe(Series, Chapter, Pages);
			// Save the comic information.
			ComicInfo.Save(ZipOutputStream);
			// Close the file entry.
			ZipOutputStream.CloseEntry();
		}
		#endregion
	}
}