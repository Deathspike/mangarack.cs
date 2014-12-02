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
    /// Represents a Batoto chapter.
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
            var location = Location;
            var next = null as Action;
            next = () => {
                Http.Get(location, response => {
                    var htmlDocument = new HtmlDocument();
                    HtmlNode htmlNode;
                    htmlDocument.LoadHtml(response.AsString());
                    if ((htmlNode = htmlDocument.DocumentNode.Descendants("a").FirstOrDefault(x => HtmlEntity.DeEntitize(x.GetAttributeValue("href", string.Empty)).Equals("?supress_webtoon=t"))) != null) {
                        location = location + HtmlEntity.DeEntitize(htmlNode.Attributes["href"].Value).Trim();
                        next();
                        return;
                    }
                    Children = htmlDocument.DocumentNode.Descendants("select")
                        .Where(x => HtmlEntity.DeEntitize(x.GetAttributeValue("name", string.Empty)).Trim().Equals("page_select"))
                        .Select(x => x.Descendants("option"))
                        .First()
                        .Where(x => HtmlEntity.DeEntitize(x.GetAttributeValue("value", string.Empty)).Trim().Contains("/read/"))
                        .Select(x => new Page(HtmlEntity.DeEntitize(x.GetAttributeValue("value", string.Empty)).Trim()) as IPage)
                        .ToArray();
                    done(this);
                });
            };
            next();
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