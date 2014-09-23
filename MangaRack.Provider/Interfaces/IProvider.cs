// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using MangaRack.Provider.Contracts;

namespace MangaRack.Provider.Interfaces
{
    [ContractClass(typeof (IProviderContract))]
    public interface IProvider
    {
        #region Methods

        ISeries Open(string location);
        Task<ISearch> SearchAsync(string input);

        #endregion

        #region Properties

        string Location { get; }

        #endregion
    }
}