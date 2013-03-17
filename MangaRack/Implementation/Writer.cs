// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using ICSharpCode.SharpZipLib.Zip;
using MangaRack.Provider;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Xml;

namespace MangaRack {
	/// <summary>
	/// Represents a writer.
	/// </summary>
	public sealed class Writer : IWriter {
		/// <summary>
		/// Contains the number of pages.
		/// </summary>
		private int _NumberOfPages;

		/// <summary>
		/// Contains the file path.
		/// </summary>
		private string _FilePath;

		/// <summary>
		/// Contains the file stream.
		/// </summary>
		private FileStream _FileStream;

		/// <summary>
		/// Contains the memory stream.
		/// </summary>
		private MemoryStream _MemoryStream;

		/// <summary>
		/// Contains the temporary path.
		/// </summary>
		private string _TemporaryPath;

		/// <summary>
		/// Contains the xml writer.
		/// </summary>
		private XmlWriter _XmlWriter;

		/// <summary>
		/// Contains the archive stream.
		/// </summary>
		private ZipOutputStream _ZipOutputStream;

		#region Constructor
		/// <summary>
		/// Initialize a new instance of the Writer class.
		/// </summary>
		/// <param name="Series">The series name.</param>
		/// <param name="Name">The (file) name.</param>
		public Writer(string Series, string Name) {
			// Initialize the file path.
			_FilePath = Path.Combine(Series.PathInvalidate(), string.Format("{0} {1}.cbz", Series, Name).PathInvalidate());
			// Check if the file already exists.
			if (File.Exists(_FilePath)) {
				// Return.
				return;
			} else {
				// Initialize the temporary path.
				_TemporaryPath = Path.GetTempFileName();
				// Initialize the file stream.
				_FileStream = File.Create(_TemporaryPath);
				// Initialize the memory stream.
				_MemoryStream = new MemoryStream();
				// Initialize  the xml writer.
				_XmlWriter = XmlWriter.Create(_MemoryStream, new XmlWriterSettings { Indent = true });
				// Initialize the archive stream.
				_ZipOutputStream = new ZipOutputStream(_FileStream);
			}
		}
		#endregion

		#region Method
		/// <summary>
		/// Write a bitmap.
		/// </summary>
		/// <param name="Bitmap">The bitmap.</param>
		/// <param name="IsPreview">Indicates whether this is a preview image.</param>
		private void Write(Bitmap Bitmap, bool IsPreview) {
			// Check if the bitmap is available.
			if (Bitmap != null) {
				// Initialize the bytes.
				byte[] Bytes;
				// Initialize a new instance of the MemoryStream class.
				using (MemoryStream MemoryStream = new MemoryStream()) {
					// Save the bitmap to the memory stream.
					Bitmap.Save(MemoryStream, ImageFormat.Jpeg);
					// Retrieve the bytes for the bitmap.
					Bytes = MemoryStream.ToArray();
				}
				// Write information.
				if (true) {
					// Write the Page element for the Page.
					_XmlWriter.WriteStartElement("Page");
					// Write the Image attribute for the Page.
					_XmlWriter.WriteAttributeString("Image", _NumberOfPages.ToString());
					// Write the ImageSize attribute for the Page.
					_XmlWriter.WriteAttributeString("ImageSize", Bytes.Length.ToString());
					// Write the ImageWidth attribute for the Page.
					_XmlWriter.WriteAttributeString("ImageWidth", Bitmap.Width.ToString());
					// Write the ImageHeight attribute for the Page.
					_XmlWriter.WriteAttributeString("ImageHeight", Bitmap.Height.ToString());
					// Check if the bitmap is a preview image.
					if (IsPreview) {
						// Write the Type attribute for the Page.
						_XmlWriter.WriteAttributeString("Type", "Preview");
					}
					// Close the Page element.
					_XmlWriter.WriteEndElement();
				}
				// Write the image to the archive.
				if (true) {
					// Write a file for the bitmap.
					_ZipOutputStream.PutNextEntry(new ZipEntry(string.Format("{0}.jpg", _NumberOfPages.ToString("000"))));
					// Write the image.
					_ZipOutputStream.Write(Bytes, 0, Bytes.Length);
					// Close the file entry.
					_ZipOutputStream.CloseEntry();
					// Increment the number of pages.
					_NumberOfPages++;
				}
			}
		}
		#endregion

		#region IDisposable
		/// <summary>
		/// Dispose of the object.
		/// </summary>
		public void Dispose() {
			// Check if the stream is valid.
			if (_FileStream != null) {
				// Dispose of each stream.
				if (true) {
					// Dispose of the object.
					(_XmlWriter as IDisposable).Dispose();
					// Dispose of the object.
					_MemoryStream.Dispose();
					// Dispose of the object.
					_ZipOutputStream.Dispose();
					// Dispose of the object.
					_FileStream.Dispose();
					// Remove the stream.
					_FileStream = null;
				}
				// Check if the directory containing the file does not exist.
				if (!Directory.Exists(Path.GetDirectoryName(_FilePath))) {
					// Create each directory as specified by the path.
					Directory.CreateDirectory(Path.GetDirectoryName(_FilePath));
				}
				// Move the temporary path to the file path.
				File.Move(_TemporaryPath, _FilePath);
			}
		}
		#endregion

		#region IWriter
		/// <summary>
		/// Indicates whether the writer can continue.
		/// </summary>
		public bool CanContinue() {
			// Check if the file stream is valid.
			return _FileStream != null;
		}

		/// <summary>
		/// Write the manga.
		/// </summary>
		/// <param name="Series">The series.</param>
		/// <param name="Chapter">The chapter.</param>
		/// <param name="Volume">The volume.</param>
		public int Write(ISeries Series, IChapter Chapter, double Volume) {
			// Attempt the following code.
			try {
				// Write information.
				if (true) {
					// Start writing the ComicInfo element.
					_XmlWriter.WriteStartElement("ComicInfo");
					// Write basic information.
					if (true) {
						// Write the ComicInfo/xmlns:xsd attribute.
						_XmlWriter.WriteAttributeString("xmlns", "xsd", null, "http://www.w3.org/2001/XMLSchema");
						// Write the ComicInfo/xmlns:xsi attribute.
						_XmlWriter.WriteAttributeString("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");
						// Write the ComicInfo/Manga element.
						_XmlWriter.WriteElementString("Manga", "YesAndRightToLeft");
						// Write the ComicInfo/Number element.
						_XmlWriter.WriteElementString("Number", Chapter.Number.ToString("0.####"));
						// Check if the volume is valid.
						if (Volume >= 0) {
							// Write the ComicInfo/Volume element.
							_XmlWriter.WriteElementString("Volume", Volume.ToString("0"));
						}
					}
					// Write additional information.
					if (true) {
						// Check if a genre is available.
						if (Series.Artists.Count() != 0) {
							// Write the ComicInfo/Genre element.
							_XmlWriter.WriteElementString("Genre", string.Join(", ", Series.Genres.ToArray()));
						}
						// Check if a artist is available.
						if (Series.Artists.Count() != 0) {
							// Write the ComicInfo/Penciller element.
							_XmlWriter.WriteElementString("Penciller", string.Join(", ", Series.Artists.ToArray()));
						}
						// Check if the title of the series is available.
						if (!string.IsNullOrEmpty(Series.Title)) {
							// Write the ComicInfo/Series element.
							_XmlWriter.WriteElementString("Series", Series.Title.Trim());
						}
						// Check if the summary is available.
						if (!string.IsNullOrEmpty(Series.Summary)) {
							// Write the ComicInfo/Summary element.
							_XmlWriter.WriteElementString("Summary", Series.Summary.Trim());
						}
						// Check if the title of the chapter is available.
						if (!string.IsNullOrEmpty(Chapter.Title)) {
							// Write the ComicInfo/Title element.
							_XmlWriter.WriteElementString("Title", Chapter.Title.Trim());
						}
						// Check if a author is available.
						if (Series.Authors.Count() != 0) {
							// Write the ComicInfo/Writer element.
							_XmlWriter.WriteElementString("Writer", string.Join(", ", Series.Authors.ToArray()));
						}
					}
					// Write the pages.
					if (true) {
						// Start writing the ComicInfo/Pages element.
						_XmlWriter.WriteStartElement("Pages");
						// Write the image.
						Write(Series.PreviewImage, true);
						// Iterate through each page.
						foreach (IPage Page in Chapter.Pages) {
							// Write the image.
							Write(Page.Bitmap, false);
							// Dispose of the page.
							Page.Dispose();
						}
						// Stop writing the ComicInfo/Pages element.
						_XmlWriter.WriteEndElement();
						// Write the ComicInfo/PageCount element.
						_XmlWriter.WriteElementString("PageCount", _NumberOfPages.ToString());
					}
					// Stop writing the ComicInfo element.
					_XmlWriter.WriteEndElement();
				}
				// Write the meta information.
				if (true) {
					// Write a file entry for the image.
					_ZipOutputStream.PutNextEntry(new ZipEntry("ComicInfo.xml"));
					// Flush the XmlWriter contents to the stream.
					_XmlWriter.Flush();
					// Set the position within the stream.
					_MemoryStream.Position = 0;
					// Read all the bytes from the stream and copy them to the output stream.
					_MemoryStream.CopyTo(_ZipOutputStream);
					// Close the file entry.
					_ZipOutputStream.CloseEntry();
				}
				// Return the number of pages.
				return _NumberOfPages - (Series.PreviewImage == null ? 0 : 1);
			} catch (Exception e) {
				// Dispose of the object.
				Dispose();
				// Delete the incomplete file.
				File.Delete(_FilePath);
				// Throw the exception.
				throw e;
			}
		}
		#endregion
	}
}