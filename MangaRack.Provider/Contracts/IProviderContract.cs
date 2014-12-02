// ======================================================================
using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using MangaRack.Provider.Interfaces;

namespace MangaRack.Provider.Contracts
{
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
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(location));
            return null;
        }

        public Task<ISearch> SearchAsync(string input)
        {
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(input));
            Contract.Ensures(Contract.Result<ISearch>() != null);
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