// ======================================================================
using System.Collections.Generic;

namespace MangaRack.Provider.Interfaces
{
    public interface IChapter : IAsync
    {
        #region Properties

        IEnumerable<IPage> Children { get; }
        string Location { get; }
        double? Number { get; }
        string Title { get; }
        string UniqueIdentifier { get; }
        double? Volume { get; }

        #endregion
    }
}