// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using ICSharpCode.SharpZipLib.Zip;
using MangaRack.Concrete;
using MangaRack.Core;
using MangaRack.Core.Abstract;
using MangaRack.Core.Concrete.Xml;
using MangaRack.Extension;
using MangaRack.Provider;
using MangaRack.Provider.Abstract;

namespace MangaRack {
    /// <summary>
    /// Represents a publisher.
    /// </summary>
    internal sealed class Publisher : IDisposable, IPublisher {
        /// <summary>
        /// Contains the begin time.
        /// </summary>
        private readonly long _beginTime;

        /// <summary>
        /// Contains the file path.
        /// </summary>
        private readonly string _filePath;

        /// <summary>
        /// Contains the file stream.
        /// </summary>
        private readonly FileStream _fileStream;

        /// <summary>
        /// Indicates whether the publisher is repairing.
        /// </summary>
        private readonly bool _isRepairing;

        /// <summary>
        /// Contains the number of pages.
        /// </summary>
        private int _numberOfPages;

        /// <summary>
        /// Contains a collection of options.
        /// </summary>
        private readonly Options _options;

        /// <summary>
        /// Contains the provider.
        /// </summary>
        private readonly IProvider _provider;

        /// <summary>
        /// Containst the compressed file.
        /// </summary>
        private readonly ZipFile _zipFile;

        #region Constructor
        /// <summary>
        /// Initialize a new instance of the Publisher class.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="options">The collection of options.</param>
        /// <param name="provider">The provider.</param>
        public Publisher(string filePath, Options options, IProvider provider)
            : this(filePath, options, provider, false) {}

        /// <summary>
        /// Initialize a new instance of the Publisher class.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="options">The collection of options.</param>
        /// <param name="provider">The provider.</param>
        /// <param name="isRepairing">Indicates whether the publisher is repairing.</param>
        public Publisher(string filePath, Options options, IProvider provider, bool isRepairing) {
            Console.WriteLine("{0} {1}", isRepairing ? "Checking" : "Fetching", Path.GetFileName(filePath));
            _beginTime = DateTime.Now.Ticks;
            _filePath = filePath;
            _fileStream = File.Open(isRepairing ? filePath : Path.GetTempFileName(), FileMode.OpenOrCreate);
            _isRepairing = isRepairing;
            _options = options;
            _provider = provider;
            _zipFile = isRepairing ? new ZipFile(_fileStream) : ZipFile.Create(_fileStream);
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
            _fileStream.Dispose();
            if (!File.Exists(_fileStream.Name)) return;
            if (!_isRepairing) {
                if (!Directory.Exists(Path.GetDirectoryName(_filePath))) {
                    Directory.CreateDirectory(Path.GetDirectoryName(_filePath));
                }
                File.Copy(_fileStream.Name, _filePath, true);
                File.Delete(_fileStream.Name);
            } else if (HasFailed) {
                File.Delete(_filePath);
            }
            if (HasFailed) {
                Console.WriteLine("{0} {1}", "Squashed", Path.GetFileName(_filePath));
            } else {
                var elapsed = new TimeSpan(DateTime.Now.Ticks - _beginTime);
                Console.WriteLine("Finished {0} ({1}:{2}, {3}s/Page)", Path.GetFileName(_filePath), elapsed.Minutes.ToString("00"), elapsed.Seconds.ToString("00"), (elapsed.TotalSeconds / (_numberOfPages == 0 ? 1 : _numberOfPages)).ToString("0.0"));
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
            var bitmap = image.ToBitmap();
            try {
                var imageHeight = (int?) null;
                var imageWidth = (int?) null;
                var isValid = true;
                if (bitmap == null) {
                    Console.WriteLine("Shredded {0}:#{1}", Path.GetFileName(_filePath), number.ToString("000"));
                    isValid = false;
                    using (var brokenBitmap = new Bitmap(128, 32)) {
                        using (var graphics = Graphics.FromImage(brokenBitmap)) {
                            var rectangleF = new RectangleF(0, 0, brokenBitmap.Width, brokenBitmap.Height);
                            var font = new Font("Tahoma", 10);
                            var stringFormat = new StringFormat {Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center};
                            graphics.FillRectangle(Brushes.White, rectangleF);
                            graphics.DrawString(string.Format("Broken page #{0}", number.ToString("000")), font, Brushes.Black, rectangleF, stringFormat);
                        }
                        image = brokenBitmap.ToByteArray("png");
                        imageHeight = brokenBitmap.Height;
                        imageWidth = brokenBitmap.Width;
                    }
                } else if (!previewImage) {
                    var requiresSave = false;
                    if (!_options.DisableAnimationFraming) {
                        var result = bitmap.Frame();
                        if (bitmap != result) {
                            bitmap = result;
                            requiresSave = true;
                        }
                    }
                    if (!_options.DisableFooterIncision && string.Equals(_provider.Location, "http://mangafox.me/")) {
                        var result = bitmap.Crop();
                        if (bitmap != result) {
                            bitmap = result;
                            requiresSave = true;
                        }
                    }
                    if (PlatformID.MacOSX != Environment.OSVersion.Platform && PlatformID.Unix != Environment.OSVersion.Platform) {
                        if (!_options.DisableImageProcessing) {
                            bitmap = bitmap.Sharpen();
                            bitmap = bitmap.Noise();
                            bitmap = bitmap.Contrast();
                            bitmap = bitmap.Colour();
                            requiresSave = true;
                        }
                        if (!_options.DisableGrayscaleSizeComparisonAndSave) {
                            var isPng = image.DetectImageFormat().Equals("png");
                            if (isPng || requiresSave) {
                                bitmap = bitmap.Grayscale();
                                if (isPng && !requiresSave) {
                                    var grayscaleImage = bitmap.ToByteArray(image.DetectImageFormat());
                                    if (grayscaleImage.Length < image.Length) {
                                        image = grayscaleImage;
                                    }
                                }
                            }
                        }
                    }
                    if (requiresSave) {
                        image = bitmap.ToByteArray(image.DetectImageFormat());
                    }
                }
                if (true) {
                    var key = string.Format("{0}.{1}", number.ToString("000"), image.DetectImageFormat());
                    _numberOfPages++;
                    _zipFile.BeginUpdate();
                    _zipFile.TryDelete(number.ToString("000"), "bmp", "gif", "jpg", "png");
                    _zipFile.Add(new DataSource(image), key);
                    _zipFile.CommitUpdate();
                    return new ComicInfoPage {
                        Image = number,
                        ImageHeight = imageHeight ?? bitmap.Height,
                        ImageSize = image.Length,
                        ImageWidth = imageWidth ?? bitmap.Width,
                        Key = key,
                        Type = isValid ? (previewImage ? "FrontCover" : null) : "Deleted"
                    };
                }
            } finally {
                if (bitmap != null) {
                    bitmap.Dispose();
                }
            }
        }

        /// <summary>
        /// Publish comic information.
        /// </summary>
        /// <param name="comicInfo">The comic information.</param>
        public void Publish(ComicInfo comicInfo) {
            if (!_isRepairing && _options.DisableMetaInformation) return;
            using (var memoryStream = new MemoryStream()) {
                comicInfo.Save(memoryStream);
                memoryStream.Position = 0;
                _zipFile.BeginUpdate();
                _zipFile.Add(new DataSource(memoryStream), "ComicInfo.xml");
                _zipFile.CommitUpdate();
            }
        }

        /// <summary>
        /// Publish broken page information.
        /// </summary>
        /// <param name="brokenPages">Each broken page.</param>
        public void Publish(IEnumerable<string> brokenPages) {
            if (_options.DisableRepairAndErrorTracking) return;
            if (!Directory.Exists(Path.GetDirectoryName(_filePath))) {
                Directory.CreateDirectory(Path.GetDirectoryName(_filePath));
            }
            HasBrokenPages = true;
            File.WriteAllLines(string.Format("{0}.txt", _filePath), brokenPages.ToArray());
        }
        #endregion
    }
}