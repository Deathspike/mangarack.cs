// ======================================================================
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using MangaRack.Provider.Contracts;

namespace MangaRack.Provider.Interfaces
{
    [ContractClass(typeof (ISearchContract))]
    public interface ISearch : IAsync
    {
        #region Properties

        IEnumerable<ISeries> Children { get; }
        string Input { get; }

        #endregion
    }
}