// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using MangaRack.Provider.Contracts;

namespace MangaRack.Provider.Interfaces
{
    [ContractClass(typeof (ISearchContract))]
    public interface ISearch : IAsync
    {
        #region Properties

        IEnumerable<ISeries> Children { get; }
        string Input { get; }

        #endregion
    }
}