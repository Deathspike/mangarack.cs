// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using MangaRack.Provider;
using System.Collections.Generic;

namespace MangaRack.Core {
	/// <summary>
	/// Represents the class providing extensions for the ComicInfo class.
	/// </summary>
	public static class ExtensionForComicInfo {
		#region Methods
		/// <summary>
		/// Transcribe the series, chapter and pages information.
		/// </summary>
		/// <param name="ComicInfo">The comic information.</param>
		/// <param name="Series">The series.</param>
		/// <param name="Chapter">The chapter.</param>
		/// <param name="Pages">Each page.</param>
		public static void Transcribe(this ComicInfo ComicInfo, ISeries Series, IChapter Chapter, IEnumerable<ComicInfoPage> Pages) {
			// Set each genre.
			ComicInfo.Genre = new ComicInfoSplitter(Series.Genres);
			// Set the manga specification.
			ComicInfo.Manga = "YesAndRightToLeft";
			// Set the number.
			ComicInfo.Number = Chapter.Number;
			// Set each page.
			ComicInfo.Pages = new ComicInfoPageCollection(Pages);
			// Set each penciller.
			ComicInfo.Penciller = new ComicInfoSplitter(Series.Artists);
			// Set the series.
			ComicInfo.Series = Series.Title;
			// Set the summary.
			ComicInfo.Summary = Series.Summary;
			// Set the title.
			ComicInfo.Title = Chapter.Title;
			// Set the volume.
			ComicInfo.Volume = Chapter.Volume;
			// Set each writer.
			ComicInfo.Writer = new ComicInfoSplitter(Series.Authors);
		}
		#endregion
	}
}