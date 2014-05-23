// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using AForge.Imaging;
using AForge.Imaging.Filters;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace MangaRack {
	/// <summary>
	/// Represents the class providing extensions for the Bitmap class.
	/// </summary>
	static class ExtensionForBitmap {
		#region Methods
		/// <summary>
		/// Convert grayscale to RGB colour space.
		/// </summary>
		/// <param name="bitmap">The bitmap.</param>
		public static Bitmap Channel(this Bitmap bitmap) {
			// Initialize a new instance of the ImageStatistics class.
			var imageStatistics = new ImageStatistics(bitmap);
			// Check if the image is grayscale.
			if (imageStatistics.IsGrayscale) {
				// Initialize a new instance of the GrayscaleToRGB class.
				var grayscaleToRgb = new GrayscaleToRGB();
				// Apply the filter to the image.
				var result = grayscaleToRgb.Apply(bitmap);
				// Dispose of the original image.
				bitmap.Dispose();
				// Return the result.
				return result;
			}
			// Return the bitmap.
			return bitmap;
		}

		/// <summary>
		/// Linear correction in RGB colour space.
		/// </summary>
		/// <param name="bitmap">The bitmap.</param>
		public static Bitmap Colour(this Bitmap bitmap) {
			// Convert grayscale to RGB colour space.
			if ((bitmap = bitmap.Channel()) != null) {
				// Initialize a new instance of the LevelsLinear class.
				var imageStatistics = new ImageStatistics(bitmap);
				// Initialize a new instance of the LevelsLinear class.
				var levelsLinear = new LevelsLinear {
					// Retrieve and set the range around the median for the red-channel.
					InRed = imageStatistics.Red.GetRange(0.995),
					// Retrieve and set the range around the median for the green-channel.
					InGreen = imageStatistics.Green.GetRange(0.995),
					// Retrieve and set the range around the median for the blue-channel.
					InBlue = imageStatistics.Blue.GetRange(0.995)
				};
				// Apply the filter to the image.
				levelsLinear.ApplyInPlace(bitmap);
			}
			// Return the bitmap.
			return bitmap;
		}

		/// <summary>
		/// Adjust contrast in RGB colour space.
		/// </summary>
		/// <param name="bitmap">The bitmap.</param>
		public static Bitmap Contrast(this Bitmap bitmap) {
			// Convert grayscale to RGB colour space.
			if ((bitmap = bitmap.Channel()) != null) {
				// Initialize a new instance of the ContrastCorrection class.
				var contrastCorrection = new ContrastCorrection();
				// Apply the filter to the image.
				contrastCorrection.ApplyInPlace(bitmap);
			}
			// Return the bitmap.
			return bitmap;
		}

		/// <summary>
		/// Crop the image to remove a textual addition.
		/// </summary>
		/// <param name="bitmap">The bitmap.</param>
		public static Bitmap Crop(this Bitmap bitmap) {
			// Convert grayscale to RGB colour space.
			if ((bitmap = bitmap.Channel()) != null) {
				// Initialize each channel.
				var channel = new int[3];
				// Initialize the incision line.
				var incision = -1;
				// Initialize the first black line.
				var firstBlack = -1;
				// Initialize the previous black line.
				var previousBlack = -1;
				// Iterate through each line until the maximum incision height.
				for (var line = 0; line < 80 && line < bitmap.Height; line++) {
					// Initialize the boolean indicating whether the line has black.
					var hasBlack = false;
					// Initialize the line.
					var y = bitmap.Height - line - 1;
					// Reset each channel.
					channel[0] = channel[1] = channel[2] = 0;
					// Iterate through each pixel on the line.
					for (var x = 0; x < bitmap.Width; x++) {
						// Retrieve the color for the pixel.
						var color = bitmap.GetPixel(x, y);
						// Check if the color is considered to be black.
						if (color.R < 45 && color.G < 45 && color.B < 45) {
							// Set the status indicating this line has black.
							hasBlack = true;
							// Break from the iteration.
							break;
						}
						// Add the red component to the red channel.
						channel[0] += color.R;
						// Add the green component to the red channel.
						channel[1] += color.G;
						// Add the blue component to the red channel.
						channel[2] += color.B;
					}
					// Check if the line has black.
					if (hasBlack) {
						// Check if the first black line has not been set.
						if (firstBlack == -1 && (firstBlack = line > 5 ? 5 : line) == 0) {
							// Break when the first line is black.
							break;
						}
						// Set the previous black line.
						previousBlack = line;
					} else if (firstBlack != -1 && previousBlack != -1) {
						// Divide the red color channel to attain the median.
						channel[0] /= bitmap.Width;
						// Divide the green color channel to attain the median.
						channel[1] /= bitmap.Width;
						// Divide the blue color channel to attain the median.
						channel[2] /= bitmap.Width;
						// Check if the line is considered to be white.
						if (channel[0] >= 245 && channel[1] >= 245 && channel[2] >= 245) {
							// Set the incision line.
							incision = previousBlack + firstBlack;
						}
					}
				}
				// Check if an incision line is available.
				if (incision != -1) {
					// Initialize the rectangle.
					var rectangle = new Rectangle(0, 0, bitmap.Width, bitmap.Height - incision);
					// Initialize the result.
					var result = bitmap.Clone(rectangle, bitmap.PixelFormat);
					// Dispose of the image.
					bitmap.Dispose();
					// Return the result.
					return result;
				}
			}
			// Return the bitmap.
			return bitmap;
		}

		/// <summary>
		/// Frame to the last available frame.
		/// </summary>
		/// <param name="bitmap">The bitmap.</param>
		public static Bitmap Frame(this Bitmap bitmap) {
			// Check if the frame dimension list is available.
			if (bitmap.FrameDimensionsList != null) {
				// Initialize a new instance of the FrameDimension class.
				var frameDimension = new FrameDimension(bitmap.FrameDimensionsList[0]);
				// Retrieve the number of frames.
				var frameCount = bitmap.GetFrameCount(frameDimension);
				// Check if more than one frame is available.
				if (frameCount > 1) {
					// Select the last frame containing the appropriate image.
					bitmap.SelectActiveFrame(frameDimension, frameCount - 1);
					// Create the image for the frame.
					var result = new Bitmap(bitmap);
					// Dispose of the original image.
					bitmap.Dispose();
					// Return the result.
					return result;
				}
			}
			// Return the bitmap.
			return bitmap;
		}

		/// <summary>
		/// Convert RGB colour space to grayscale when applicable.
		/// </summary>
		/// <param name="bitmap">The bitmap.</param>
		public static Bitmap Grayscale(this Bitmap bitmap) {
			// Convert grayscale to RGB colour space.
			if ((bitmap = bitmap.Channel()) != null) {
				// Initialize a new instance of the ImageStatisticsHSL class.
				var imageStatisticsHSL = new ImageStatisticsHSL(bitmap);
				// Check if the image is grayscale.
				if (imageStatisticsHSL.Saturation.Max == 0) {
					// Initialize a new instance of the Grayscale class.
					var grayscale = new Grayscale(0.2125, 0.7154, 0.0721);
					// Apply the filter to the image.
					var result = grayscale.Apply(bitmap);
					// Dispose of the original image.
					bitmap.Dispose();
					// Return the result.
					return result;
				}
			}
			// Return the bitmap.
			return bitmap;
		}

		/// <summary>
		/// Reduce noise using conservative smoothing.
		/// </summary>
		/// <param name="bitmap">The bitmap.</param>
		public static Bitmap Noise(this Bitmap bitmap) {
			// Convert grayscale to RGB colour space.
			if ((bitmap = bitmap.Channel()) != null) {
				// Initialize a new instance of the ConservativeSmoothing class.
				var conservativeSmoothing = new ConservativeSmoothing();
				// Reduce noise while preserving detail.
				conservativeSmoothing.ApplyInPlace(bitmap);
			}
			// Return the bitmap.
			return bitmap;
		}

		/// <summary>
		/// Sharpen using a gaussian sharpen filter.
		/// </summary>
		public static Bitmap Sharpen(this Bitmap bitmap) {
			// Convert grayscale to RGB colour space.
			if ((bitmap = bitmap.Channel()) != null) {
				// Initialize a new instance of the GaussianSharpen class.
				var gaussianSharpen = new GaussianSharpen();
				// Apply the filter to the image.
				gaussianSharpen.ApplyInPlace(bitmap);
			}
			// Return the bitmap.
			return bitmap;
		}

		/// <summary>
		/// Create an byte array from the bitmap.
		/// </summary>
		/// <param name="bitmap">The bitmap.</param>
		/// <param name="outputFormat">The output format.</param>
		public static byte[] ToByteArray(this Bitmap bitmap, string outputFormat) {
			// Initialize a new instance of the MemoryStream class.
			using (var memoryStream = new MemoryStream()) {
				// Switch on the output format.
				switch (outputFormat) {
					case "bmp":
						// Save using the Bitmap (BMP) format.
						bitmap.Save(memoryStream, ImageFormat.Bmp);
						// Break iteration.
						break;
					case "gif":
						// Save using the Graphics Interchange Format (GIF) format.
						bitmap.Save(memoryStream, ImageFormat.Gif);
						// Break iteration.
						break;
					case "png":
						// Save using the W3C Portable Network Graphics (PNG) format.
						bitmap.Save(memoryStream, ImageFormat.Png);
						// Break iteration.
						break;
					default:
						// Save using the Joint Photographic Experts Group (JPEG) format.
						bitmap.Save(memoryStream, ImageFormat.Jpeg);
						// Break iteration.
						break;
				}
				// Return the image.
				return memoryStream.ToArray();
			}
		}
		#endregion
	}
}