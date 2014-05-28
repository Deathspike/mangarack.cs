// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using MangaRack.Provider;
using System;
using System.Collections.Generic;

namespace MangaRack {
	/// <summary>
	/// Represents the class providing extensions for the IChapter interface.
	/// </summary>
	static class ExtensionForChapter {
		#region Methods
		/// <summary>
		/// Filter the chapters according to the chapter and volume filter.
		/// </summary>
		/// <param name="chapters">Each chapter.</param>
		/// <param name="options">The collection of options.</param>
		public static IEnumerable<IChapter> Filter(this IEnumerable<IChapter> chapters, Options options) {
			// Iterate through each chapter.
			foreach (var chapter in chapters) {
				// Check if the chapter number is valid.
				if (chapter.Number != -1) {
					// Filter on negative chapter number values.
					if (options.FilterOnChapter < 0 && chapter.Number >= Math.Abs(options.FilterOnChapter)) {
						// Continue iteration.
						continue;
					}
					// Filter on positive chapter number values.
					if (options.FilterOnChapter > 0 && chapter.Number <= options.FilterOnChapter) {
						// Continue iteration.
						continue;
					}
				}
				// Check if the volume is valid.
				if (chapter.Volume != -1) {
					// Filter on negative chapter volume values.
					if (options.FilterOnVolume < 0 && chapter.Volume >= Math.Abs(options.FilterOnVolume)) {
						// Continue iteration.
						continue;
					}
					// Filter on positive chapter volume values.
					if (options.FilterOnVolume > 0 && chapter.Volume <= options.FilterOnVolume) {
						// Continue iteration.
						continue;
					}
				}
				// Return the chapter.
				yield return chapter;
			}
		}

		/// <summary>
		/// Convert the chapter to a file name.
		/// </summary>
		/// <param name="chapter">The chapter.</param>
		/// <param name="seriesTitle">The series title.</param>
		/// <param name="options">The collection of options.</param>
		public static string ToFileName(this IChapter chapter, string seriesTitle, Options options) {
			// Initialize the file name.
			return string.Format(chapter.Volume == -1 ? "{0} #{2}.{3}" : "{0} V{1} #{2}.{3}", seriesTitle, chapter.Volume.ToString("00"), chapter.Number.ToString("000.####"), options.FileExtension.InvalidatePath());
		}
		#endregion
	}
}