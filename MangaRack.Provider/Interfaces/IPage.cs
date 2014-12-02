// ======================================================================
namespace MangaRack.Provider.Interfaces
{
    public interface IPage : IAsync
    {
        #region Properties

        byte[] Image { get; }
        string Location { get; }

        #endregion
    }
}