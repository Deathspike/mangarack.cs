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