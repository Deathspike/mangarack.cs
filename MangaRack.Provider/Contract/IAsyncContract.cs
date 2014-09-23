// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using MangaRack.Provider.Interface;

namespace MangaRack.Provider.Contract
{
    // ReSharper disable once InconsistentNaming
    [ContractClassFor(typeof (IAsync))]
    internal abstract class IAsyncContract : IAsync
    {
        #region Implementation of IAsync

        public Task PopulateAsync()
        {
            System.Diagnostics.Contracts.Contract.Ensures(System.Diagnostics.Contracts.Contract.Result<Task>() != null);
            return null;
        }

        #endregion

        #region Implementation of IDisposable

        public void Dispose()
        {
        }

        #endregion
    }
}