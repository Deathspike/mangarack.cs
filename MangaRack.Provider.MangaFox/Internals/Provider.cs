// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
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