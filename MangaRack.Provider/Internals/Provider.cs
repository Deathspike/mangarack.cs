// ======================================================================
using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using MangaRack.Provider.Interfaces;

namespace MangaRack.Provider.Internals
{
    internal class Provider : IProvider
    {
        private readonly IProvider _provider;

        #region Constructor

        public Provider(IProvider provider)
        {
            Contract.Requires<ArgumentNullException>(provider != null);
            _provider = provider;
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
            var series = _provider.Open(location);
            return series == null ? null : new Series(series);
        }

        public async Task<ISearch> SearchAsync(string input)
        {
            return new Search(await _provider.SearchAsync(input));
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