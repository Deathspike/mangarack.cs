// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using ICSharpCode.SharpZipLib.Zip;
using MangaRack.Core;
using MangaRack.Provider;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace MangaRack {
	/// <summary>
	/// Represents a publisher.
	/// </summary>
	sealed class Publisher : IDisposable, IPublisher {
		/// <summary>
		/// Contains the begin time.
		/// </summary>
		private long _BeginTime;

		/// <summary>
		/// Contains the file path.
		/// </summary>
		private string _FilePath;

		/// <summary>
		/// Contains the file stream.
		/// </summary>
		private FileStream _FileStream;

		/// <summary>
		/// Indicates whether the publisher is repairing.
		/// </summary>
		private bool _IsRepairing;

		/// <summary>
		/// Contains the number of pages.
		/// </summary>
		private int _NumberOfPages;

		/// <summary>
		/// Contains a collection of options.
		/// </summary>
		private Options _Options;

		/// <summary>
		/// Contains the provider.
		/// </summary>
		private IProvider _Provider;

		/// <summary>
		/// Containst the compressed file.
		/// </summary>
		private ZipFile _ZipFile;

		#region Constructor
		/// <summary>
		/// Initialize a new instance of the Publisher class.
		/// </summary>
		/// <param name="FilePath">The file path.</param>
		/// <param name="Options">The collection of options.</param>
		/// <param name="Provider">The provider.</param>
		public Publisher(string FilePath, Options Options, IProvider Provider)
			: this(FilePath, Options, Provider, false) {
			// Stop the function.
			return;
		}

		/// <summary>
		/// Initialize a new instance of the Publisher class.
		/// </summary>
		/// <param name="FilePath">The file path.</param>
		/// <param name="Options">The collection of options.</param>
		/// <param name="Provider">The provider.</param>
		/// <param name="IsRepairing">Indicates whether the publisher is repairing.</param>
		public Publisher(string FilePath, Options Options, IProvider Provider, bool IsRepairing) {
			// Write the message.
			Console.WriteLine("{0} {1}", IsRepairing ? "Checking" : "Fetching", Path.GetFileName(FilePath));
			// Initialize properties.
			if (true) {
				// Set the begin time.
				_BeginTime = DateTime.Now.Ticks;
				// Set the file path.
				_FilePath = FilePath;
				// Set the file stream.
				_FileStream = File.Open(IsRepairing ? FilePath : Path.GetTempFileName(), FileMode.OpenOrCreate);
				// Set whether the publisher is repairing.
				_IsRepairing = IsRepairing;
				// Set the collection of options.
				_Options = Options;
				// Set the provider.
				_Provider = Provider;
				// Set the compressed file.
				_ZipFile = IsRepairing ? new ZipFile(_FileStream) : ZipFile.Create(_FileStream);
			}
		}
		#endregion

		#region Properties
		/// <summary>
		/// Indicates whether there are broken pages.
		/// </summary>
		public bool HasBrokenPages { get; set; }

		/// <summary>
		/// Indicates whether repairing has failed.
		/// </summary>
		public bool HasFailed { get; set; }
		#endregion

		#region IDisposable
		/// <summary>
		/// Dispose of the object.
		/// </summary>
		public void Dispose() {
			// Dispose of the file stream.
			_FileStream.Dispose();
			// Check if the file does exist.
			if (File.Exists(_FileStream.Name)) {
				// Check if the publisher is not repairing.
				if (!_IsRepairing) {
					// Check if the series directory does not exist.
					if (!Directory.Exists(Path.GetDirectoryName(_FilePath))) {
						// Create the directory for the series.
						Directory.CreateDirectory(Path.GetDirectoryName(_FilePath));
					}
					// Move the temporary file to the file path.
					File.Copy(_FileStream.Name, _FilePath, true);
					// Delete the temporary file.
					File.Delete(_FileStream.Name);
				} else if (HasFailed) {
					// Delete the file.
					File.Delete(_FilePath);
				}
				// Check if repairing has failed.
				if (HasFailed) {
					// Write the message.
					Console.WriteLine("{0} {1}", "Squashed", Path.GetFileName(_FilePath));
				} else {
					// Initialize the elapsed time.
					TimeSpan Elapsed = new TimeSpan(DateTime.Now.Ticks - _BeginTime);
					// Write the message.
					Console.WriteLine("Finished {0} ({1}:{2}, {3}s/Page)", Path.GetFileName(_FilePath), Elapsed.Minutes.ToString("00"), Elapsed.Seconds.ToString("00"), (Elapsed.TotalSeconds / (_NumberOfPages == 0 ? 1 : _NumberOfPages)).ToString("0.0"));
				}
			}
		}
		#endregion

		#region IPublisher
		/// <summary>
		/// Publish an image.
		/// </summary>
		/// <param name="Image">The image.</param>
		/// <param name="PreviewImage">Indicates whether this is a preview image.</param>
		/// <param name="Number">The number, when not a preview image.</param>
		public ComicInfoPage Publish(byte[] Image, bool PreviewImage, int Number) {
			// Create a bitmap from the byte array.
			Bitmap Bitmap = Image.ToBitmap();
			// Attempt the following code.
			try {
				// Initialize the image height.
				int? ImageHeight = null;
				// Initialize the image width.
				int? ImageWidth = null;
				// Initialize the page valid status.
				bool IsValid = true;
				// Check if the bitmap is invalid.
				if (Bitmap == null) {
					// Write the message.
					Console.WriteLine("Shredded {0}:#{1}", Path.GetFileName(_FilePath), Number.ToString("000"));
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
							Graphics.DrawString(string.Format("Broken page #{0}", Number.ToString("000")), Font, Brushes.Black, RectangleF, StringFormat);
						}
						// Set the image.
						Image = BrokenBitmap.ToByteArray("png");
						// Set the image height.
						ImageHeight = BrokenBitmap.Height;
						// Set the image width.
						ImageWidth = BrokenBitmap.Width;
					}
				} else if (!PreviewImage) {
					// Indicating whether saving is required.
					bool RequiresSave = false;
					// Check if animation framing is not disabled.
					if (!_Options.DisableAnimationFraming) {
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
					if (!_Options.DisableFooterIncision && string.Equals(_Provider.UniqueIdentifier, "MangaFox")) {
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
					// Check if this is a platform compatible with image manipulation.
					if (PlatformID.MacOSX != Environment.OSVersion.Platform && PlatformID.Unix != Environment.OSVersion.Platform) {
						// Check if image processing is not disabled
						if (!_Options.DisableImageProcessing) {
							// Sharpen using a gaussian sharpen filter.
							Bitmap = Bitmap.Sharpen();
							// Reduce noise using conservative smoothing.
							Bitmap = Bitmap.Noise();
							// Adjust contrast in RGB colour space.
							Bitmap = Bitmap.Contrast();
							// Linear correction in RGB colour space.
							Bitmap = Bitmap.Colour();
							// Set the required save state.
							RequiresSave = true;
						}
						// Check if grayscale size comparison and save is not disabled.
						if (!_Options.DisableGrayscaleSizeComparisonAndSave) {
							// Determine whether the image is a PNG.
							bool IsPNG = Image.DetectImageFormat().Equals("png");
							// Check if this is either a PNG image or an image that requires saving.
							if (IsPNG || RequiresSave) {
								// Convert RGB colour space to grayscale when applicable.
								Bitmap = Bitmap.Grayscale();
								// Check if the image is a PNG but does not require to be saved.
								if (IsPNG && !RequiresSave) {
									// Create a byte array from the bitmap.
									byte[] GrayscaleImage = Bitmap.ToByteArray(Image.DetectImageFormat());
									// Check if the grayscale image has a smaller file size.
									if (GrayscaleImage.Length < Image.Length) {
										// Set the grayscale image.
										Image = GrayscaleImage;
									}
								}
							}
						}
					}
					// Check if saving is required.
					if (RequiresSave) {
						// Create a byte array from the bitmapy.
						Image = Bitmap.ToByteArray(Image.DetectImageFormat());
					}
				}
				// Save the image.
				if (true) {
					// Initialize the file name.
					string Key = string.Format("{0}.{1}", Number.ToString("000"), Image.DetectImageFormat());
					// Increment the number of pages.
					_NumberOfPages++;
					// Begin updating the compressed file.
					_ZipFile.BeginUpdate();
					// Attempt to delete files matching the file name and extension.
					_ZipFile.TryDelete(Number.ToString("000"), "bmp", "gif", "jpg", "png");
					// Add the file.
					_ZipFile.Add(new DataSource(Image), Key);
					// End updating the compressed file.
					_ZipFile.CommitUpdate();
					// Return comic page information ...
					return new ComicInfoPage {
						// ... with the image ...
						Image = Number,
						// ... with the image height ...
						ImageHeight = ImageHeight ?? Bitmap.Height,
						// ... with the image size ...
						ImageSize = Image.Length,
						// ... with the image width ...
						ImageWidth = ImageWidth ?? Bitmap.Width,
						// ... with the key ...
						Key = Key,
						// ... with the type.
						Type = IsValid ? (PreviewImage ? "FrontCover" : null) : "Deleted"
					};
				}
			} finally {
				// Check if the bitmap is valid.
				if (Bitmap != null) {
					// Dispose of the bitmap.
					Bitmap.Dispose();
				}
			}
		}

		/// <summary>
		/// Publish comic information.
		/// </summary>
		/// <param name="ComicInfo">The comic information.</param>
		public void Publish(ComicInfo ComicInfo) {
			// Check if meta-information is not disabled or check if repairing.
			if (_IsRepairing || !_Options.DisableMetaInformation) {
				// Initialize a new instance of the MemoryStream class.
				using (MemoryStream MemoryStream = new MemoryStream()) {
					// Save the comic information.
					ComicInfo.Save(MemoryStream);
					// Rewind the stream.
					MemoryStream.Position = 0;
					// Begin updating the compressed file.
					_ZipFile.BeginUpdate();
					// Add the file.
					_ZipFile.Add(new DataSource(MemoryStream), "ComicInfo.xml");
					// End updating the compressed file.
					_ZipFile.CommitUpdate();
				}
			}
		}

		/// <summary>
		/// Publish broken page information.
		/// </summary>
		/// <param name="BrokenPages">Each broken page.</param>
		public void Publish(IEnumerable<string> BrokenPages) {
			// Check if repair and error tracking is not disabled
			if (!_Options.DisableRepairAndErrorTracking) {
				// Check if the series directory does not exist.
				if (!Directory.Exists(Path.GetDirectoryName(_FilePath))) {
					// Create the directory for the series.
					Directory.CreateDirectory(Path.GetDirectoryName(_FilePath));
				}
				// Set whether there are broken pages.
				HasBrokenPages = true;
				// Write broken page information.
				File.WriteAllLines(string.Format("{0}.txt", _FilePath), BrokenPages.ToArray());
			}
		}
		#endregion
	}
}