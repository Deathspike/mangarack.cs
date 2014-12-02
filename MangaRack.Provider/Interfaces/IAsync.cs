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