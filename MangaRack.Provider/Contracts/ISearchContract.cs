// ======================================================================
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using MangaRack.Provider.Interfaces;

namespace MangaRack.Provider.Contracts
{
    [ContractClassFor(typeof (ISearch))]
    internal abstract class ISearchContract : ISearch
    {
        #region Implementation of IAsync

        public Task PopulateAsync()
        {
            return null;
        }

        #endregion

        #region Implementation of IDisposable

        public void Dispose()
        {
        }

        #endregion

        #region Implementation of ISearch

        public IEnumerable<ISeries> Children
        {
            get
            {
                return null;
            }
        }

        public string Input
        {
            get
            {
                Contract.Ensures(!string.IsNullOrWhiteSpace(Contract.Result<string>()));
                return null;
            }
        }

        #endregion
    }
}