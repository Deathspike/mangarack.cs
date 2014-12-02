// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System.Collections.Generic;

namespace MangaRack.Provider.Abstract {
    /// <summary>
    /// Represents a series.
    /// </summary>
    public interface ISeries : IAsync<ISeries> {
        #region Properties
        /// <summary>
        /// Contains each artist.
        /// </summary>
        IEnumerable<string> Artists { get; }

        /// <summary>
        /// Contains each author.
        /// </summary>
        IEnumerable<string> Authors { get; }

        /// <summary>
        /// Contains each child.
        /// </summary>
        IEnumerable<IChapter> Children { get; }

        /// <summary>
        /// Contains each genre.
        /// </summary>
        IEnumerable<string> Genres { get; }

        /// <summary>
        /// Contains the location.
        /// </summary>
        string Location { get; }

        /// <summary>
        /// Contains the preview image.
        /// </summary>
        byte[] PreviewImage { get; }

        /// <summary>
        /// Contains the summary.
        /// </summary>
        string Summary { get; }

        /// <summary>
        /// Contains the title.
        /// </summary>
        string Title { get; }
        #endregion
    }
}