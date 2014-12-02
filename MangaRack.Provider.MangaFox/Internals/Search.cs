// ======================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MangaRack.Provider.Interfaces;
using TinyHttp;

namespace MangaRack.Provider.MangaFox.Internals
{
    /// <summary>
    ///     Represents a MangaFox search.
    /// </summary>
    internal class Search : ISearch
    {
        #region Constructor

        public Search(string input)
        {
            Input = input;
        }

        #endregion

        #region IAsync

        /// <summary>
        ///     Populate asynchronously.
        /// </summary>
        public async Task PopulateAsync()
        {
            var response =
                await
                    Http.GetAsync(Provider.Domain + "search.php?advopts=1&name=" + Uri.EscapeDataString(Input));
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(response.AsString());
            Children = htmlDocument.DocumentNode.Descendants("a")
                .Where(
                    x =>
                        Regex.Match(HtmlEntity.DeEntitize(x.GetAttributeValue("href", string.Empty)).Trim(),
                            "/manga/([^/]+?)/?$", RegexOptions.IgnoreCase).Success)
                .Select(
                    x =>
                        new Series(HtmlEntity.DeEntitize(x.Attributes["href"].Value).Trim(),
                            HtmlEntity.DeEntitize(x.InnerText).Trim()) as ISeries)
                .ToArray();
        }

        #endregion

        #region IDisposable

        /// <summary>
        ///     Dispose of the object.
        /// </summary>
        public void Dispose()
        {
            if (Children != null)
            {
                foreach (var child in Children)
                {
                    child.Dispose();
                }
                Children = null;
            }
        }

        #endregion

        #region ISearch

        /// <summary>
        ///     Contains each child.
        /// </summary>
        public IEnumerable<ISeries> Children { get; private set; }

        /// <summary>
        ///     Contains the input.
        /// </summary>
        public string Input { get; private set; }

        #endregion
    }
}