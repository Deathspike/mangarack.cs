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
		/// <param name="Bitmap">The bitmap.</param>
		public static Bitmap Channel(this Bitmap Bitmap) {
			// Initialize a new instance of the ImageStatistics class.
			ImageStatistics ImageStatistics = new ImageStatistics(Bitmap);
			// Check if the image is grayscale.
			if (ImageStatistics.IsGrayscale) {
				// Initialize a new instance of the GrayscaleToRGB class.
				GrayscaleToRGB GrayscaleToRGB = new GrayscaleToRGB();
				// Apply the filter to the image.
				Bitmap Result = GrayscaleToRGB.Apply(Bitmap);
				// Dispose of the original image.
				Bitmap.Dispose();
				// Return the result.
				return Result;
			}
			// Return the bitmap.
			return Bitmap;
		}

		/// <summary>
		/// Linear correction in RGB colour space.
		/// </summary>
		/// <param name="Bitmap">The bitmap.</param>
		public static Bitmap Colour(this Bitmap Bitmap) {
			// Convert grayscale to RGB colour space.
			if ((Bitmap = Bitmap.Channel()) != null) {
				// Initialize a new instance of the LevelsLinear class.
				ImageStatistics ImageStatistics = new ImageStatistics(Bitmap);
				// Initialize a new instance of the LevelsLinear class.
				LevelsLinear LevelsLinear = new LevelsLinear {
					// Retrieve and set the range around the median for the red-channel.
					InRed = ImageStatistics.Red.GetRange(0.995),
					// Retrieve and set the range around the median for the green-channel.
					InGreen = ImageStatistics.Green.GetRange(0.995),
					// Retrieve and set the range around the median for the blue-channel.
					InBlue = ImageStatistics.Blue.GetRange(0.995)
				};
				// Apply the filter to the image.
				LevelsLinear.ApplyInPlace(Bitmap);
			}
			// Return the bitmap.
			return Bitmap;
		}

		/// <summary>
		/// Adjust contrast in RGB colour space.
		/// </summary>
		public static Bitmap Contrast(this Bitmap Bitmap) {
			// Convert grayscale to RGB colour space.
			if ((Bitmap = Bitmap.Channel()) != null) {
				// Initialize a new instance of the ContrastCorrection class.
				var ContrastCorrection = new ContrastCorrection();
				// Apply the filter to the image.
				ContrastCorrection.ApplyInPlace(Bitmap);
			}
			// Return the bitmap.
			return Bitmap;
		}

		/// <summary>
		/// Crop the image to remove a textual addition.
		/// </summary>
		/// <param name="Bitmap">The bitmap.</param>
		public static Bitmap Crop(this Bitmap Bitmap) {
			// Convert grayscale to RGB colour space.
			if ((Bitmap = Bitmap.Channel()) != null) {
				// Initialize each channel.
				int[] Channel = new int[3];
				// Initialize the incision line.
				int Incision = -1;
				// Initialize the first black line.
				int FirstBlack = -1;
				// Initialize the previous black line.
				int PreviousBlack = -1;
				// Iterate through each line until the maximum incision height.
				for (int Line = 0; Line < 80 && Line < Bitmap.Height; Line++) {
					// Initialize the boolean indicating whether the line has black.
					bool HasBlack = false;
					// Initialize the line.
					int Y = Bitmap.Height - Line - 1;
					// Reset each channel.
					Channel[0] = Channel[1] = Channel[2] = 0;
					// Iterate through each pixel on the line.
					for (int X = 0; X < Bitmap.Width; X++) {
						// Retrieve the color for the pixel.
						Color Color = Bitmap.GetPixel(X, Y);
						// Check if the color is considered to be black.
						if (Color.R < 45 && Color.G < 45 && Color.B < 45) {
							// Set the status indicating this line has black.
							HasBlack = true;
							// Break from the iteration.
							break;
						}
						// Add the red component to the red channel.
						Channel[0] += Color.R;
						// Add the green component to the red channel.
						Channel[1] += Color.G;
						// Add the blue component to the red channel.
						Channel[2] += Color.B;
					}
					// Check if the line has black.
					if (HasBlack) {
						// Check if the first black line has not been set.
						if (FirstBlack == -1 && (FirstBlack = Line > 5 ? 5 : Line) == 0) {
							// Break when the first line is black.
							break;
						}
						// Set the previous black line.
						PreviousBlack = Line;
					} else if (FirstBlack != -1 && PreviousBlack != -1) {
						// Divide the red color channel to attain the median.
						Channel[0] /= Bitmap.Width;
						// Divide the green color channel to attain the median.
						Channel[1] /= Bitmap.Width;
						// Divide the blue color channel to attain the median.
						Channel[2] /= Bitmap.Width;
						// Check if the line is considered to be white.
						if (Channel[0] >= 245 && Channel[1] >= 245 && Channel[2] >= 245) {
							// Set the incision line.
							Incision = PreviousBlack + FirstBlack;
						}
					}
				}
				// Check if an incision line is available.
				if (Incision != -1) {
					// Initialize a new instance of the Crop class.
					Crop Crop = new Crop(new Rectangle(0, 0, Bitmap.Width, Bitmap.Height - Incision));
					// Apply the filter to the image.
					Bitmap Result = Crop.Apply(Bitmap);
					// Dispose of the image.
					Bitmap.Dispose();
					// Return the result.
					return Result;
				}
			}
			// Return the bitmap.
			return Bitmap;
		}

		/// <summary>
		/// Frame to the last available frame.
		/// </summary>
		/// <param name="Bitmap">The bitmap.</param>
		public static Bitmap Frame(this Bitmap Bitmap) {
			// Check if the frame dimension list is available.
			if (Bitmap.FrameDimensionsList != null) {
				// Initialize a new instance of the FrameDimension class.
				FrameDimension FrameDimension = new FrameDimension(Bitmap.FrameDimensionsList[0]);
				// Retrieve the number of frames.
				int FrameCount = Bitmap.GetFrameCount(FrameDimension);
				// Check if more than one frame is available.
				if (FrameCount > 1) {
					// Select the last frame containing the appropriate image.
					Bitmap.SelectActiveFrame(FrameDimension, FrameCount - 1);
					// Create the image for the frame.
					Bitmap Result = new Bitmap(Bitmap);
					// Dispose of the original image.
					Bitmap.Dispose();
					// Return the result.
					return Result;
				}
			}
			// Return the bitmap.
			return Bitmap;
		}

		/// <summary>
		/// Convert RGB colour space to grayscale when applicable.
		/// </summary>
		public static Bitmap Grayscale(this Bitmap Bitmap) {
			// Convert grayscale to RGB colour space.
			if ((Bitmap = Bitmap.Channel()) != null) {
				// Initialize a new instance of the ImageStatisticsHSL class.
				ImageStatisticsHSL ImageStatisticsHSL = new ImageStatisticsHSL(Bitmap);
				// Check if the image is grayscale.
				if (ImageStatisticsHSL.Saturation.Max == 0) {
					// Initialize a new instance of the Grayscale class.
					Grayscale Grayscale = new Grayscale(0.2125, 0.7154, 0.0721);
					// Apply the filter to the image.
					Bitmap Result = Grayscale.Apply(Bitmap);
					// Dispose of the original image.
					Bitmap.Dispose();
					// Return the result.
					return Result;
				}
			}
			// Return the bitmap.
			return Bitmap;
		}

		/// <summary>
		/// Reduce noise using conservative smoothing.
		/// </summary>
		public static Bitmap Noise(this Bitmap Bitmap) {
			// Convert grayscale to RGB colour space.
			if ((Bitmap = Bitmap.Channel()) != null) {
				// Initialize a new instance of the ConservativeSmoothing class.
				var ConservativeSmoothing = new ConservativeSmoothing();
				// Reduce noise while preserving detail.
				ConservativeSmoothing.ApplyInPlace(Bitmap);
			}
			// Return the bitmap.
			return Bitmap;
		}

		/// <summary>
		/// Sharpen using a gaussian sharpen filter.
		/// </summary>
		public static Bitmap Sharpen(this Bitmap Bitmap) {
			// Convert grayscale to RGB colour space.
			if ((Bitmap = Bitmap.Channel()) != null) {
				// Initialize a new instance of the GaussianSharpen class.
				var GaussianSharpen = new GaussianSharpen();
				// Apply the filter to the image.
				GaussianSharpen.ApplyInPlace(Bitmap);
			}
			// Return the bitmap.
			return Bitmap;
		}

		/// <summary>
		/// Create an byte array from the bitmap.
		/// </summary>
		/// <param name="Bitmap">The bitmap.</param>
		/// <param name="OutputFormat">The output format.</param>
		public static byte[] ToByteArray(this Bitmap Bitmap, string OutputFormat) {
			// Initialize a new instance of the MemoryStream class.
			using (MemoryStream MemoryStream = new MemoryStream()) {
				// Switch on the output format.
				switch (OutputFormat) {
					case "bmp":
						// Save using the Bitmap (BMP) format.
						Bitmap.Save(MemoryStream, ImageFormat.Bmp);
						// Break iteration.
						break;
					case "gif":
						// Save using the Graphics Interchange Format (GIF) format.
						Bitmap.Save(MemoryStream, ImageFormat.Gif);
						// Break iteration.
						break;
					case "png":
						// Save using the W3C Portable Network Graphics (PNG) format.
						Bitmap.Save(MemoryStream, ImageFormat.Png);
						// Break iteration.
						break;
					default:
						// Save using the Joint Photographic Experts Group (JPEG) format.
						Bitmap.Save(MemoryStream, ImageFormat.Jpeg);
						// Break iteration.
						break;
				}
				// Return the image.
				return MemoryStream.ToArray();
			}
		}
		#endregion
	}
}