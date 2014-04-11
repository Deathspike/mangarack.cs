// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using TinyHttp;

namespace MangaRack.Provider.MangaFox {
	/// <summary>
	/// Represents a MangaFox series.
	/// </summary>
	sealed class Series : ISeries {
		#region Abstract
		/// <summary>
		/// Populate each artist.
		/// </summary>
		private HtmlDocument _Artists {
			set {
				// Find each anchor element ...
				Artists = value.DocumentNode.Descendants("a")
					// ... with a reference starting with the appropriate address ...
					.Where(x => HtmlEntity.DeEntitize(x.GetAttributeValue("href", string.Empty)).Trim().StartsWith("/search/artist/"))
					// ... select the text ...
					.Select(x => HtmlEntity.DeEntitize(x.InnerText).Trim())
					// ... and create an array.
					.ToArray();
			}
		}

		/// <summary>
		/// Populate each author.
		/// </summary>
		private HtmlDocument _Authors {
			set {
				// Find each anchor element ...
				Authors = value.DocumentNode.Descendants("a")
					// ... with a reference starting with the appropriate address ...
					.Where(x => HtmlEntity.DeEntitize(x.GetAttributeValue("href", string.Empty)).Trim().StartsWith("/search/author/"))
					// ... select the text ...
					.Select(x => HtmlEntity.DeEntitize(x.InnerText).Trim())
					// ... and create an array.
					.ToArray();
			}
		}

		/// <summary>
		/// Populate each child.
		/// </summary>
		private HtmlDocument _Children {
			set {
				// Initialize the processed number and volume.
				double ProcessedNumber, ProcessedVolume;
				// Find each header element ...
				Children = value.DocumentNode.Descendants("h3")
					// ... with a class indicating a volume ...
					.Where(x => HtmlEntity.DeEntitize(x.GetAttributeValue("class", string.Empty)).Trim().Split(' ').Contains("volume"))
					// ... select each valid volume ...
					.Select(x => new { Match = Regex.Match(HtmlEntity.DeEntitize(x.FirstChild.InnerText).Trim(), @"^Volume\s(?<Volume>.+)$", RegexOptions.IgnoreCase), ChapterListing = x.ParentNode.NextSibling })
					// ... where the previous match was successful ...
					.Where(x => x.Match.Success)
					// ... select each chapter from each volume ...
					.SelectMany(x => x.ChapterListing.Descendants("a")
						// ... where the chapter is valid ...
						.Where(y => HtmlEntity.DeEntitize(y.GetAttributeValue("href", string.Empty)).Trim().Contains("/manga/"))
						// ... and select each chapter with a number ...
						.Select(y => new Chapter(double.TryParse(Regex.Match(HtmlEntity.DeEntitize(y.InnerText).Trim(), "(?<Number>[0-9.]+)$", RegexOptions.IgnoreCase).Groups["Number"].Value, out ProcessedNumber) ? ProcessedNumber : -1,
							// ... with a title ...
							y.ParentNode.Descendants("span")
								// ... with a class indicating a title ...
								.Where(z => HtmlEntity.DeEntitize(z.GetAttributeValue("class", string.Empty)).Trim().Split(' ').Contains("title"))
								// ... select the text ...
								.Select(z => HtmlEntity.DeEntitize(z.InnerText).Trim())
								// ... use the first or default ...
								.FirstOrDefault(),
							// ... with an unique identifier ...
							HtmlEntity.DeEntitize(y.GetAttributeValue("href", string.Empty)).Trim(),
							// ... with a volume.
							double.TryParse(x.Match.Groups["Volume"].Value, out ProcessedVolume) ? ProcessedVolume : -1) as IChapter))
					// ... reverse the order ...
					.Reverse()
					// ... and create an array.
					.ToArray();
			}
		}

		/// <summary>
		/// Populate each genre.
		/// </summary>
		private HtmlDocument _Genres {
			set {
				// Find each anchor element ...
				Genres = value.DocumentNode.Descendants("a")
					// ... with a reference starting with the appropriate address ...
					.Where(x => HtmlEntity.DeEntitize(x.GetAttributeValue("href", string.Empty)).Trim().StartsWith("?/search/genres/"))
					// ... select the text ...
					.Select(x => HtmlEntity.DeEntitize(x.InnerText).Trim())
					// ... and create an array.
					.ToArray();
			}
		}

		/// <summary>
		/// Populate the summary.
		/// </summary>
		private HtmlDocument _Summary {
			set {
				// Find each paragraph element ...
				Summary = string.Join(" ", value.DocumentNode.Descendants("p")
					// ... with a class indicating a summary ...
					.Where(x => HtmlEntity.DeEntitize(x.GetAttributeValue("class", string.Empty)).Trim().Split(' ').Contains("summary"))
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
		/// Populate the title.
		/// </summary>
		private HtmlDocument _Title {
			set {
				// Find each title element ...
				Title = value.DocumentNode.Descendants("title")
					// ... with a regular expression to match the content ...
					.Select(x => new { Match = Regex.Match(HtmlEntity.DeEntitize(x.InnerText).Trim(), @"^(?<Title>.+)(\s+)Manga(\s+)-", RegexOptions.IgnoreCase) })
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
		/// Initialize a new instance of the Series class.
		/// </summary>
		/// <param name="UniqueIdentifier">The unique identifier.</param>
		public Series(string UniqueIdentifier) {
			// Set the unique identifier.
			this.UniqueIdentifier = UniqueIdentifier;
		}

		/// <summary>
		/// Initialize a new instance of the Series class.
		/// </summary>
		/// <param name="UniqueIdentifier">The unique identifier.</param>
		/// <param name="Title">The title.</param>
		public Series(string UniqueIdentifier, string Title)
			: this(UniqueIdentifier) {
			// Set the title.
			this.Title = Title;
		}
		#endregion

		#region IAsync
		/// <summary>
		/// Populate asynchronously.
		/// </summary>
		/// <param name="Done">The callback.</param>
		public void Populate(Action<ISeries> Done) {
			// Get the document.
			Http.Get(UniqueIdentifier, (Response) => {
				// Initialize a new instance of the HtmlDocument class.
				HtmlDocument HtmlDocument = new HtmlDocument();
				// Load the document.
				HtmlDocument.LoadHtml(Response.AsString());
				// Iterate through each property to populate the series without retaining the document.
				foreach (PropertyInfo PropertyInfo in GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic).Where(x => x.CanWrite)) {
					// Set the value, causing it to populate this property.
					PropertyInfo.SetValue(this, HtmlDocument, null);
				}
				// Find each image element ..
				Http.Get(HtmlDocument.DocumentNode.Descendants("img")
					// ... with a reference containing a preview image ...
					.Where(x => HtmlEntity.DeEntitize(x.GetAttributeValue("src", string.Empty)).Contains("cover.jpg"))
					// ... select the reference attribute ...
					.Select(x => HtmlEntity.DeEntitize(x.Attributes["src"].Value))
					// ... download the first preview image ...
					.First(), (ImageResponse) => {
						// ... and set the image for the bytes.
						PreviewImage = ImageResponse.AsBinary();
						// Invoke the handler indicating the initialization is completed.
						Done(this);
					});
			});
		}
		#endregion

		#region IDisposable
		/// <summary>
		/// Dispose of the object.
		/// </summary>
		public void Dispose() {
			// Check if the children are valid.
			if (Children != null) {
				// Iterate through each child.
				foreach (IChapter Child in Children) {
					// Dispose of the object.
					Child.Dispose();
				}
				// Remove the children.
				Children = null;
			}
			// Check if the results are valid.
			if (PreviewImage != null) {
				// Remove the preview image.
				PreviewImage = null;
			}
		}
		#endregion

		#region ISeries
		/// <summary>
		/// Contains each artist.
		/// </summary>
		public IEnumerable<string> Artists { get; private set; }

		/// <summary>
		/// Contains each author.
		/// </summary>
		public IEnumerable<string> Authors { get; private set; }

		/// <summary>
		/// Contains each child.
		/// </summary>
		public IEnumerable<IChapter> Children { get; private set; }

		/// <summary>
		/// Contains each genre.
		/// </summary>
		public IEnumerable<string> Genres { get; private set; }

		/// <summary>
		/// Contains the preview image.
		/// </summary>
		public byte[] PreviewImage { get; private set; }

		/// <summary>
		/// Contains the summary.
		/// </summary>
		public string Summary { get; private set; }

		/// <summary>
		/// Contains the title.
		/// </summary>
		public string Title { get; private set; }

		/// <summary>
		/// Contains the unique identifier.
		/// </summary>
		public string UniqueIdentifier { get; private set; }
		#endregion
	}
}