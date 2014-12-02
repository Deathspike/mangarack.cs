// ======================================================================
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace MangaRack.Internals {
	/// <summary>
	/// Represents a stream data source.
	/// </summary>
	class DataSource : IStaticDataSource {
		/// <summary>
		/// Contains the stream.
		/// </summary>
		private Stream _stream;

		#region Constructor
		/// <summary>
		/// Initialize a new instance of the DataSource class.
		/// </summary>
		/// <param name="buffer">The buffer.</param>
		public DataSource(byte[] buffer)
			: this(new MemoryStream(buffer)) {
			return;
		}

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