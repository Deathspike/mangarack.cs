// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using ICSharpCode.SharpZipLib.Zip;
using System.IO;

namespace MangaRack {
	/// <summary>
	/// Represents a stream data source.
	/// </summary>
	public sealed class DataSource : IStaticDataSource {
		/// <summary>
		/// Contains the stream.
		/// </summary>
		private Stream _Stream;

		#region Constructor
		/// <summary>
		/// Initialize a new instance of the DataSource class.
		/// </summary>
		/// <param name="Buffer">The buffer.</param>
		public DataSource(byte[] Buffer)
			: this(new MemoryStream(Buffer)) {
			// Stop the function.
			return;
		}

		/// <summary>
		/// Initialize a new instance of the DataSource class.
		/// </summary>
		/// <param name="Stream">The stream.</param>
		public DataSource(Stream Stream) {
			// Set the stream.
			_Stream = Stream;
		}
		#endregion

		#region IStaticDataSource
		/// <summary>
		/// Gets the source.
		/// </summary>
		public Stream GetSource() {
			// Return the stream.
			return _Stream;
		}
		#endregion
	}
}