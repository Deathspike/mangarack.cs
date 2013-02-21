// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System.IO;

namespace MangaRack {
	/// <summary>
	/// Represents the class providing extensions for the Stream class.
	/// </summary>
	public static class ExtensionForStream {
		/// <summary>
		/// Reads the bytes from the current stream and writes them to another stream.
		/// </summary>
		/// <param name="Source">The stream from which the contents will be read.</param>
		/// <param name="Destination">The stream to which the contents of the current stream will be copied.</param>
		public static void CopyTo(this Stream Source, Stream Destination) {
			// Initialize the buffer.
			byte[] Buffer = new byte[8192];
			// Initialize the number of bytes read.
			int BytesRead;
			// Read bytes from the source stream.
			while ((BytesRead = Source.Read(Buffer, 0, Buffer.Length)) > 0) {
				// Write the bytes to the destination stream.
				Destination.Write(Buffer, 0, BytesRead);
			}
		}
	}
}