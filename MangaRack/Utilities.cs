// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using ICSharpCode.SharpZipLib.Zip;
using MangaRack.Provider;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Xml;

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
		public static bool Write(IList<Meta> MetaInformation, ZipOutputStream ZipOutputStream, byte[] Image) {
			// Create a bitmap from the byte array.
			using (Bitmap Bitmap = Image.ToBitmap()) {
				// Check if the bitmap is valid.
				if (Bitmap != null) {
					// Initialize the file name.
					string FileName = string.Format("{0}.{1}", MetaInformation.Count.ToString("000"), Image.DetectImageFormat());
					// Write a file for the bitmap.
					ZipOutputStream.PutNextEntry(new ZipEntry(FileName));
					// Write the image.
					ZipOutputStream.Write(Image, 0, Image.Length);
					// Close the file entry.
					ZipOutputStream.CloseEntry();
					// Add meta-information ...
					MetaInformation.Add(new Meta {
						// ... with the image height ...
						Height = Bitmap.Height,
						// ... with the file name ...
						FileName = FileName,
						// ... with the file size ...
						Size = Image.Length,
						// ... with the image width.
						Width = Bitmap.Width
					});
					// Return true.
					return true;
				}
				// Return false.
				return false;
			}
		}

		/// <summary>
		/// Write the meta-information to the stream.
		/// </summary>
		/// <param name="MetaInformation">The meta-information.</param>
		/// <param name="ZipOutputStream">The output stream.</param>
		/// <param name="Series">The series.</param>
		/// <param name="Chapter">The chapter.</param>
		public static void Write(IList<Meta> MetaInformation, ZipOutputStream ZipOutputStream, ISeries Series, IChapter Chapter) {
			// Initialize a new instance of the MemoryStream class.
			using (MemoryStream MemoryStream = new MemoryStream()) {
				// Initialize the xml writer to write meta-information.
				using (XmlWriter XmlWriter = XmlWriter.Create(MemoryStream, new XmlWriterSettings { Indent = true })) {
					// ==================================================
					// Basic Information
					// --------------------------------------------------
					XmlWriter.WriteStartElement("ComicInfo");
					// Write the ComicInfo/xmlns:xsd attribute.
					XmlWriter.WriteAttributeString("xmlns", "xsd", null, "http://www.w3.org/2001/XMLSchema");
					// Write the ComicInfo/xmlns:xsi attribute.
					XmlWriter.WriteAttributeString("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");
					// Write the ComicInfo/Manga element.
					XmlWriter.WriteElementString("Manga", "YesAndRightToLeft");
					// Write the ComicInfo/Number element.
					XmlWriter.WriteElementString("Number", Chapter.Number.ToString("0.####"));
					// Check if the volume is valid.
					if (Chapter.Volume >= 0) {
						// Write the ComicInfo/Volume element.
						XmlWriter.WriteElementString("Volume", Chapter.Volume.ToString("0"));
					}

					// ==================================================
					// Additional Information
					// --------------------------------------------------
					if (Series.Genres.Count() != 0) {
						// Write the ComicInfo/Genre element.
						XmlWriter.WriteElementString("Genre", string.Join(", ", Series.Genres.ToArray()));
					}
					// Check if a artist is available.
					if (Series.Artists.Count() != 0) {
						// Write the ComicInfo/Penciller element.
						XmlWriter.WriteElementString("Penciller", string.Join(", ", Series.Artists.ToArray()));
					}
					// Check if the title of the series is available.
					if (!string.IsNullOrEmpty(Series.Title)) {
						// Write the ComicInfo/Series element.
						XmlWriter.WriteElementString("Series", Series.Title.Trim());
					}
					// Check if the summary is available.
					if (!string.IsNullOrEmpty(Series.Summary)) {
						// Write the ComicInfo/Summary element.
						XmlWriter.WriteElementString("Summary", Series.Summary.Trim());
					}
					// Check if the title of the chapter is available.
					if (!string.IsNullOrEmpty(Chapter.Title)) {
						// Write the ComicInfo/Title element.
						XmlWriter.WriteElementString("Title", Chapter.Title.Trim());
					}
					// Check if a author is available.
					if (Series.Authors.Count() != 0) {
						// Write the ComicInfo/Writer element.
						XmlWriter.WriteElementString("Writer", string.Join(", ", Series.Authors.ToArray()));
					}

					// ==================================================
					// Page Information
					// --------------------------------------------------
					// Start writing the ComicInfo/Pages element.
					XmlWriter.WriteStartElement("Pages");
					// Iterate through each page meta information.
					for (int i = 0; i < MetaInformation.Count; i++) {
						// Initialize the meta-information entry.
						Meta MetaInformationEntry = MetaInformation[i];
						// Write the Page element for the Page.
						XmlWriter.WriteStartElement("Page");
						// Write the Image attribute for the Page.
						XmlWriter.WriteAttributeString("Image", i.ToString());
						// Write the ImageSize attribute for the Page.
						XmlWriter.WriteAttributeString("ImageSize", MetaInformationEntry.Size.ToString());
						// Write the ImageWidth attribute for the Page.
						XmlWriter.WriteAttributeString("ImageWidth", MetaInformationEntry.Width.ToString());
						// Write the ImageHeight attribute for the Page.
						XmlWriter.WriteAttributeString("ImageHeight", MetaInformationEntry.Height.ToString());
						// Check if the bitmap is a preview image.
						if (!string.IsNullOrWhiteSpace(MetaInformationEntry.Type)) {
							// Write the Type attribute for the Page.
							XmlWriter.WriteAttributeString("Type", MetaInformationEntry.Type);
						}
						// Close the Page element.
						XmlWriter.WriteEndElement();
					}
					// Stop writing the ComicInfo/Pages element.
					XmlWriter.WriteEndElement();
					// Write the ComicInfo/PageCount element.
					XmlWriter.WriteElementString("PageCount", MetaInformation.Count.ToString());
					// Stop writing the ComicInfo element.
					XmlWriter.WriteEndElement();
				}
				// ==================================================
				// Stream Output
				// --------------------------------------------------
				ZipOutputStream.PutNextEntry(new ZipEntry("ComicInfo.xml"));
				// Set the position within the memory stream.
				MemoryStream.Position = 0;
				// Read each byte from the stream and add each to the output stream.
				MemoryStream.CopyTo(ZipOutputStream);
				// Close the file entry.
				ZipOutputStream.CloseEntry();
			}
		}
		#endregion
	}
}