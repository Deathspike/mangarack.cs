// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using MangaRack.Provider;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using MangaRack.Provider.Interfaces;

namespace MangaRack.Core {
	/// <summary>
	/// Represents comic information.
	/// </summary>
	[XmlRoot("ComicInfo")]
	public class ComicInfo {
		#region Constructor
		/// <summary>
		/// Initialize a new instance of the ComicInfo class.
		/// </summary>
		public ComicInfo() {
			// Set each genre.
			Genre = new ComicInfoSplitter();
			// Set each penciller.
			Penciller = new ComicInfoSplitter();
			// Set the volume.
			Volume = -1;
			// Set each writer.
			Writer = new ComicInfoSplitter();
		}
		#endregion

		#region Methods
		/// <summary>
		/// Transcribe the series, chapter and pages information.
		/// </summary>
		/// <param name="series">The series.</param>
		/// <param name="chapter">The chapter.</param>
		/// <param name="pages">Each page.</param>
		public void Transcribe(ISeries series, IChapter chapter, IEnumerable<ComicInfoPage> pages) {
			// Set each genre.
			Genre = new ComicInfoSplitter(series.Genres);
			// Set the manga specification.
			Manga = "YesAndRightToLeft";
			// Set the number.
			Number = chapter.Number;
			// Set each page.
			Pages = new ComicInfoPageCollection(pages);
			// Set each penciller.
			Penciller = new ComicInfoSplitter(series.Artists);
			// Set the series.
			Series = series.Title;
			// Set the summary.
			Summary = series.Summary;
			// Set the title.
			Title = chapter.Title;
			// Set the volume.
			Volume = chapter.Volume;
			// Set each writer.
			Writer = new ComicInfoSplitter(series.Authors);
		}

		/// <summary>
		/// Save the instance to a stream.
		/// </summary>
		/// <param name="stream">The stream.</param>
		public void Save(Stream stream) {
			// Initialize a new instance of the StreamWriter class.
			var streamWriter = new StreamWriter(stream, Encoding.UTF8);
			// Initialize a new instance of the XmlSerializer class.
			var xmlSerializer = new XmlSerializer(typeof(ComicInfo));
			// Serialize the document
			xmlSerializer.Serialize(streamWriter, this);
			// Flush the stream writer.
			streamWriter.Flush();
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
		public double? Number { get; set; }

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
		public double? Volume { get; set; }

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
		/// <param name="stream">The stream.</param>
		/// <returns></returns>
		public static ComicInfo Load(Stream stream) {
			// Initialize a new instance of the XmlSerializer class.
			var xmlSerializer = new XmlSerializer(typeof(ComicInfo));
			// Return the deserialized the document.
			return xmlSerializer.Deserialize(stream) as ComicInfo;
		}
		#endregion
	}
}