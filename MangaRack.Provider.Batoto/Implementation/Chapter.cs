// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MangaRack.Provider.Batoto {
	/// <summary>
	/// Represents a Batoto chapter.
	/// </summary>
	public sealed class Chapter : KeyValueStore, IChapter {
		/// <summary>
		/// Contains the HTML element.
		/// </summary>
		private readonly HtmlNode _HtmlNode;

		/// <summary>
		/// Contains the volume.
		/// </summary>
		private readonly double _Number;

		/// <summary>
		/// Contains the volume.
		/// </summary>
		private readonly double _Volume;

		/// <summary>
		/// Contains the title.
		/// </summary>
		private readonly string _Title;

		/// <summary>
		/// Contains the HTTP client.
		/// </summary>
		private readonly StateWebClient _WebClient;

		#region Constructor
		/// <summary>
		/// Initialize a new instance of the Chapter class.
		/// </summary>
		/// <param name="Referer">The referer.</param>
		/// <param name="HtmlNode">The HTML element.</param>
		/// <param name="Volume">The volume.</param>
		/// /// <param name="Number">The number.</param>
		/// <param name="Title">The title.</param>
		public Chapter(Uri Referer, HtmlNode HtmlNode, double Volume, double Number, string Title) {
			// Set the HTML element.
			_HtmlNode = HtmlNode;
			// Set the number.
			_Number = Number;
			// Set the title.
			_Title = Title;
			// Set the volume.
			_Volume = Volume;
			// Initialize a new instance of the StateWebClient class.
			_WebClient = new StateWebClient(Referer);
		}
		#endregion

		#region IChapter
		/// <summary>
		/// Contains the number.
		/// </summary>
		public double Number {
			get {
				// Return the number.
				return _Number;
			}
		}

		/// <summary>
		/// Contains each page.
		/// </summary>
		public IEnumerable<IPage> Pages {
			get {
				// Initialize a new instance of the HtmlDocument class.
				HtmlDocument HtmlDocument = new HtmlDocument();
				// Initialize the node.
				HtmlNode HtmlNode = null;
				// Initialize the image.
				byte[] Image = null;
				// Retrieve the location.
				string Location = HtmlEntity.DeEntitize(_HtmlNode.GetAttributeValue("href", string.Empty));
				// Iterate until the final page.
				while (true) {
					// Download the HTML document using the HTTP client and load the document.
					HtmlDocument.LoadHtml(_WebClient.DownloadString(Location));
					// Check if a link allowing switching to traditional reading mode is available.
					if ((HtmlNode = HtmlDocument.DocumentNode.Descendants("a").FirstOrDefault(x => x.GetAttributeValue("href", string.Empty).Equals("?supress_webtoon=t"))) != null) {
						// Set the location.
						Location = Location + HtmlEntity.DeEntitize(HtmlNode.Attributes["href"].Value);
						// Continue iteration.
						continue;
					}
					// Find each image element ...
					if ((HtmlNode = HtmlDocument.DocumentNode.Descendants("img")
						// ... whith an identifier indicating the main image ...
						.Where(x => HtmlEntity.DeEntitize(x.GetAttributeValue("id", string.Empty)).Equals("comic_page"))
						// ... and use the first or default.
						.FirstOrDefault()) != null) {
						// Attempt the retrieve the image.
						try {
							// Retrieve the image.
							Image = _WebClient.DownloadData(HtmlEntity.DeEntitize(HtmlNode.Attributes["src"].Value));
						} catch {
							// Clear the image.
							Image = null;
						}
						// Continue processing the page.
						if (true) {
							// Yield return a new instance of the MangaChapterPage class.
							yield return new Page(Image);
							// Retrieve the node containing the next available page.
							if ((HtmlNode = HtmlNode.ParentNode) != null) {
								// Set the location.
								Location = HtmlEntity.DeEntitize(HtmlNode.Attributes["href"].Value);
								// Check if the location is valid.
								if (Regex.Match(Location, "/([0-9]+)$").Success) {
									// Continue iteration.
									continue;
								}
							}
						}
					}
					// Break from iteration.
					break;
				}
			}
		}

		/// <summary>
		/// Contains the title.
		/// </summary>
		public string Title {
			get {
				// Return the title.
				return _Title;
			}
		}

		/// <summary>
		/// Contains the volume.
		/// </summary>
		public double Volume {
			get {
				// Return the volume.
				return _Volume;
			}
		}
		#endregion

		#region IDisposable
		/// <summary>
		/// Dispose of the object.
		/// </summary>
		public void Dispose() {
			// Dispose of the web client.
			_WebClient.Dispose();
		}
		#endregion
	}
}