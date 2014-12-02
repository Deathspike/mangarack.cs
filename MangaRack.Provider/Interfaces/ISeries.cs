// ======================================================================
using System.Collections.Generic;

namespace MangaRack.Provider.Interfaces
{
    public interface ISeries : IAsync
    {
        #region Properties

        IEnumerable<string> Artists { get; }
        IEnumerable<string> Authors { get; }
        IEnumerable<IChapter> Children { get; }
        IEnumerable<string> Genres { get; }
        string Location { get; }
        byte[] PreviewImage { get; }
        string Summary { get; }
        string Title { get; }

        #endregion
    }
}