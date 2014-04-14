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

namespace MangaRack.Core {
	/// <summary>
	/// Represents comic information.
	/// </summary>
	[XmlRoot("ComicInfo")]
	public sealed class ComicInfo {
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
		/// <param name="Series">The series.</param>
		/// <param name="Chapter">The chapter.</param>
		/// <param name="Pages">Each page.</param>
		public void Transcribe(ISeries Series, IChapter Chapter, IEnumerable<ComicInfoPage> Pages) {
			// Set each genre.
			this.Genre = new ComicInfoSplitter(Series.Genres);
			// Set the manga specification.
			this.Manga = "YesAndRightToLeft";
			// Set the number.
			this.Number = Chapter.Number;
			// Set each page.
			this.Pages = new ComicInfoPageCollection(Pages);
			// Set each penciller.
			this.Penciller = new ComicInfoSplitter(Series.Artists);
			// Set the series.
			this.Series = Series.Title;
			// Set the summary.
			this.Summary = Series.Summary;
			// Set the title.
			this.Title = Chapter.Title;
			// Set the volume.
			this.Volume = Chapter.Volume;
			// Set each writer.
			this.Writer = new ComicInfoSplitter(Series.Authors);
		}

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