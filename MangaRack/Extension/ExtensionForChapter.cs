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
		/// <param name="Chapters">Each chapter.</param>
		/// <param name="Options">The collection of options.</param>
		public static IEnumerable<IChapter> Filter(this IEnumerable<IChapter> Chapters, Options Options) {
			// Iterate through each chapter.
			foreach (IChapter Chapter in Chapters) {
				// Check if the chapter number is valid.
				if (Chapter.Number != -1) {
					// Filter on negative chapter number values.
					if (Options.FilterOnChapter < 0 && Chapter.Number >= Math.Abs(Options.FilterOnChapter)) {
						// Continue iteration.
						continue;
					}
					// Filter on positive chapter number values.
					if (Options.FilterOnChapter > 0 && Chapter.Number <= Options.FilterOnChapter) {
						// Continue iteration.
						continue;
					}
				}
				// Check if the volume is valid.
				if (Chapter.Volume != -1) {
					// Filter on negative chapter volume values.
					if (Options.FilterOnVolume < 0 && Chapter.Volume >= Math.Abs(Options.FilterOnVolume)) {
						// Continue iteration.
						continue;
					}
					// Filter on positive chapter volume values.
					if (Options.FilterOnVolume > 0 && Chapter.Volume <= Options.FilterOnVolume) {
						// Continue iteration.
						continue;
					}
				}
				// Return the chapter.
				yield return Chapter;
			}
		}
		#endregion
	}
}