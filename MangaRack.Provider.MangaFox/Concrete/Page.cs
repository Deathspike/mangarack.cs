// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System;
using System.Linq;
using System.Net;
using HtmlAgilityPack;
using MangaRack.Provider.Abstract;
using TinyHttp;

namespace MangaRack.Provider.MangaFox.Concrete {
    /// <summary>
    /// Represents a MangaFox page.
    /// </summary>
    internal sealed class Page : IPage {
        #region Constructor
        /// <summary>
        /// Initialize a new instance of the Page class.
        /// </summary>
        /// <param name="location">The location.</param>
        public Page(string location) {
            Location = location;
        }
        #endregion

        #region IAsync
        /// <summary>
        /// Populate asynchronously.
        /// </summary>
        /// <param name="done">The callback.</param>
        public void Populate(Action<IPage> done) {
            Http.Get(Location, htmlResponse => {
                var htmlDocument = new HtmlDocument();
                HtmlNode htmlNode;
                htmlDocument.LoadHtml(htmlResponse.AsString());
                if ((htmlNode = htmlDocument.DocumentNode.Descendants("meta").FirstOrDefault(x => HtmlEntity.DeEntitize(x.GetAttributeValue("property", string.Empty)).Equals("og:image"))) != null) {
                    var address = HtmlEntity.DeEntitize(htmlNode.GetAttributeValue("content", string.Empty)).Trim()
                        .Replace("thumbnails/mini.", "compressed/")
                        .Replace("http://l.", "http://l.");
                    Http.Get(address, imageResponse => {
                        if (imageResponse == null || imageResponse.StatusCode != HttpStatusCode.OK) {
                            Http.Get(address.Replace("http://z.", "http://l."), alternativeImageResponse => {
                                Image = alternativeImageResponse.AsBinary();
                                done(this);
                            });
                        } else {
                            Image = imageResponse.AsBinary();
                            done(this);
                        }
                    });
                } else {
                    done(this);
                }
            });
        }
        #endregion

        #region IDisposable
        /// <summary>
        /// Dispose of the object.
        /// </summary>
        public void Dispose() {
            Image = null;
        }
        #endregion

        #region IPage
        /// <summary>
        /// Contains the image.
        /// </summary>
        public byte[] Image { get; private set; }

        /// <summary>
        /// Contains the location.
        /// </summary>
        public string Location { get; private set; }
        #endregion
    }
}