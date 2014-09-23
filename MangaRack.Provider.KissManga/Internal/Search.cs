// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MangaRack.Provider.Interface;
using TinyHttp;

namespace MangaRack.Provider.KissManga.Internal
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
            // Set the input.
            Input = input;
        }

        #endregion

        #region IAsync

        /// <summary>
        ///     Populate asynchronously.
        /// </summary>
        public async Task PopulateAsync()
        {
            // Initialize each value.
            var values = new Dictionary<string, string>();
            // Add the keyword.
            values.Add("keyword", Input);
            // Get the document.
            var response = await Http.PostAsync(Provider.Domain + "/Search/Manga", values);
            // Initialize a new instance of the HtmlDocument class.
            var htmlDocument = new HtmlDocument();
            // Load the document.
            htmlDocument.LoadHtml(response.AsString());
            // Find each anchor element ...
            Children = htmlDocument.DocumentNode.Descendants("a")
                // ... with a references indicating a series ...
                .Where(
                    x =>
                        Regex.Match(HtmlEntity.DeEntitize(x.GetAttributeValue("href", string.Empty)).Trim(),
                            "^(?!http).*/manga/([^/]+?)/?$", RegexOptions.IgnoreCase).Success)
                // ... select the results ...
                .Select(
                    x =>
                        new Series(
                            Provider.Domain + HtmlEntity.DeEntitize(x.Attributes["href"].Value).Trim().TrimStart('/'),
                            HtmlEntity.DeEntitize(x.InnerText).Trim()) as ISeries)
                // ... and create an array.
                .ToArray();
        }

        #endregion

        #region IDisposable

        /// <summary>
        ///     Dispose of the object.
        /// </summary>
        public void Dispose()
        {
            // Check if the children are valid.
            if (Children != null)
            {
                // Iterate through each child.
                foreach (var child in Children)
                {
                    // Dispose of the object.
                    child.Dispose();
                }
                // Remove the children.
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