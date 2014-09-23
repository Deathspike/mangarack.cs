// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MangaRack.Provider.Interfaces;
using TinyHttp;

namespace MangaRack.Provider.MangaFox.Internals
{
    internal class Page : IPage
    {
        #region Constructor

        public Page(string location)
        {
            Location = location;
        }

        #endregion

        #region IAsync

        public async Task PopulateAsync()
        {
            var htmlResponse = await Http.GetAsync(Location);
            var htmlDocument = new HtmlDocument();
            var htmlNode = null as HtmlNode;

            htmlDocument.LoadHtml(htmlResponse.AsString());
            if ((htmlNode = htmlDocument.DocumentNode.Descendants("meta")
                .FirstOrDefault(
                    x => HtmlEntity.DeEntitize(x.GetAttributeValue("property", string.Empty)).Equals("og:image"))) !=
                null)
            {
                var address = HtmlEntity.DeEntitize(htmlNode.GetAttributeValue("content", string.Empty)).Trim()
                    .Replace("thumbnails/mini.", "compressed/")
                    .Replace("http://l.", "http://l.");

                var imageResponse = await Http.GetAsync(address);
                if (imageResponse == null || imageResponse.StatusCode != HttpStatusCode.OK)
                {
                    var alternativeImageResponse = await Http.GetAsync(address.Replace("http://z.", "http://l."));
                    Image = alternativeImageResponse.AsBinary();
                }
                else
                {
                    Image = imageResponse.AsBinary();
                }
            }
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            Image = null;
        }

        #endregion

        #region IPage

        public byte[] Image { get; private set; }
        public string Location { get; private set; }

        #endregion
    }
}