// ======================================================================
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using MangaRack.Provider.Interfaces;

namespace MangaRack.Provider.Contracts
{
    [ContractClassFor(typeof (IAsync))]
    internal abstract class IAsyncContract : IAsync
    {
        #region Implementation of IAsync

        public Task PopulateAsync()
        {
            Contract.Ensures(Contract.Result<Task>() != null);
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