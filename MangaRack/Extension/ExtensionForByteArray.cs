// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System.Drawing;
using System.IO;
using System.Linq;

namespace MangaRack.Extension {
    /// <summary>
    /// Represents the class providing extensions for each byte array.
    /// </summary>
    internal static class ExtensionForByteArray {
        /// <summary>
        /// Contains the Bitmap (BMP) header.
        /// </summary>
        private static readonly byte[] Bmp = {66, 77};

        /// <summary>
        /// Contains the Graphics Interchange Format (GIF) header.
        /// </summary>
        private static readonly byte[] Gif = {71, 73, 70};

        /// <summary>
        /// Contains the Joint Photographic Experts Group (JPEG) header.
        /// </summary>
        private static readonly byte[] Jpg = {255, 216};

        /// <summary>
        /// Contains the W3C Portable Network Graphics (PNG) header.
        /// </summary>
        private static readonly byte[] Png = {137, 80, 78, 71};

        #region Methods
        /// <summary>
        /// Detect an image format from the header.
        /// </summary>
        /// <param name="buffer">Each byte.</param>
        public static string DetectImageFormat(this byte[] buffer) {
            if (Bmp.SequenceEqual(buffer.Take(Bmp.Length))) {
                return "bmp";
            }
            if (Gif.SequenceEqual(buffer.Take(Gif.Length))) {
                return "gif";
            }
            if (Jpg.SequenceEqual(buffer.Take(Jpg.Length))) {
                return "jpg";
            }
            if (Png.SequenceEqual(buffer.Take(Png.Length))) {
                return "png";
            }
            return null;
        }

        /// <summary>
        /// Create an bitmap from the byte array.
        /// </summary>
        /// <param name="buffer">Each byte.</param>
        public static Bitmap ToBitmap(this byte[] buffer) {
            try {
                using (var memoryStream = new MemoryStream(buffer)) {
                    using (var bitmap = Image.FromStream(memoryStream) as Bitmap) {
                        return new Bitmap(bitmap);
                    }
                }
            } catch {
                return null;
            }
        }
        #endregion
    }
}