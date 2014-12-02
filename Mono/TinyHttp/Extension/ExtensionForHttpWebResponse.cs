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
			if (Response == null) {
				return null;
			}
			using (Stream Stream = Response.AsUncompressed()) {
				using (MemoryStream MemoryStream = new MemoryStream()) {
					byte[] Buffer = new byte[4096];
					int Count;
					while ((Count = Stream.Read(Buffer, 0, Buffer.Length)) != 0) {
						MemoryStream.Write(Buffer, 0, Count);
					}
					return MemoryStream.Position != 0 ? MemoryStream.ToArray() : null;
				}
			}
		}

		/// <summary>
		/// Retrieve the response as string.
		/// </summary>
		/// <param name="Response">The response.</param>
		public static string AsString(this HttpWebResponse Response) {
			if (Response == null) {
				return null;
			}
			using (Stream Stream = Response.AsUncompressed()) {
				string[] ContentTypePair = Response.ContentType.Split(new[] { ';' });
				Encoding Encoding = Encoding.GetEncoding("ISO-8859-1");
				if (ContentTypePair.Length == 2) {
					string[] CharacterSetPair = ContentTypePair[1].Split(new[] { '=' });
					if (CharacterSetPair.Length == 2 && CharacterSetPair[0].TrimStart().Equals("charset")) {
						try {
							Encoding = Encoding.GetEncoding(CharacterSetPair[1]);
						} catch {
							if (CharacterSetPair[1].Equals("utf8")) {
								Encoding = Encoding.UTF8;
							}
						}
					}
				}
				using (StreamReader StreamReader = new StreamReader(Stream, Encoding)) {
					return StreamReader.ReadToEnd();
				}
			}
		}

		/// <summary>
		/// Retrieve the response stream as an uncompressed stream.
		/// </summary>
		/// <param name="Response">The response.</param>
		public static Stream AsUncompressed(this HttpWebResponse Response) {
			if (Response == null) {
				return null;
			}
			if (Response.Headers.AllKeys.Contains("Content-Encoding")) {
				return Response.Headers["Content-Encoding"].Equals("gzip") ? new GZipInputStream(Response.GetResponseStream()) : new InflaterInputStream(Response.GetResponseStream());
			}
			return Response.GetResponseStream();
		}
		#endregion
	}
}