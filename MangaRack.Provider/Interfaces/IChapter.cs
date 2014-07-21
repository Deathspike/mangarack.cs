// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System.Collections.Generic;

namespace MangaRack.Provider.Interfaces
{
    public interface IChapter : IAsync
    {
        #region Properties

        IEnumerable<IPage> Children { get; }
        string Location { get; }
        double Number { get; }
        string Title { get; }
        string UniqueIdentifier { get; }
        double Volume { get; }

        #endregion
    }
}