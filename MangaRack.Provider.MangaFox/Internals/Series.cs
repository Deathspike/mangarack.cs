// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MangaRack.Provider.Interfaces;
using MangaRack.Provider.MangaFox.Internals;
using TinyHttp;

namespace MangaRack.Provider.MangaFox
{
    /// <summary>
    ///     Represents a MangaFox series.
    /// </summary>
    internal class Series : ISeries
    {
        #region Abstract

        /// <summary>
        ///     Populate each artist.
        /// </summary>
        private HtmlDocument _Artists
        {
            set
            {
                // Find each anchor element ...
                Artists = value.DocumentNode.Descendants("a")
                    // ... with a reference starting with the appropriate address ...
                    .Where(
                        x =>
                            HtmlEntity.DeEntitize(x.GetAttributeValue("href", string.Empty))
                                .Trim()
                                .StartsWith("/search/artist/"))
                    // ... select the text ...
                    .Select(x => HtmlEntity.DeEntitize(x.InnerText).Trim())
                    // ... and create an array.
                    .ToArray();
            }
        }

        /// <summary>
        ///     Populate each author.
        /// </summary>
        private HtmlDocument _Authors
        {
            set
            {
                // Find each anchor element ...
                Authors = value.DocumentNode.Descendants("a")
                    // ... with a reference starting with the appropriate address ...
                    .Where(
                        x =>
                            HtmlEntity.DeEntitize(x.GetAttributeValue("href", string.Empty))
                                .Trim()
                                .StartsWith("/search/author/"))
                    // ... select the text ...
                    .Select(x => HtmlEntity.DeEntitize(x.InnerText).Trim())
                    // ... and create an array.
                    .ToArray();
            }
        }

        /// <summary>
        ///     Populate each child.
        /// </summary>
        private HtmlDocument _Children
        {
            set
            {
                // Initialize the processed number and volume.
                double ProcessedNumber, ProcessedVolume;
                // Find each header element ...
                Children = value.DocumentNode.Descendants("h3")
                    // ... with a class indicating a volume ...
                    .Where(
                        x =>
                            HtmlEntity.DeEntitize(x.GetAttributeValue("class", string.Empty))
                                .Trim()
                                .Split(' ')
                                .Contains("volume"))
                    // ... select each valid volume ...
                    .Select(
                        x =>
                            new
                            {
                                Match =
                                    Regex.Match(HtmlEntity.DeEntitize(x.FirstChild.InnerText).Trim(),
                                        @"^Volume\s(?<Volume>.+)$", RegexOptions.IgnoreCase),
                                ChapterListing = x.ParentNode.NextSibling
                            })
                    // ... where the previous match was successful ...
                    .Where(x => x.Match.Success)
                    // ... select each chapter from each volume ...
                    .SelectMany(x => x.ChapterListing.Descendants("a")
                        // ... where the chapter is valid ...
                        .Where(
                            y =>
                                HtmlEntity.DeEntitize(y.GetAttributeValue("href", string.Empty))
                                    .Trim()
                                    .Contains("/manga/"))
                        // ... and select each chapter with a number ...
                        .Select(
                            y =>
                                new Chapter(
                                    double.TryParse(
                                        Regex.Match(HtmlEntity.DeEntitize(y.InnerText).Trim(), "(?<Number>[0-9.]+)$",
                                            RegexOptions.IgnoreCase).Groups["Number"].Value, out ProcessedNumber)
                                        ? (double?) ProcessedNumber
                                        : null,
                                    // ... with a location ...
                                    HtmlEntity.DeEntitize(y.GetAttributeValue("href", string.Empty)).Trim(),
                                    // ... with a title ...
                                    y.ParentNode.Descendants("span")
                                        // ... with a class indicating a title ...
                                        .Where(
                                            z =>
                                                HtmlEntity.DeEntitize(z.GetAttributeValue("class", string.Empty))
                                                    .Trim()
                                                    .Split(' ')
                                                    .Contains("title"))
                                        // ... select the text ...
                                        .Select(z => HtmlEntity.DeEntitize(z.InnerText).Trim())
                                        // ... use the first or default ...
                                        .FirstOrDefault(),
                                    // ... with a link ...
                                    y.ParentNode.ParentNode.Descendants("a")
                                        // ... with a class indicating an edit ...
                                        .Where(
                                            z =>
                                                HtmlEntity.DeEntitize(z.GetAttributeValue("class", string.Empty))
                                                    .Trim()
                                                    .Split(' ')
                                                    .Contains("edit"))
                                        // ... select the identifier ...
                                        .Select(
                                            z =>
                                                Internals.Provider.Domain +
                                                Regex.Match(
                                                    HtmlEntity.DeEntitize(z.GetAttributeValue("href", string.Empty))
                                                        .Trim(), "chapter_id=(?<UniqueIdentifier>[0-9]+)",
                                                    RegexOptions.IgnoreCase).Groups["UniqueIdentifier"].Value)
                                        // ... use the first or default ...
                                        .FirstOrDefault(),
                                    // ... with a volume.
                                    double.TryParse(x.Match.Groups["Volume"].Value, out ProcessedVolume)
                                        ? (double?) ProcessedVolume
                                        : null) as IChapter))
                    // ... reverse the order ...
                    .Reverse()
                    // ... and create an array.
                    .ToArray();
            }
        }

        /// <summary>
        ///     Populate each genre.
        /// </summary>
        private HtmlDocument _Genres
        {
            set
            {
                // Find each anchor element ...
                Genres = value.DocumentNode.Descendants("a")
                    // ... with a reference starting with the appropriate address ...
                    .Where(
                        x =>
                            HtmlEntity.DeEntitize(x.GetAttributeValue("href", string.Empty))
                                .Trim()
                                .Contains("/search/genres/"))
                    // ... select the text ...
                    .Select(x => HtmlEntity.DeEntitize(x.InnerText).Trim())
                    // ... and create an array.
                    .ToArray();
            }
        }

        /// <summary>
        ///     Populate the summary.
        /// </summary>
        private HtmlDocument _Summary
        {
            set
            {
                // Find each paragraph element ...
                Summary = string.Join(" ", value.DocumentNode.Descendants("p")
                    // ... with a class indicating a summary ...
                    .Where(
                        x =>
                            HtmlEntity.DeEntitize(x.GetAttributeValue("class", string.Empty))
                                .Trim()
                                .Split(' ')
                                .Contains("summary"))
                    // ... select each split summary piece ...
                    .SelectMany(x => Regex.Split(HtmlEntity.DeEntitize(x.InnerText).Trim(), "\n").Select(y => y.Trim()))
                    // ... with a valid addition to the summary ...
                    .Where(x => !x.EndsWith(":") && !Regex.Match(x, @"^From\s+(.*)$", RegexOptions.IgnoreCase).Success)
                    // ... skip until the first piece of text ...
                    .SkipWhile(x => string.IsNullOrEmpty(x.Trim()))
                    // ... until another blank space is encountered ...
                    .TakeWhile(x => !string.IsNullOrEmpty(x.Trim()))
                    // ... selecting an array to be joined as a summary.
                    .ToArray());
            }
        }

        /// <summary>
        ///     Populate the title.
        /// </summary>
        private HtmlDocument _Title
        {
            set
            {
                // Find each title element ...
                Title = value.DocumentNode.Descendants("title")
                    // ... with a regular expression to match the content ...
                    .Select(
                        x =>
                            new
                            {
                                Match =
                                    Regex.Match(HtmlEntity.DeEntitize(x.InnerText).Trim(),
                                        @"^(?<Title>.+)(\s+)Manga(\s+)-", RegexOptions.IgnoreCase)
                            })
                    // ... where a match was found ...
                    .Where(x => x.Match.Success)
                    // ... select the title ...
                    .Select(x => x.Match.Groups["Title"].Value)
                    // ... and use the first or default.
                    .FirstOrDefault();
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        ///     Initialize a new instance of the Series class.
        /// </summary>
        /// <param name="location">The location.</param>
        public Series(string location)
        {
            // Set the location.
            Location = location;
        }

        /// <summary>
        ///     Initialize a new instance of the Series class.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="title">The title.</param>
        public Series(string location, string title)
            : this(location)
        {
            // Set the title.
            Title = title;
        }

        #endregion

        #region IAsync

        /// <summary>
        ///     Populate asynchronously.
        /// </summary>
        public async Task PopulateAsync()
        {
            // Get the document.
            var response = await Http.GetAsync(Location);
            // Initialize a new instance of the HtmlDocument class.
            var htmlDocument = new HtmlDocument();
            // Load the document.
            htmlDocument.LoadHtml(response.AsString());
            // Iterate through each property to populate the series without retaining the document.
            foreach (
                var propertyInfo in
                    GetType()
                        .GetTypeInfo()
                        .DeclaredProperties.Where(x => x.PropertyType == typeof (HtmlDocument) && x.CanWrite))
            {
                // Set the value, causing it to populate this property.
                propertyInfo.SetValue(this, htmlDocument, null);
            }
            // Find each image element ..
            var imageResponse = await Http.GetAsync(htmlDocument.DocumentNode.Descendants("img")
                // ... with a reference containing a preview image ...
                .Where(x => HtmlEntity.DeEntitize(x.GetAttributeValue("src", string.Empty)).Contains("cover.jpg"))
                // ... select the reference attribute ...
                .Select(x => HtmlEntity.DeEntitize(x.Attributes["src"].Value))
                // ... download the first preview image ...
                .First());
            // ... and set the image for the bytes.
            PreviewImage = imageResponse.AsBinary();
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
            // Check if the results are valid.
            if (PreviewImage != null)
            {
                // Remove the preview image.
                PreviewImage = null;
            }
        }

        #endregion

        #region ISeries

        /// <summary>
        ///     Contains each artist.
        /// </summary>
        public IEnumerable<string> Artists { get; private set; }

        /// <summary>
        ///     Contains each author.
        /// </summary>
        public IEnumerable<string> Authors { get; private set; }

        /// <summary>
        ///     Contains each child.
        /// </summary>
        public IEnumerable<IChapter> Children { get; private set; }

        /// <summary>
        ///     Contains each genre.
        /// </summary>
        public IEnumerable<string> Genres { get; private set; }

        /// <summary>
        ///     Contains the location.
        /// </summary>
        public string Location { get; private set; }

        /// <summary>
        ///     Contains the preview image.
        /// </summary>
        public byte[] PreviewImage { get; private set; }

        /// <summary>
        ///     Contains the summary.
        /// </summary>
        public string Summary { get; private set; }

        /// <summary>
        ///     Contains the title.
        /// </summary>
        public string Title { get; private set; }

        #endregion
    }
}