// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using MangaRack.Provider.Interfaces;

namespace MangaRack.Provider.Contracts
{
    // ReSharper disable once InconsistentNaming
    [ContractClassFor(typeof (IProvider))]
    internal abstract class IProviderContract : IProvider
    {
        #region Implementation of IDisposable

        public void Dispose()
        {
        }

        #endregion

        #region Implementation of IProvider:Methods

        public ISeries Open(string location)
        {
            Contract.Requires<ArgumentNullException>(location != null);
            return null;
        }

        public Task<ISearch> SearchAsync(string input)
        {
            Contract.Requires<ArgumentNullException>(input != null);
            Contract.Ensures(Contract.Result<Task<ISearch>>() != null);
            return null;
        }

        #endregion

        #region Implementation of IProvider:Properties

        public string Location
        {
            get
            {
                Contract.Ensures(Contract.Result<string>() != null);
                return null;
            }
        }

        #endregion
    }
}