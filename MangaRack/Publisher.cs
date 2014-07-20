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
using MangaRack.Provider.Interfaces;

namespace MangaRack {
	/// <summary>
	/// Represents a publisher.
	/// </summary>
	class Publisher : IDisposable, IPublisher {
		/// <summary>
		/// Contains the begin time.
		/// </summary>
		private long _beginTime;

		/// <summary>
		/// Contains the file path.
		/// </summary>
		private string _filePath;

		/// <summary>
		/// Contains the file stream.
		/// </summary>
		private FileStream _fileStream;

		/// <summary>
		/// Indicates whether the publisher is repairing.
		/// </summary>
		private bool _isRepairing;

		/// <summary>
		/// Contains the number of pages.
		/// </summary>
		private int _numberOfPages;

		/// <summary>
		/// Contains a collection of options.
		/// </summary>
		private Options _options;

		/// <summary>
		/// Contains the provider.
		/// </summary>
		private IProvider _provider;

		/// <summary>
		/// Containst the compressed file.
		/// </summary>
		private ZipFile _zipFile;

		#region Constructor
		/// <summary>
		/// Initialize a new instance of the Publisher class.
		/// </summary>
		/// <param name="filePath">The file path.</param>
		/// <param name="options">The collection of options.</param>
		/// <param name="provider">The provider.</param>
		public Publisher(string filePath, Options options, IProvider provider)
			: this(filePath, options, provider, false) {
			// Stop the function.
			return;
		}

		/// <summary>
		/// Initialize a new instance of the Publisher class.
		/// </summary>
		/// <param name="filePath">The file path.</param>
		/// <param name="options">The collection of options.</param>
		/// <param name="provider">The provider.</param>
		/// <param name="isRepairing">Indicates whether the publisher is repairing.</param>
		public Publisher(string filePath, Options options, IProvider provider, bool isRepairing) {
			// Write the message.
			Console.WriteLine("{0} {1}", isRepairing ? "Checking" : "Fetching", Path.GetFileName(filePath));
			// Initialize properties.
			if (true) {
				// Set the begin time.
				_beginTime = DateTime.Now.Ticks;
				// Set the file path.
				_filePath = filePath;
				// Set the file stream.
				_fileStream = File.Open(isRepairing ? filePath : Path.GetTempFileName(), FileMode.OpenOrCreate);
				// Set whether the publisher is repairing.
				_isRepairing = isRepairing;
				// Set the collection of options.
				_options = options;
				// Set the provider.
				_provider = provider;
				// Set the compressed file.
				_zipFile = isRepairing ? new ZipFile(_fileStream) : ZipFile.Create(_fileStream);
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
			_fileStream.Dispose();
			// Check if the file does exist.
			if (File.Exists(_fileStream.Name)) {
				// Check if the publisher is not repairing.
				if (!_isRepairing) {
					// Check if the series directory does not exist.
					if (!Directory.Exists(Path.GetDirectoryName(_filePath))) {
						// Create the directory for the series.
						Directory.CreateDirectory(Path.GetDirectoryName(_filePath));
					}
					// Move the temporary file to the file path.
					File.Copy(_fileStream.Name, _filePath, true);
					// Delete the temporary file.
					File.Delete(_fileStream.Name);
				} else if (HasFailed) {
					// Delete the file.
					File.Delete(_filePath);
				}
				// Check if repairing has failed.
				if (HasFailed) {
					// Write the message.
					Console.WriteLine("{0} {1}", "Squashed", Path.GetFileName(_filePath));
				} else {
					// Initialize the elapsed time.
					var elapsed = new TimeSpan(DateTime.Now.Ticks - _beginTime);
					// Write the message.
					Console.WriteLine("Finished {0} ({1}:{2}, {3}s/Page)", Path.GetFileName(_filePath), elapsed.Minutes.ToString("00"), elapsed.Seconds.ToString("00"), (elapsed.TotalSeconds / (_numberOfPages == 0 ? 1 : _numberOfPages)).ToString("0.0"));
				}
			}
		}
		#endregion

		#region IPublisher
		/// <summary>
		/// Publish an image.
		/// </summary>
		/// <param name="image">The image.</param>
		/// <param name="previewImage">Indicates whether this is a preview image.</param>
		/// <param name="number">The number, when not a preview image.</param>
		public ComicInfoPage Publish(byte[] image, bool previewImage, int number) {
			// Create a bitmap from the byte array.
			var bitmap = image.ToBitmap();
			// Attempt the following code.
			try {
				// Initialize the image height.
				var imageHeight = (int?)null;
				// Initialize the image width.
				var imageWidth = (int?)null;
				// Initialize the page valid status.
				var isValid = true;
				// Check if the bitmap is invalid.
				if (bitmap == null) {
					// Write the message.
					Console.WriteLine("Shredded {0}:#{1}", Path.GetFileName(_filePath), number.ToString("000"));
					// Set the page valid status to false.
					isValid = false;
					// Initialize a new instance of the Bitmap class.
					using (var brokenBitmap = new Bitmap(128, 32)) {
						// Initialize a new instance of the Graphics class.
						using (var graphics = Graphics.FromImage(brokenBitmap)) {
							// Initialize a new instance of the RectangleF class.
							var rectangleF = new RectangleF(0, 0, brokenBitmap.Width, brokenBitmap.Height);
							// Initialize a new instance of the Font class.
							var font = new Font("Tahoma", 10);
							// Initialize a new instance of the StringFormat class.
							var stringFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
							// Fill the rectangle with a white brush.
							graphics.FillRectangle(Brushes.White, rectangleF);
							// Write the broken page information with a black brush.
							graphics.DrawString(string.Format("Broken page #{0}", number.ToString("000")), font, Brushes.Black, rectangleF, stringFormat);
						}
						// Set the image.
						image = brokenBitmap.ToByteArray("png");
						// Set the image height.
						imageHeight = brokenBitmap.Height;
						// Set the image width.
						imageWidth = brokenBitmap.Width;
					}
				} else if (!previewImage) {
					// Indicating whether saving is required.
					var requiresSave = false;
					// Check if animation framing is not disabled.
					if (!_options.DisableAnimationFraming) {
						// Frame to the last available frame.
						var result = bitmap.Frame();
						// Check if the result is modified.
						if (bitmap != result) {
							// Set the bitmap.
							bitmap = result;
							// Set the required save state.
							requiresSave = true;
						}
					}
					// Check if footer incision is not disabled.
					if (!_options.DisableFooterIncision && string.Equals(_provider.Location, "http://mangafox.me/")) {
						// Crop the image to remove a textual addition.
						var result = bitmap.Crop();
						// Check if the result is modified.
						if (bitmap != result) {
							// Set the bitmap.
							bitmap = result;
							// Set the required save state.
							requiresSave = true;
						}
					}
					// Check if this is a platform compatible with image manipulation.
					if (PlatformID.MacOSX != Environment.OSVersion.Platform && PlatformID.Unix != Environment.OSVersion.Platform) {
						// Check if image processing is not disabled
						if (!_options.DisableImageProcessing) {
							// Sharpen using a gaussian sharpen filter.
							bitmap = bitmap.Sharpen();
							// Reduce noise using conservative smoothing.
							bitmap = bitmap.Noise();
							// Adjust contrast in RGB colour space.
							bitmap = bitmap.Contrast();
							// Linear correction in RGB colour space.
							bitmap = bitmap.Colour();
							// Set the required save state.
							requiresSave = true;
						}
						// Check if grayscale size comparison and save is not disabled.
						if (!_options.DisableGrayscaleSizeComparisonAndSave) {
							// Determine whether the image is a PNG.
							var isPNG = image.DetectImageFormat().Equals("png");
							// Check if this is either a PNG image or an image that requires saving.
							if (isPNG || requiresSave) {
								// Convert RGB colour space to grayscale when applicable.
								bitmap = bitmap.Grayscale();
								// Check if the image is a PNG but does not require to be saved.
								if (isPNG && !requiresSave) {
									// Create a byte array from the bitmap.
									var grayscaleImage = bitmap.ToByteArray(image.DetectImageFormat());
									// Check if the grayscale image has a smaller file size.
									if (grayscaleImage.Length < image.Length) {
										// Set the grayscale image.
										image = grayscaleImage;
									}
								}
							}
						}
					}
					// Check if saving is required.
					if (requiresSave) {
						// Create a byte array from the bitmapy.
						image = bitmap.ToByteArray(image.DetectImageFormat());
					}
				}
				// Save the image.
				if (true) {
					// Initialize the file name.
					var key = string.Format("{0}.{1}", number.ToString("000"), image.DetectImageFormat());
					// Increment the number of pages.
					_numberOfPages++;
					// Begin updating the compressed file.
					_zipFile.BeginUpdate();
					// Attempt to delete files matching the file name and extension.
					_zipFile.TryDelete(number.ToString("000"), "bmp", "gif", "jpg", "png");
					// Add the file.
					_zipFile.Add(new DataSource(image), key);
					// End updating the compressed file.
					_zipFile.CommitUpdate();
					// Return comic page information ...
					return new ComicInfoPage {
						// ... with the image ...
						Image = number,
						// ... with the image height ...
						ImageHeight = imageHeight ?? bitmap.Height,
						// ... with the image size ...
						ImageSize = image.Length,
						// ... with the image width ...
						ImageWidth = imageWidth ?? bitmap.Width,
						// ... with the key ...
						Key = key,
						// ... with the type.
						Type = isValid ? (previewImage ? "FrontCover" : null) : "Deleted"
					};
				}
			} finally {
				// Check if the bitmap is valid.
				if (bitmap != null) {
					// Dispose of the bitmap.
					bitmap.Dispose();
				}
			}
		}

		/// <summary>
		/// Publish comic information.
		/// </summary>
		/// <param name="comicInfo">The comic information.</param>
		public void Publish(ComicInfo comicInfo) {
			// Check if meta-information is not disabled or check if repairing.
			if (_isRepairing || !_options.DisableMetaInformation) {
				// Initialize a new instance of the MemoryStream class.
				using (var memoryStream = new MemoryStream()) {
					// Save the comic information.
					comicInfo.Save(memoryStream);
					// Rewind the stream.
					memoryStream.Position = 0;
					// Begin updating the compressed file.
					_zipFile.BeginUpdate();
					// Add the file.
					_zipFile.Add(new DataSource(memoryStream), "ComicInfo.xml");
					// End updating the compressed file.
					_zipFile.CommitUpdate();
				}
			}
		}

		/// <summary>
		/// Publish broken page information.
		/// </summary>
		/// <param name="brokenPages">Each broken page.</param>
		public void Publish(IEnumerable<string> brokenPages) {
			// Check if repair and error tracking is not disabled
			if (!_options.DisableRepairAndErrorTracking) {
				// Check if the series directory does not exist.
				if (!Directory.Exists(Path.GetDirectoryName(_filePath))) {
					// Create the directory for the series.
					Directory.CreateDirectory(Path.GetDirectoryName(_filePath));
				}
				// Set whether there are broken pages.
				HasBrokenPages = true;
				// Write broken page information.
				File.WriteAllLines(string.Format("{0}.txt", _filePath), brokenPages.ToArray());
			}
		}
		#endregion
	}
}