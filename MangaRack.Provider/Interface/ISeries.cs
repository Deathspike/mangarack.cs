// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System.Collections.Generic;

namespace MangaRack.Provider.Interface
{
    public interface ISeries : IAsync
    {
        #region Properties

        IEnumerable<string> Artists { get; }
        IEnumerable<string> Authors { get; }
        IEnumerable<IChapter> Children { get; }
        IEnumerable<string> Genres { get; }
        string Location { get; }
        byte[] PreviewImage { get; }
        string Summary { get; }
        string Title { get; }

        #endregion
    }
}