// ======================================================================
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MangaRack.Provider.Interfaces;

namespace MangaRack.Provider.MangaFox.Internals
{
    internal class Provider : IProvider
    {
        #region Statics

        public static string Domain
        {
            get { return "http://mangafox.me/"; }
        }

        #endregion

        #region Implementation of IProvider:Methods

        public ISeries Open(string location)
        {
            return Regex.Match(location, @"^http://mangafox\.(com|me)/manga/(.*)/$", RegexOptions.IgnoreCase).Success
                ? new Series(location)
                : null;
        }

        public async Task<ISearch> SearchAsync(string input)
        {
            var search = new Search(input);

            await search.PopulateAsync();

            return search;
        }

        #endregion

        #region Implementation of IProvider:Properties

        public string Location
        {
            get { return Domain; }
        }

        #endregion
    }
}