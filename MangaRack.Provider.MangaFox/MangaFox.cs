// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using MangaRack.Provider.Interfaces;

namespace MangaRack.Provider.MangaFox
{
    public class MangaFox : IProvider
    {
        private readonly IProvider _provider;

        #region Constructor

        public MangaFox()
        {
            _provider = new Internals.Provider();
        }

        #endregion

        #region Contract

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(_provider != null);
        }

        #endregion

        #region Implementation of IProvider:Methods

        public ISeries Open(string location)
        {
            return _provider.Open(location);
        }

        public Task<ISearch> SearchAsync(string input)
        {
            return _provider.SearchAsync(input);
        }

        #endregion

        #region Implementation of IProvider:Properties

        public string Location
        {
            get { return _provider.Location; }
        }

        #endregion
    }
}