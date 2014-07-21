﻿// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System.Threading.Tasks;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using MangaRack.Provider.Interfaces;
using TinyHttp;

namespace MangaRack.Provider.KissManga {
	/// <summary>
	/// Represents a KissManga series.
	/// </summary>
	class Series : ISeries {
		#region Abstract
		/// <summary>
		/// Populate each author.
		/// </summary>
		private HtmlDocument _Authors {
			set {
				// Find each anchor element ...
				Authors = value.DocumentNode.Descendants("a")
					// ... with a reference starting with the appropriate address ...
					.Where(x => Regex.Match(HtmlEntity.DeEntitize(x.GetAttributeValue("href", string.Empty)).Trim(), @"^/AuthorArtist/", RegexOptions.IgnoreCase).Success)
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
				// Find each anchor element ...
				Children = value.DocumentNode.Descendants("a")
					// ... with a references indicating a chapter ...
					.Where(x => HtmlEntity.DeEntitize(x.GetAttributeValue("href", string.Empty)).Trim().StartsWith("/Manga/") && HtmlEntity.DeEntitize(x.GetAttributeValue("title", string.Empty)).Trim().StartsWith("Read"))
					// ... selecting each valid volume ...
					.Select(x => new { Chapter = x, Match = Regex.Match(HtmlEntity.DeEntitize(x.InnerText).Trim().ReplaceWhileWithDigit(".0", ".").RemoveToIncluding(Title).Trim(), @"(Vol\.\s*(?<Volume>[0-9\.]+))?\s*(Ch\.)?([a-z]+)?\s*(?<Number>([0-9\.]+[a-z]?|Extra|Omake))(\s?-\s?[0-9\.]+)?(\s?v\.?[0-9]+)?(\s*\(?Part\s+(?<Part>[0-9]+)\)?)?(\s?(-|\+))?\s*(Read Onl?ine|:?\s?(?<Title>.*)?(Read Online)?)$", RegexOptions.IgnoreCase) })
					// ... where the previous match was successful ...
					.Where(x => x.Match.Success && x.Chapter.ParentNode != null && x.Chapter.ParentNode.Name.Equals("td"))
					// ... selecting a proper type with all relevant information ...
					.Select(x => new Chapter(double.TryParse(x.Match.Groups["Number"].Value.AlphabeticToNumeric(), out ProcessedNumber) ? ProcessedNumber + (string.IsNullOrEmpty(x.Match.Groups["Part"].Value) ? 0 : double.Parse(x.Match.Groups["Part"].Value) / 10) : -1, Provider.Domain + HtmlEntity.DeEntitize(x.Chapter.GetAttributeValue("href", string.Empty)).Trim().TrimStart('/'), x.Match.Groups["Title"].Value, Provider.Domain + Regex.Match(HtmlEntity.DeEntitize(x.Chapter.GetAttributeValue("href", string.Empty)).Trim(), @"id=(?<UniqueIdentifier>[0-9]+)$", RegexOptions.IgnoreCase).Groups["UniqueIdentifier"].Value, double.TryParse(x.Match.Groups["Volume"].Value, out ProcessedVolume) ? ProcessedVolume : -1) as IChapter)
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
					.Where(x => Regex.Match(HtmlEntity.DeEntitize(x.GetAttributeValue("href", string.Empty)).Trim(), @"^/Genre/", RegexOptions.IgnoreCase).Success)
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
				// Find each table definition element ...
				Summary = value.DocumentNode.Descendants("span")
					// ... with a reference containing with the appropriate name ...
					.Where(x => x.ParentNode != null && HtmlEntity.DeEntitize(x.InnerText).Trim().Equals("Summary:"))
					// ... selecting the text ...
					.Select(x => HtmlEntity.DeEntitize(x.NextElement().InnerText).Trim())
					// ... and using the first match.
					.FirstOrDefault();
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
					.Select(x => new { Match = Regex.Match(HtmlEntity.DeEntitize(x.InnerText).Trim(), @"^(?<Title>.+?)\s+manga\s+|", RegexOptions.Multiline | RegexOptions.IgnoreCase) })
					// ... where a match was found ...
					.Where(x => x.Match.Success)
					// ... select the title ...
					.Select(x => x.Match.Groups["Title"].Value.Trim())
					// ... and use the first or default.
					.FirstOrDefault();
			}
		}
		#endregion

		#region Constructor
		/// <summary>
		/// Initialize a new instance of the Series class.
		/// </summary>
		/// <param name="location">The location.</param>
		public Series(string location) {
			// Set the location.
			Location = location;
		}

		/// <summary>
		/// Initialize a new instance of the Series class.
		/// </summary>
		/// <param name="location">The location.</param>
		/// <param name="title">The title.</param>
		public Series(string location, string title)
			: this(location) {
			// Set the title.
			Title = title;
		}
		#endregion

		#region IAsync
		/// <summary>
		/// Populate asynchronously.
		/// </summary>
		public async Task PopulateAsync() {
			// Get the document.
		    var response = await Http.GetAsync(Location + "?confirm=yes");
			// Initialize a new instance of the HtmlDocument class.
			var htmlDocument = new HtmlDocument();
			// Load the document.
			htmlDocument.LoadHtml(response.AsString());
			// Iterate through each property to populate the series without retaining the document.
			foreach (var propertyInfo in GetType().GetTypeInfo().DeclaredProperties.Where(x => x.PropertyType == typeof(HtmlDocument) && x.CanWrite)) {
				// Set the value, causing it to populate this property.
				propertyInfo.SetValue(this, htmlDocument, null);
			}
			// Find each image element ..
		    var ImageResponse = await Http.GetAsync(htmlDocument.DocumentNode.Descendants("img")
		        // ... with a reference containing a preview image ...
		        .Where(x => HtmlEntity.DeEntitize(x.GetAttributeValue("src", string.Empty)).Trim().Contains("/Uploads/"))
		        // ... select the reference attribute ...
		        .Select(x => HtmlEntity.DeEntitize(x.Attributes["src"].Value).Trim())
		        // ... download the first preview image ...
		        .First());
			// ... and set the image for the bytes.
			PreviewImage = ImageResponse.AsBinary();
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
				foreach (var child in Children) {
					// Dispose of the object.
					child.Dispose();
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
		public IEnumerable<string> Artists {
			get {
				// Return the authors since there is no distinction.
				return Authors;
			}
		}

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
		/// Contains the location.
		/// </summary>
		public string Location { get; private set; }

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
		#endregion
	}
}