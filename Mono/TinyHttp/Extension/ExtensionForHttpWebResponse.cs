// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace TinyHttp {
	/// <summary>
	/// Represents the class providing extensions for the HttpWebResponse class.
	/// </summary>
	public static class ExtensionForHttpWebResponse {
		#region Methods
		/// <summary>
		/// Retrieve the response as binary.
		/// </summary>
		/// <param name="Response">The response.</param>
		public static byte[] AsBinary(this HttpWebResponse Response) {
			// Check if the response is invalid.
			if (Response == null) {
				// Return null.
				return null;
			}
			// Get the stream that is used to read the response from the server.
			using (Stream Stream = Response.AsUncompressed()) {
				// Initialize a new instance of the MemoryStream class.
				using (MemoryStream MemoryStream = new MemoryStream()) {
					// Initialize the buffer.
					byte[] Buffer = new byte[4096];
					// Initialize the counter.
					int Count;
					// Read available bytes from the stream into the buffer.
					while ((Count = Stream.Read(Buffer, 0, Buffer.Length)) != 0) {
						// Write to read bytes into the memory stream.
						MemoryStream.Write(Buffer, 0, Count);
					}
					// Return the memory stream as a byte array.
					return MemoryStream.Position != 0 ? MemoryStream.ToArray() : null;
				}
			}
		}

		/// <summary>
		/// Retrieve the response as string.
		/// </summary>
		/// <param name="Response">The response.</param>
		public static string AsString(this HttpWebResponse Response) {
			// Check if the response is invalid.
			if (Response == null) {
				// Return null.
				return null;
			}
			// Get the stream that is used to read the response from the server.
			using (Stream Stream = Response.AsUncompressed()) {
				// Initialize the content type pair.
				string[] ContentTypePair = Response.ContentType.Split(new[] { ';' });
				// Initialize the encoding.
				Encoding Encoding = Encoding.GetEncoding("ISO-8859-1");
				// Check if the content type pair is valid.
				if (ContentTypePair.Length == 2) {
					// Retrieve the character set pair.
					string[] CharacterSetPair = ContentTypePair[1].Split(new[] { '=' });
					// Check if the character set pair is valid.
					if (CharacterSetPair.Length == 2 && CharacterSetPair[0].TrimStart().Equals("charset")) {
						// Attempt the following code.
						try {
							// Set the character set of the input stream.
							Encoding = Encoding.GetEncoding(CharacterSetPair[1]);
						} catch {
							// Check if the character set is a misspelled UTF-8.
							if (CharacterSetPair[1].Equals("utf8")) {
								// Set the character set of the input stream.
								Encoding = Encoding.UTF8;
							}
						}
					}
				}
				// Initialize a new instance of the StreamReader class.
				using (StreamReader StreamReader = new StreamReader(Stream, Encoding)) {
					// Invoke the handler indicating the request is completed.
					return StreamReader.ReadToEnd();
				}
			}
		}

		/// <summary>
		/// Retrieve the response stream as an uncompressed stream.
		/// </summary>
		/// <param name="Response">The response.</param>
		public static Stream AsUncompressed(this HttpWebResponse Response) {
			// Check if the response is invalid.
			if (Response == null) {
				// Return null.
				return null;
			}
			// Check if the response stream is a compressed stream.
			if (Response.Headers.AllKeys.Contains("Content-Encoding")) {
				// Return the appropriate decompression response stream.
				return Response.Headers["Content-Encoding"].Equals("gzip") ? new GZipInputStream(Response.GetResponseStream()) : new InflaterInputStream(Response.GetResponseStream());
			}
			// Return the response stream.
			return Response.GetResponseStream();
		}
		#endregion
	}
}