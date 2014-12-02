// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using MangaRack.Provider.Abstract;
using TinyHttp;

namespace MangaRack.Provider.Batoto.Concrete {
    /// <summary>
    /// Represents a Batoto search.
    /// </summary>
    internal sealed class Search : ISearch {
        #region Constructor
        /// <summary>
        /// Initialize a new instance of the Search class.
        /// </summary>
        /// <param name="input">The input.</param>
        public Search(string input) {
            Input = input;
        }
        #endregion

        #region IAsync
        /// <summary>
        /// Populate asynchronously.
        /// </summary>
        /// <param name="done">The callback.</param>
        public void Populate(Action<ISearch> done) {
            Http.Get(Provider.Domain + "search?name=" + Uri.EscapeDataString(Input), response => {
                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(response.AsString());
                Children = htmlDocument.DocumentNode.Descendants("a")
                    .Where(x => HtmlEntity.DeEntitize(x.GetAttributeValue("href", string.Empty)).Trim().Contains("/comic/"))
                    .Select(x => new Series(HtmlEntity.DeEntitize(x.Attributes["href"].Value).Trim(), HtmlEntity.DeEntitize(x.InnerText).Trim()) as ISeries)
                    .ToArray();
                done(this);
            });
        }
        #endregion

        #region IDisposable
        /// <summary>
        /// Dispose of the object.
        /// </summary>
        public void Dispose() {
            if (Children == null) return;
            foreach (var child in Children) {
                child.Dispose();
            }
            Children = null;
        }
        #endregion

        #region ISearch
        /// <summary>
        /// Contains each child.
        /// </summary>
        public IEnumerable<ISeries> Children { get; private set; }

        /// <summary>
        /// Contains the input.
        /// </summary>
        public string Input { get; private set; }
        #endregion
    }
}