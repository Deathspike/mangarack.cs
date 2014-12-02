// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using AForge.Imaging;
using AForge.Imaging.Filters;

namespace MangaRack.Extension {
    /// <summary>
    /// Represents the class providing extensions for the Bitmap class.
    /// </summary>
    internal static class ExtensionForBitmap {
        #region Methods
        /// <summary>
        /// Convert grayscale to RGB colour space.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        public static Bitmap Channel(this Bitmap bitmap) {
            var imageStatistics = new ImageStatistics(bitmap);
            if (!imageStatistics.IsGrayscale) return bitmap;
            var grayscaleToRgb = new GrayscaleToRGB();
            var result = grayscaleToRgb.Apply(bitmap);
            bitmap.Dispose();
            return result;
        }

        /// <summary>
        /// Linear correction in RGB colour space.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        public static Bitmap Colour(this Bitmap bitmap) {
            if ((bitmap = bitmap.Channel()) == null) return null;
            var imageStatistics = new ImageStatistics(bitmap);
            var levelsLinear = new LevelsLinear {
                InRed = imageStatistics.Red.GetRange(0.995),
                InGreen = imageStatistics.Green.GetRange(0.995),
                InBlue = imageStatistics.Blue.GetRange(0.995)
            };
            levelsLinear.ApplyInPlace(bitmap);
            return bitmap;
        }

        /// <summary>
        /// Adjust contrast in RGB colour space.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        public static Bitmap Contrast(this Bitmap bitmap) {
            if ((bitmap = bitmap.Channel()) == null) return null;
            var contrastCorrection = new ContrastCorrection();
            contrastCorrection.ApplyInPlace(bitmap);
            return bitmap;
        }

        /// <summary>
        /// Crop the image to remove a textual addition.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        public static Bitmap Crop(this Bitmap bitmap) {
            if ((bitmap = bitmap.Channel()) == null) return null;
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
            if (incision == -1) return bitmap;
            var rectangle = new Rectangle(0, 0, bitmap.Width, bitmap.Height - incision);
            var result = bitmap.Clone(rectangle, bitmap.PixelFormat);
            bitmap.Dispose();
            return result;
        }

        /// <summary>
        /// Frame to the last available frame.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        public static Bitmap Frame(this Bitmap bitmap) {
            var frameDimension = new FrameDimension(bitmap.FrameDimensionsList[0]);
            var frameCount = bitmap.GetFrameCount(frameDimension);
            if (frameCount <= 1) return bitmap;
            bitmap.SelectActiveFrame(frameDimension, frameCount - 1);
            var result = new Bitmap(bitmap);
            bitmap.Dispose();
            return result;
        }

        /// <summary>
        /// Convert RGB colour space to grayscale when applicable.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        public static Bitmap Grayscale(this Bitmap bitmap) {
            if ((bitmap = bitmap.Channel()) == null) return null;
            var imageStatisticsHsl = new ImageStatisticsHSL(bitmap);
            if (imageStatisticsHsl.Saturation.Max != 0) return bitmap;
            var grayscale = new Grayscale(0.2125, 0.7154, 0.0721);
            var result = grayscale.Apply(bitmap);
            bitmap.Dispose();
            return result;
        }

        /// <summary>
        /// Reduce noise using conservative smoothing.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        public static Bitmap Noise(this Bitmap bitmap) {
            if ((bitmap = bitmap.Channel()) == null) return null;
            var conservativeSmoothing = new ConservativeSmoothing();
            conservativeSmoothing.ApplyInPlace(bitmap);
            return bitmap;
        }

        /// <summary>
        /// Sharpen using a gaussian sharpen filter.
        /// </summary>
        public static Bitmap Sharpen(this Bitmap bitmap) {
            if ((bitmap = bitmap.Channel()) == null) return null;
            var gaussianSharpen = new GaussianSharpen();
            gaussianSharpen.ApplyInPlace(bitmap);
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