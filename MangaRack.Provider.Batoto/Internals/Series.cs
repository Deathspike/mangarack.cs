﻿// ======================================================================
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MangaRack.Provider.Batoto.Extensions;
using MangaRack.Provider.Interfaces;
using TinyHttp;

namespace MangaRack.Provider.Batoto.Internals {
	/// <summary>
	/// Represents a Batoto series.
	/// </summary>
	class Series : ISeries {
		#region Abstract
		/// <summary>
		/// Populate each artist.
		/// </summary>
		private HtmlDocument _Artists {
			set {
				Artists = value.DocumentNode.Descendants("td")
					.Where(x => x.ParentNode != null && HtmlEntity.DeEntitize(x.InnerText).Trim().Equals("Artist:"))
					.SelectMany(x => x.ParentNode.LastElement().Descendants("a")
						.Select(y => HtmlEntity.DeEntitize(y.InnerText).Trim()))
					.ToArray();
			}
		}

		/// <summary>
		/// Populate each author.
		/// </summary>
		private HtmlDocument _Authors {
			set {
				Authors = value.DocumentNode.Descendants("td")
					.Where(x => x.ParentNode != null && HtmlEntity.DeEntitize(x.InnerText).Trim().Equals("Author:"))
					.SelectMany(x => x.ParentNode.LastElement().Descendants("a")
						.Select(y => HtmlEntity.DeEntitize(y.InnerText).Trim()))
					.ToArray();
			}
		}

		/// <summary>
		/// Populate each child.
		/// </summary>
		private HtmlDocument _Children {
			set {
				double ProcessedNumber, ProcessedVolume;
				Children = value.DocumentNode.Descendants("a")
					.Where(x => x.ParentNode != null && x.ParentNode.ParentNode != null && HtmlEntity.DeEntitize(x.ParentNode.ParentNode.GetAttributeValue("class", string.Empty)).Trim().Split(' ').Contains("lang_English"))
					.Where(x => HtmlEntity.DeEntitize(x.GetAttributeValue("href", string.Empty)).Trim().Contains("/read/"))
					.Select(x => new { Chapter = x, Match = Regex.Match(HtmlEntity.DeEntitize(x.InnerText).Trim(), @"(Vol\.\s*(?<Volume>[0-9\.]+))?\s*(Ch\.)?([a-z]+)?\s*(?<Number>([0-9\.]+[a-z]?|Extra|Omake))(\s?-\s?[0-9\.]+)?(\s?v\.?[0-9]+)?(\s*\(?Part\s+(?<Part>[0-9]+)\)?)?(\s?(-|\+))?\s*(Read Onl?ine|:?\s?(?<Title>.*)?(Read Online)?)$", RegexOptions.IgnoreCase) })
					.Where(x => x.Match.Success)
					.Select(x => new Chapter(double.TryParse(x.Match.Groups["Number"].Value.AlphabeticToNumeric(), out ProcessedNumber) ? ProcessedNumber + (string.IsNullOrEmpty(x.Match.Groups["Part"].Value) ? 0 : (double?)double.Parse(x.Match.Groups["Part"].Value) / 10) : null, HtmlEntity.DeEntitize(x.Chapter.GetAttributeValue("href", string.Empty)).Trim(), x.Match.Groups["Title"].Value.Trim(), Provider.Domain + Regex.Match(HtmlEntity.DeEntitize(x.Chapter.GetAttributeValue("href", string.Empty)).Trim(), @"/_/(?<UniqueIdentifier>[0-9]+)/", RegexOptions.IgnoreCase).Groups["UniqueIdentifier"].Value, double.TryParse(x.Match.Groups["Volume"].Value, out ProcessedVolume) ? (double?)ProcessedVolume : null) as IChapter)
					.Reverse()
					.ToArray();
			}
		}

		/// <summary>
		/// Populate each genre.
		/// </summary>
		private HtmlDocument _Genres {
			set {
				Genres = value.DocumentNode.Descendants("td")
					.Where(x => x.ParentNode != null && HtmlEntity.DeEntitize(x.InnerText).Equals("Genres:"))
					.SelectMany(x => x.ParentNode.LastElement().Descendants("span")
						.Select(y => HtmlEntity.DeEntitize(y.InnerText).Trim()))
					.ToArray();
			}
		}

		/// <summary>
		/// Populate the summary.
		/// </summary>
		private HtmlDocument _Summary {
			set {
				Summary = string.Join("\n", value.DocumentNode.Descendants("td")
					.Where(x => x.ParentNode != null && HtmlEntity.DeEntitize(x.InnerText).Trim().Equals("Description:"))
					.SelectMany(x => x.ParentNode.LastElement().Descendants()
						.Select(y => HtmlEntity.DeEntitize(y.InnerText).Trim()))
					.ToArray())
					.Replace("\n\n", "\n");
			}
		}

		/// <summary>
		/// Populate the title.
		/// </summary>
		private HtmlDocument _Title {
			set {
				Title = value.DocumentNode.Descendants("h1")
					.Where(x => HtmlEntity.DeEntitize(x.GetAttributeValue("class", string.Empty)).Trim().Split(' ').Contains("ipsType_pagetitle"))
					.Select(x => HtmlEntity.DeEntitize(x.InnerText).Trim())
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
			Location = location;
		}

		/// <summary>
		/// Initialize a new instance of the Series class.
		/// </summary>
		/// <param name="location">The location.</param>
		/// <param name="title">The title.</param>
		public Series(string location, string title)
			: this(location) {
			Title = title;
		}
		#endregion

		#region IAsync
		/// <summary>
		/// Populate asynchronously.
		/// </summary>
		public async Task PopulateAsync() {
		    var response = await Http.GetAsync(Location);
			var htmlDocument = new HtmlDocument();
			htmlDocument.LoadHtml(response.AsString());
			foreach (var propertyInfo in GetType().GetTypeInfo().DeclaredProperties.Where(x => x.PropertyType == typeof(HtmlDocument) && x.CanWrite)) {
				propertyInfo.SetValue(this, htmlDocument, null);
			}
			var imageResponse = await Http.GetAsync(htmlDocument.DocumentNode.Descendants("img")
				.Where(x => HtmlEntity.DeEntitize(x.GetAttributeValue("src", string.Empty)).Trim().Contains("/uploads/"))
				.Select(x => HtmlEntity.DeEntitize(x.Attributes["src"].Value).Trim())
				.First());
			PreviewImage = imageResponse.AsBinary();
		}
		#endregion

		#region IDisposable
		/// <summary>
		/// Dispose of the object.
		/// </summary>
		public void Dispose() {
			if (Children != null) {
				foreach (var child in Children) {
					child.Dispose();
				}
				Children = null;
			}
			if (PreviewImage != null) {
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