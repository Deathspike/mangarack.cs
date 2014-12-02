// ======================================================================
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using AForge.Imaging;
using AForge.Imaging.Filters;

namespace MangaRack.Extensions {
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
			var imageStatistics = new ImageStatistics(bitmap);
			if (imageStatistics.IsGrayscale) {
				var grayscaleToRgb = new GrayscaleToRGB();
				var result = grayscaleToRgb.Apply(bitmap);
				bitmap.Dispose();
				return result;
			}
			return bitmap;
		}

		/// <summary>
		/// Linear correction in RGB colour space.
		/// </summary>
		/// <param name="bitmap">The bitmap.</param>
		public static Bitmap Colour(this Bitmap bitmap) {
			if ((bitmap = bitmap.Channel()) != null) {
				var imageStatistics = new ImageStatistics(bitmap);
				var levelsLinear = new LevelsLinear {
					InRed = imageStatistics.Red.GetRange(0.995),
					InGreen = imageStatistics.Green.GetRange(0.995),
					InBlue = imageStatistics.Blue.GetRange(0.995)
				};
				levelsLinear.ApplyInPlace(bitmap);
			}
			return bitmap;
		}

		/// <summary>
		/// Adjust contrast in RGB colour space.
		/// </summary>
		/// <param name="bitmap">The bitmap.</param>
		public static Bitmap Contrast(this Bitmap bitmap) {
			if ((bitmap = bitmap.Channel()) != null) {
				var contrastCorrection = new ContrastCorrection();
				contrastCorrection.ApplyInPlace(bitmap);
			}
			return bitmap;
		}

		/// <summary>
		/// Crop the image to remove a textual addition.
		/// </summary>
		/// <param name="bitmap">The bitmap.</param>
		public static Bitmap Crop(this Bitmap bitmap) {
			if ((bitmap = bitmap.Channel()) != null) {
				var channel = new int[3];
				var incision = -1;
				var firstBlack = -1;
				var previousBlack = -1;
				for (var line = 0; line < 80 && line < bitmap.Height; line++) {
					var hasBlack = false;
					var y = bitmap.Height - line - 1;
					channel[0] = channel[1] = channel[2] = 0;
					for (var x = 0; x < bitmap.Width; x++) {
						var color = bitmap.GetPixel(x, y);
						if (color.R < 45 && color.G < 45 && color.B < 45) {
							hasBlack = true;
							break;
						}
						channel[0] += color.R;
						channel[1] += color.G;
						channel[2] += color.B;
					}
					if (hasBlack) {
						if (firstBlack == -1 && (firstBlack = line > 5 ? 5 : line) == 0) {
							break;
						}
						previousBlack = line;
					} else if (firstBlack != -1 && previousBlack != -1) {
						channel[0] /= bitmap.Width;
						channel[1] /= bitmap.Width;
						channel[2] /= bitmap.Width;
						if (channel[0] >= 245 && channel[1] >= 245 && channel[2] >= 245) {
							incision = previousBlack + firstBlack;
						}
					}
				}
				if (incision != -1) {
					var rectangle = new Rectangle(0, 0, bitmap.Width, bitmap.Height - incision);
					var result = bitmap.Clone(rectangle, bitmap.PixelFormat);
					bitmap.Dispose();
					return result;
				}
			}
			return bitmap;
		}

		/// <summary>
		/// Frame to the last available frame.
		/// </summary>
		/// <param name="bitmap">The bitmap.</param>
		public static Bitmap Frame(this Bitmap bitmap) {
			if (bitmap.FrameDimensionsList != null) {
				var frameDimension = new FrameDimension(bitmap.FrameDimensionsList[0]);
				var frameCount = bitmap.GetFrameCount(frameDimension);
				if (frameCount > 1) {
					bitmap.SelectActiveFrame(frameDimension, frameCount - 1);
					var result = new Bitmap(bitmap);
					bitmap.Dispose();
					return result;
				}
			}
			return bitmap;
		}

		/// <summary>
		/// Convert RGB colour space to grayscale when applicable.
		/// </summary>
		/// <param name="bitmap">The bitmap.</param>
		public static Bitmap Grayscale(this Bitmap bitmap) {
			if ((bitmap = bitmap.Channel()) != null) {
				var imageStatisticsHSL = new ImageStatisticsHSL(bitmap);
				if (imageStatisticsHSL.Saturation.Max == 0) {
					var grayscale = new Grayscale(0.2125, 0.7154, 0.0721);
					var result = grayscale.Apply(bitmap);
					bitmap.Dispose();
					return result;
				}
			}
			return bitmap;
		}

		/// <summary>
		/// Reduce noise using conservative smoothing.
		/// </summary>
		/// <param name="bitmap">The bitmap.</param>
		public static Bitmap Noise(this Bitmap bitmap) {
			if ((bitmap = bitmap.Channel()) != null) {
				var conservativeSmoothing = new ConservativeSmoothing();
				conservativeSmoothing.ApplyInPlace(bitmap);
			}
			return bitmap;
		}

		/// <summary>
		/// Sharpen using a gaussian sharpen filter.
		/// </summary>
		public static Bitmap Sharpen(this Bitmap bitmap) {
			if ((bitmap = bitmap.Channel()) != null) {
				var gaussianSharpen = new GaussianSharpen();
				gaussianSharpen.ApplyInPlace(bitmap);
			}
			return bitmap;
		}

		/// <summary>
		/// Create an byte array from the bitmap.
		/// </summary>
		/// <param name="bitmap">The bitmap.</param>
		/// <param name="outputFormat">The output format.</param>
		public static byte[] ToByteArray(this Bitmap bitmap, string outputFormat) {
			using (var memoryStream = new MemoryStream()) {
				switch (outputFormat) {
					case "bmp":
						bitmap.Save(memoryStream, ImageFormat.Bmp);
						break;
					case "gif":
						bitmap.Save(memoryStream, ImageFormat.Gif);
						break;
					case "png":
						bitmap.Save(memoryStream, ImageFormat.Png);
						break;
					default:
						bitmap.Save(memoryStream, ImageFormat.Jpeg);
						break;
				}
				return memoryStream.ToArray();
			}
		}
		#endregion
	}
}