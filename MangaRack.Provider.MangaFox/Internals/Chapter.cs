// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MangaRack.Provider.Interfaces;
using TinyHttp;

namespace MangaRack.Provider.MangaFox.Internals
{
    internal class Chapter : IChapter
    {
        #region Constructor

        public Chapter(double? number, string location, string title, string uniqueIdentifier, double? volume)
        {
            Number = number;
            Location = location;
            Title = title;
            UniqueIdentifier = uniqueIdentifier;
            Volume = volume;
        }

        #endregion

        #region IAsync

        public async Task PopulateAsync()
        {
            var response = await Http.GetAsync(Location.EndsWith("1.html") ? Location : Location + "1.html");
            var document = new HtmlDocument();
            document.LoadHtml(response.AsString());
            Children = document.DocumentNode.Descendants("select")
                .Where(x => HtmlEntity.DeEntitize(x.GetAttributeValue("onchange", string.Empty))
                    .Trim()
                    .StartsWith("change_page"))
                .Select(x => x.Descendants("option"))
                .First()
                .Where(x => !HtmlEntity.DeEntitize(x.GetAttributeValue("value", string.Empty)).Trim().Equals("0"))
                .Select(x => new Page(
                    string.Join("/", Location.Split('/').TakeWhile(y => !y.EndsWith(".html")).ToArray()).TrimEnd('/') +
                    "/" +
                    HtmlEntity.DeEntitize(x.GetAttributeValue("value", string.Empty)).Trim() + ".html") as IPage)
                .ToArray();
        }

        #endregion

        #region IChapter

        public IEnumerable<IPage> Children { get; private set; }
        public string Location { get; private set; }
        public double? Number { get; private set; }
        public string Title { get; private set; }
        public string UniqueIdentifier { get; private set; }
        public double? Volume { get; private set; }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            foreach (var child in Children)
            {
                if (child == null) continue;
                child.Dispose();
            }
        }

        #endregion
    }
}