// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace MangaRack.Concrete {
    /// <summary>
    /// Represents a stream data source.
    /// </summary>
    internal sealed class DataSource : IStaticDataSource {
        /// <summary>
        /// Contains the stream.
        /// </summary>
        private readonly Stream _stream;

        #region Constructor
        /// <summary>
        /// Initialize a new instance of the DataSource class.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        public DataSource(byte[] buffer)
            : this(new MemoryStream(buffer)) {}

        /// <summary>
        /// Initialize a new instance of the DataSource class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public DataSource(Stream stream) {
            _stream = stream;
        }
        #endregion

        #region IStaticDataSource
        /// <summary>
        /// Gets the source.
        /// </summary>
        public Stream GetSource() {
            return _stream;
        }
        #endregion
    }
}