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
using System.Threading.Tasks;

namespace MangaRack.Provider.KissManga {
	/// <summary>
	/// Represents a KissManga chapter.
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
		/// <param name="Number">The number.</param>
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
				// Retrieve the content.
				string Content = _WebClient.DownloadString(string.Format("http://kissmanga.com/{0}", HtmlEntity.DeEntitize(_HtmlNode.GetAttributeValue("href", string.Empty))));
				// Match the images from the JavaScript portion of the page.
				MatchCollection Matches = new Regex(@"lstImages\.push\((.*)\)").Matches(Content);
				// Initialize a new instance of the Dictionary class.
				Dictionary<int, IPage> Pages = new Dictionary<int, IPage>();
				// Retrieve each match in parallel.
				Parallel.For(0, Matches.Count, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, (i) => {
					// Initialize a new instance of the StateWebClient class.
					using (StateWebClient WebClient = new StateWebClient(_WebClient.Referer)) {
						// Attempt the following code.
						try {
							// Initialize a new instance of the Page class.
							IPage Page = new Page(WebClient.DownloadData(HtmlEntity.DeEntitize(Matches[i].Groups[1].Value).Trim('"', '\\')));
							// Lock the pages.
							lock (Pages) {
								// Set the page.
								Pages[i] = Page;
							}
						} catch {
							// Lock the pages.
							lock (Pages) {
								// Set the page.
								Pages[i] = null;
							}
						}
					}
				});
				// Return the pages.
				return Pages.OrderBy(x => x.Key).Select(x => x.Value);
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