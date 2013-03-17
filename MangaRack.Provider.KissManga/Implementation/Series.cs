// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;

namespace MangaRack.Provider.KissManga {
	/// <summary>
	/// Represents a KissManga series.
	/// </summary>
	public sealed class Series : KeyValueStore, ISeries {
		/// <summary>
		/// Contains the document.
		/// </summary>
		private readonly HtmlDocument _HtmlDocument;

		/// <summary>
		/// Contains the preview image.
		/// </summary>
		private Bitmap _PreviewImage;

		/// <summary>
		/// Contains the web client.
		/// </summary>
		private readonly StateWebClient _WebClient;

		/// <summary>
		/// Contains the unique identifier.
		/// </summary>
		private readonly string _UniqueIdentifier;

		#region Constructor
		/// <summary>
		/// Initialize a new instance of the Series class.
		/// </summary>
		/// <param name="UniqueIdentifier">The unique identifier.</param>
		public Series(string UniqueIdentifier) {
			// Initialize the properties.
			if (true) {
				// Initialize a new instance of the HtmlDocument class.
				_HtmlDocument = new HtmlDocument();
				// Set the unique identifier.
				_UniqueIdentifier = UniqueIdentifier;
				// Initialize a new instance of the StateWebClient class.
				_WebClient = new StateWebClient();
			}
			// Initialize the document.
			if (true) {
				// Download the document and load it.
				_HtmlDocument.LoadHtml(_WebClient.DownloadString(UniqueIdentifier));
			}
		}
		#endregion

		#region IDisposable
		/// <summary>
		/// Dispose of the object.
		/// </summary>
		public void Dispose() {
			// Check if the preview image is available.
			if (_PreviewImage != null) {
				// Dispose of the preview image.
				_PreviewImage.Dispose();
			}
			// Dispose of the web client.
			_WebClient.Dispose();
		}
		#endregion

		#region ISeries
		/// <summary>
		/// Contains each artist.
		/// </summary>
		public IEnumerable<string> Artists {
			get {
				// Since there is no distinction between author and artist, return authors.
				return Authors;
			}
		}

		/// <summary>
		/// Contains each author.
		/// </summary>
		public IEnumerable<string> Authors {
			get {
				// Check if the key-value store does not contain the key.
				if (!_Contains(() => Authors)) {
					// Find each anchor element ...
					_Set(() => Authors, _HtmlDocument.DocumentNode.Descendants("a")
						// ... with a reference starting with the appropriate address ...
						.Where(x => Regex.Match(HtmlEntity.DeEntitize(x.GetAttributeValue("href", string.Empty)), @"^/AuthorArtist/", RegexOptions.IgnoreCase).Success)
						// ... and select the text without HTML entities.
						.Select(x => HtmlEntity.DeEntitize(x.InnerText)));
				}
				// Get a value.
				return _Get(() => Authors) as IEnumerable<string>;
			}
		}

		/// <summary>
		/// Contains each chapter.
		/// </summary>
		public IEnumerable<IChapter> Chapters {
			get {
				// Check if the key-value store does not contain the key.
				if (!_Contains(() => Chapters)) {
					// Initialize the processed chapter and volume.
					double ProcessedChapter, ProcessedVolume;
					// Find each anchor element ...
					_Set(() => Chapters, _HtmlDocument.DocumentNode.Descendants("a")
						// ... with a references indicating a chapter ...
						.Where(x => x.GetAttributeValue("href", string.Empty).StartsWith("/Manga/"))
						// ... selecting each valid volume ...
						.Select(x => new { Chapter = x, Match = Regex.Match(HtmlEntity.DeEntitize(x.InnerText).ReplaceWhileWithDigit(".0", ".").RemoveToIncluding(Title).Trim(), @"(\s?Vol\.\s?(?<Volume>[0-9\.]+))?\s?(Ch\.)?\s?(?<Number>([0-9\.]+|Extra|Omake))(\s?-\s?[0-9\.]+)?(\s?v\.?[0-9]+)?(\s?\(?Part\s(?<Part>[0-9]+)\)?)?(\s?(-|\+)\s?)?\s?(Read Onl?ine|:?\s?(?<Title>.+?)?(Read Online)?)$", RegexOptions.IgnoreCase) })
						// ... where the previous match was successful ...
						.Where(x => x.Match.Success && x.Chapter.ParentNode != null && x.Chapter.ParentNode.Name.Equals("td"))
						// ... selecting a proper type with all relevant information ...
						.Select(x => new Chapter(_WebClient.Referer, x.Chapter, double.TryParse(x.Match.Groups["Volume"].Value, out ProcessedVolume) ? ProcessedVolume : 0, double.TryParse(x.Match.Groups["Number"].Value, out ProcessedChapter) ? ProcessedChapter : -1, x.Match.Groups["Title"].Value) as IChapter)
						// ... in the reverse order ...
						.Reverse());
				}
				// Get a value.
				return _Get(() => Chapters) as IEnumerable<IChapter>;
			}
		}

		/// <summary>
		/// Contains each genre.
		/// </summary>
		public IEnumerable<string> Genres {
			get {
				// Check if the key-value store does not contain the key.
				if (!_Contains(() => Genres)) {
					// Find each anchor element ...
					_Set(() => Genres, _HtmlDocument.DocumentNode.Descendants("a")
						// ... with a reference starting with the appropriate address ...
						.Where(x => Regex.Match(HtmlEntity.DeEntitize(x.GetAttributeValue("href", string.Empty)), @"^/Genre/", RegexOptions.IgnoreCase).Success)
						// ... and select the text without HTML entities.
						.Select(x => HtmlEntity.DeEntitize(x.InnerText)));
				}
				// Get a value.
				return _Get(() => Genres) as IEnumerable<string>;
			}
		}

		/// <summary>
		/// Contains the preview image.
		/// </summary>
		public Bitmap PreviewImage {
			get {
				// Check if the preview image is invalid.
				if (_PreviewImage == null) {
					// Find each image element ...
					_PreviewImage = _WebClient.DownloadData(_HtmlDocument.DocumentNode.Descendants("img")
						// ... with a reference containing a preview image ...
						.Where(x => HtmlEntity.DeEntitize(x.GetAttributeValue("src", string.Empty)).Contains("/Uploads/"))
						// ... select the reference attribute ...
						.Select(x => HtmlEntity.DeEntitize(x.Attributes["src"].Value))
						// ... download the first preview image ...
						.First())
						// ... and create a bitmap from the bytes.
						.Bitmap();
				}
				// Return the preview image.
				return _PreviewImage;
			}
		}

		/// <summary>
		/// Contains the summary.
		/// </summary>
		public string Summary {
			get {
				// Check if the key-value store does not contain the key.
				if (!_Contains(() => Summary)) {
					// Find each table definition element ...
					_Set(() => Summary, _HtmlDocument.DocumentNode.Descendants("span")
						// ... with a reference containing with the appropriate name ...
						.Where(x => x.ParentNode != null && HtmlEntity.DeEntitize(x.InnerText).Equals("Summary:"))
						// .. selecting the text without HTML entities ...
						.Select(x => HtmlEntity.DeEntitize(x.NextElement().InnerText))
						// ... and using the first match.
						.FirstOrDefault());
				}
				// Get a value.
				return _Get(() => Summary) as string;
			}
		}

		/// <summary>
		/// Contains the title.
		/// </summary>
		public string Title {
			get {
				// Check if the key-value store does not contain the key.
				if (!_Contains(() => Title)) {
					// Find each title element ...
					_Set(() => Title, _HtmlDocument.DocumentNode.Descendants("title")
						// ... with a regular expression to match the content ...
						.Select(x => new { Match = Regex.Match(HtmlEntity.DeEntitize(x.InnerText), @"^\s+?(?<Title>.+)\s+Manga\s+|", RegexOptions.Multiline | RegexOptions.IgnoreCase) })
						// ... where a match was found ...
						.Where(x => x.Match.Success)
						// ... select the title without HTML entities ...
						.Select(x => x.Match.Groups["Title"].Value.Trim())
						// ... and use the first or default.
						.FirstOrDefault());
				}
				// Get a value.
				return _Get(() => Title) as string;
			}
		}

		/// <summary>
		/// Contains the unique identifier.
		/// </summary>
		public string UniqueIdentifier {
			get {
				// Return the unique identifier.
				return _UniqueIdentifier;
			}
		}
		#endregion
	}
}