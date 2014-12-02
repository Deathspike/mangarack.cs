// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System;
using System.Collections.Generic;
using MangaRack.Concrete;
using MangaRack.Provider;
using MangaRack.Provider.Abstract;

namespace MangaRack.Extension {
    /// <summary>
    /// Represents the class providing extensions for the IChapter interface.
    /// </summary>
    internal static class ExtensionForChapter {
        #region Methods
        /// <summary>
        /// Filter the chapters according to the chapter and volume filter.
        /// </summary>
        /// <param name="chapters">Each chapter.</param>
        /// <param name="options">The collection of options.</param>
        public static IEnumerable<IChapter> Filter(this IEnumerable<IChapter> chapters, Options options) {
            foreach (var chapter in chapters) {
                if (chapter.Number != -1) {
                    if (options.FilterOnChapter < 0 && chapter.Number >= Math.Abs(options.FilterOnChapter)) {
                        continue;
                    }
                    if (options.FilterOnChapter > 0 && chapter.Number <= options.FilterOnChapter) {
                        continue;
                    }
                }
                if (chapter.Volume != -1) {
                    if (options.FilterOnVolume < 0 && chapter.Volume >= Math.Abs(options.FilterOnVolume)) {
                        continue;
                    }
                    if (options.FilterOnVolume > 0 && chapter.Volume <= options.FilterOnVolume) {
                        continue;
                    }
                }
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
            return string.Format(chapter.Volume == -1 ? "{0} #{2}.{3}" : "{0} V{1} #{2}.{3}", seriesTitle, chapter.Volume.ToString("00"), chapter.Number.ToString("000.####"), options.FileExtension.InvalidatePath());
        }
        #endregion
    }
}