// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace MangaRack.Core {
	/// <summary>
	/// Represents comic information.
	/// </summary>
	[XmlRoot("ComicInfo")]
	public sealed class ComicInfo {
		#region Methods
		/// <summary>
		/// Save the instance to a stream.
		/// </summary>
		/// <param name="Stream">The stream.</param>
		public void Save(Stream Stream) {
			// Initialize a new instance of the XmlSerializer class.
			XmlSerializer XmlSerializer = new XmlSerializer(typeof(ComicInfo));
			// Initialize a new instance of the StreamWriter class.
			StreamWriter StreamWriter = new StreamWriter(Stream, Encoding.UTF8);
			// Serialize the document
			XmlSerializer.Serialize(StreamWriter, this);
			// Flush the stream writer.
			StreamWriter.Flush();
		}
		#endregion

		#region Properties
		/// <summary>
		/// Contains each genre.
		/// </summary>
		[XmlElement]
		public ComicInfoSplitter Genre { get; set; }

		/// <summary>
		/// Contains the number.
		/// </summary>
		[XmlElement]
		public double Number { get; set; }

		/// <summary>
		/// Contains the manga specification.
		/// </summary>
		[XmlElement]
		public string Manga { get; set; }

		/// <summary>
		/// Contains each penciller.
		/// </summary>
		[XmlElement]
		public ComicInfoSplitter Penciller { get; set; }

		/// <summary>
		/// Contains the page count.
		/// </summary>
		[XmlElement]
		public int PageCount {
			get {
				return Pages.Count;
			}
		}

		/// <summary>
		/// Contains each page.
		/// </summary>
		[XmlElement]
		public ComicInfoPageCollection Pages { get; set; }

		/// <summary>
		/// Contains the series.
		/// </summary>
		[XmlElement]
		public string Series { get; set; }

		/// <summary>
		/// Contains the summary.
		/// </summary>
		[XmlElement]
		public string Summary { get; set; }

		/// <summary>
		/// Contains the title.
		/// </summary>
		[XmlElement]
		public string Title { get; set; }

		/// <summary>
		/// Contains the volume.
		/// </summary>
		[XmlElement]
		public double Volume { get; set; }

		/// <summary>
		/// Determines whether the volume is specified.
		/// </summary>
		[XmlIgnore]
		public bool VolumeSpecified {
			get {
				// Determine whether the volume is specified.
				return Volume != -1;
			}
		}

		/// <summary>
		/// Contains each writer.
		/// </summary>
		[XmlElement]
		public ComicInfoSplitter Writer { get; set; }
		#endregion

		#region Statics
		/// <summary>
		/// Load an instance from a stream.
		/// </summary>
		/// <param name="Stream">The stream.</param>
		/// <returns></returns>
		public static ComicInfo Load(Stream Stream) {
			// Initialize a new instance of the XmlSerializer class.
			XmlSerializer XmlSerializer = new XmlSerializer(typeof(ComicInfo));
			// Return the deserialized the document.
			return XmlSerializer.Deserialize(Stream) as ComicInfo;
		}
		#endregion
	}
}