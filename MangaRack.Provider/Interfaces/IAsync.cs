// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using MangaRack.Provider.Contracts;

namespace MangaRack.Provider.Interfaces
{
    [ContractClass(typeof (IAsyncContract))]
    public interface IAsync : IDisposable
    {
        #region Methods

        Task PopulateAsync();

        #endregion
    }
}