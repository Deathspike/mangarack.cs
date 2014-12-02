// ======================================================================
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MangaRack.Provider.Interfaces;
using TinyHttp;

namespace MangaRack.Provider.KissManga.Internals
{
    /// <summary>
    ///     Represents a KissManga search.
    /// </summary>
    internal class Search : ISearch
    {
        #region Constructor

        /// <summary>
        ///     Initialize a new instance of the Search class.
        /// </summary>
        /// <param name="input">The input.</param>
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
            var values = new Dictionary<string, string>();
            values.Add("keyword", Input);
            var response = await Http.PostAsync(Provider.Domain + "/Search/Manga", values);
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(response.AsString());
            Children = htmlDocument.DocumentNode.Descendants("a")
                .Where(
                    x =>
                        Regex.Match(HtmlEntity.DeEntitize(x.GetAttributeValue("href", string.Empty)).Trim(),
                            "^(?!http).*/manga/([^/]+?)/?$", RegexOptions.IgnoreCase).Success)
                .Select(
                    x =>
                        new Series(
                            Provider.Domain + HtmlEntity.DeEntitize(x.Attributes["href"].Value).Trim().TrimStart('/'),
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

        public IEnumerable<ISeries> Children { get; private set; }
        public string Input { get; private set; }

        #endregion
    }
}