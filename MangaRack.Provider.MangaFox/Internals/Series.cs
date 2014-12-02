// ======================================================================
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MangaRack.Provider.Interfaces;
using TinyHttp;

namespace MangaRack.Provider.MangaFox.Internals
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
                Artists = value.DocumentNode.Descendants("a")
                    .Where(
                        x =>
                            HtmlEntity.DeEntitize(x.GetAttributeValue("href", string.Empty))
                                .Trim()
                                .StartsWith("/search/artist/"))
                    .Select(x => HtmlEntity.DeEntitize(x.InnerText).Trim())
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
                Authors = value.DocumentNode.Descendants("a")
                    .Where(
                        x =>
                            HtmlEntity.DeEntitize(x.GetAttributeValue("href", string.Empty))
                                .Trim()
                                .StartsWith("/search/author/"))
                    .Select(x => HtmlEntity.DeEntitize(x.InnerText).Trim())
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
                double ProcessedNumber, ProcessedVolume;
                Children = value.DocumentNode.Descendants("h3")
                    .Where(
                        x =>
                            HtmlEntity.DeEntitize(x.GetAttributeValue("class", string.Empty))
                                .Trim()
                                .Split(' ')
                                .Contains("volume"))
                    .Select(
                        x =>
                            new
                            {
                                Match =
                                    Regex.Match(HtmlEntity.DeEntitize(x.FirstChild.InnerText).Trim(),
                                        @"^Volume\s(?<Volume>.+)$", RegexOptions.IgnoreCase),
                                ChapterListing = x.ParentNode.NextSibling
                            })
                    .Where(x => x.Match.Success)
                    .SelectMany(x => x.ChapterListing.Descendants("a")
                        .Where(
                            y =>
                                HtmlEntity.DeEntitize(y.GetAttributeValue("href", string.Empty))
                                    .Trim()
                                    .Contains("/manga/"))
                        .Select(
                            y =>
                                new Chapter(
                                    double.TryParse(
                                        Regex.Match(HtmlEntity.DeEntitize(y.InnerText).Trim(), "(?<Number>[0-9.]+)$",
                                            RegexOptions.IgnoreCase).Groups["Number"].Value, out ProcessedNumber)
                                        ? (double?) ProcessedNumber
                                        : null,
                                    HtmlEntity.DeEntitize(y.GetAttributeValue("href", string.Empty)).Trim(),
                                    y.ParentNode.Descendants("span")
                                        .Where(
                                            z =>
                                                HtmlEntity.DeEntitize(z.GetAttributeValue("class", string.Empty))
                                                    .Trim()
                                                    .Split(' ')
                                                    .Contains("title"))
                                        .Select(z => HtmlEntity.DeEntitize(z.InnerText).Trim())
                                        .FirstOrDefault(),
                                    y.ParentNode.ParentNode.Descendants("a")
                                        .Where(
                                            z =>
                                                HtmlEntity.DeEntitize(z.GetAttributeValue("class", string.Empty))
                                                    .Trim()
                                                    .Split(' ')
                                                    .Contains("edit"))
                                        .Select(
                                            z =>
                                                Provider.Domain +
                                                Regex.Match(
                                                    HtmlEntity.DeEntitize(z.GetAttributeValue("href", string.Empty))
                                                        .Trim(), "chapter_id=(?<UniqueIdentifier>[0-9]+)",
                                                    RegexOptions.IgnoreCase).Groups["UniqueIdentifier"].Value)
                                        .FirstOrDefault(),
                                    double.TryParse(x.Match.Groups["Volume"].Value, out ProcessedVolume)
                                        ? (double?) ProcessedVolume
                                        : null) as IChapter))
                    .Reverse()
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
                Genres = value.DocumentNode.Descendants("a")
                    .Where(
                        x =>
                            HtmlEntity.DeEntitize(x.GetAttributeValue("href", string.Empty))
                                .Trim()
                                .Contains("/search/genres/"))
                    .Select(x => HtmlEntity.DeEntitize(x.InnerText).Trim())
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
                Summary = string.Join(" ", value.DocumentNode.Descendants("p")
                    .Where(
                        x =>
                            HtmlEntity.DeEntitize(x.GetAttributeValue("class", string.Empty))
                                .Trim()
                                .Split(' ')
                                .Contains("summary"))
                    .SelectMany(x => Regex.Split(HtmlEntity.DeEntitize(x.InnerText).Trim(), "\n").Select(y => y.Trim()))
                    .Where(x => !x.EndsWith(":") && !Regex.Match(x, @"^From\s+(.*)$", RegexOptions.IgnoreCase).Success)
                    .SkipWhile(x => string.IsNullOrEmpty(x.Trim()))
                    .TakeWhile(x => !string.IsNullOrEmpty(x.Trim()))
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
                Title = value.DocumentNode.Descendants("title")
                    .Select(
                        x =>
                            new
                            {
                                Match =
                                    Regex.Match(HtmlEntity.DeEntitize(x.InnerText).Trim(),
                                        @"^(?<Title>.+)(\s+)Manga(\s+)-", RegexOptions.IgnoreCase)
                            })
                    .Where(x => x.Match.Success)
                    .Select(x => x.Match.Groups["Title"].Value)
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
            Title = title;
        }

        #endregion

        #region IAsync

        /// <summary>
        ///     Populate asynchronously.
        /// </summary>
        public async Task PopulateAsync()
        {
            var response = await Http.GetAsync(Location);
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(response.AsString());
            foreach (
                var propertyInfo in
                    GetType()
                        .GetTypeInfo()
                        .DeclaredProperties.Where(x => x.PropertyType == typeof (HtmlDocument) && x.CanWrite))
            {
                propertyInfo.SetValue(this, htmlDocument, null);
            }
            var imageResponse = await Http.GetAsync(htmlDocument.DocumentNode.Descendants("img")
                .Where(x => HtmlEntity.DeEntitize(x.GetAttributeValue("src", string.Empty)).Contains("cover.jpg"))
                .Select(x => HtmlEntity.DeEntitize(x.Attributes["src"].Value))
                .First());
            PreviewImage = imageResponse.AsBinary();
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
            if (PreviewImage != null)
            {
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