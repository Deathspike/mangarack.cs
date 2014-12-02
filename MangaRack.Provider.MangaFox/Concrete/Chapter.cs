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

namespace MangaRack.Provider.MangaFox.Concrete {
    /// <summary>
    /// Represents a MangaFox chapter.
    /// </summary>
    internal sealed class Chapter : IChapter {
        #region Constructor
        /// <summary>
        /// Initialize a new instance of the Chapter class.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <param name="location">The location.</param>
        /// <param name="title">The title.</param>
        /// <param name="uniqueIdentifier">The unique identifier.</param>
        /// <param name="volume">The volume.</param>
        public Chapter(double number, string location, string title, string uniqueIdentifier, double volume) {
            Number = number;
            Location = location;
            Title = title;
            UniqueIdentifier = uniqueIdentifier;
            Volume = volume;
        }
        #endregion

        #region IAsync
        /// <summary>
        /// Populate asynchronously.
        /// </summary>
        /// <param name="done">The callback.</param>
        public void Populate(Action<IChapter> done) {
            Http.Get(Location.EndsWith("1.html") ? Location : Location + "1.html", response => {
                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(response.AsString());
                Children = htmlDocument.DocumentNode.Descendants("select")
                    .Where(x => HtmlEntity.DeEntitize(x.GetAttributeValue("onchange", string.Empty)).Trim().StartsWith("change_page"))
                    .Select(x => x.Descendants("option"))
                    .First()
                    .Where(x => !HtmlEntity.DeEntitize(x.GetAttributeValue("value", string.Empty)).Trim().Equals("0"))
                    .Select(x => new Page(string.Join("/", Location.Split('/').TakeWhile(y => !y.EndsWith(".html")).ToArray()).TrimEnd('/') + "/" + HtmlEntity.DeEntitize(x.GetAttributeValue("value", string.Empty)).Trim() + ".html") as IPage)
                    .ToArray();
                done(this);
            });
        }
        #endregion

        #region IChapter
        /// <summary>
        /// Contains each child.
        /// </summary>
        public IEnumerable<IPage> Children { get; private set; }

        /// <summary>
        /// Contains the location.
        /// </summary>
        public string Location { get; private set; }

        /// <summary>
        /// Contains the number.
        /// </summary>
        public double Number { get; private set; }

        /// <summary>
        /// Contains the title.
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Contains the unique identifier.
        /// </summary>
        public string UniqueIdentifier { get; private set; }

        /// <summary>
        /// Contains the volume.
        /// </summary>
        public double Volume { get; private set; }
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
    }
}