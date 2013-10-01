// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using TinyHttp;

namespace MangaRack.Provider.Batoto {
	/// <summary>
	/// Represents a Batoto chapter.
	/// </summary>
	sealed class Chapter : IChapter {
		#region Constructor
		/// <summary>
		/// Initialize a new instance of the Chapter class.
		/// </summary>
		/// <param name="Number">The number.</param>
		/// <param name="Title">The title.</param>
		/// <param name="UniqueIdentifier">The unique identifier.</param>
		/// <param name="Volume">The volume.</param>
		public Chapter(double Number, string Title, string UniqueIdentifier, double Volume) {
			// Set the number.
			this.Number = Number;
			// Set the title.
			this.Title = Title;
			// Set the unique identifier.
			this.UniqueIdentifier = UniqueIdentifier;
			// Set the volume.
			this.Volume = Volume;
		}
		#endregion
		
		#region IAsync
		/// <summary>
		/// Populate asynchronously.
		/// </summary>
		/// <param name="Done">The callback.</param>
		public void Populate(Action<IChapter> Done) {
			// Retrieve the address.
			string Address = UniqueIdentifier;
			// Initialize the next.
			Action Next = null;
			// Initialize the next handler.
			Next = () => {
				// Get the document.
				Http.Get(Address, (Response) => {
					// Initialize a new instance of the HtmlDocument class.
					HtmlDocument HtmlDocument = new HtmlDocument();
					// Initialize the node.
					HtmlNode HtmlNode;
					// Load the document.
					HtmlDocument.LoadHtml(Response.AsString());
					// Check if a link allowing switching to traditional reading mode is available.
					if ((HtmlNode = HtmlDocument.DocumentNode.Descendants("a").FirstOrDefault(x => HtmlEntity.DeEntitize(x.GetAttributeValue("href", string.Empty)).Equals("?supress_webtoon=t"))) != null) {
						// Set the address.
						Address = Address + HtmlEntity.DeEntitize(HtmlNode.Attributes["href"].Value).Trim();
						// Invoke the next handler.
						Next();
						// Stop the function.
						return;
					}
					// Find each select element ...
					Pages = HtmlDocument.DocumentNode.Descendants("select")
						// ... with the page selection name ...
						.Where(x => HtmlEntity.DeEntitize(x.GetAttributeValue("name", string.Empty)).Trim().Equals("page_select"))
						// ... select each option element ...
						.Select(x => x.Descendants("option"))
						// ... select the first set of elements ...
						.First()
						// ... with a value pointing to a read page ...
						.Where(x => HtmlEntity.DeEntitize(x.GetAttributeValue("value", string.Empty)).Trim().Contains("/read/"))
						// ... select each page ...
						.Select(x => new Page(HtmlEntity.DeEntitize(x.GetAttributeValue("value", string.Empty)).Trim()) as IPage)
						// ... and create a list.
						.ToList();
					// Invoke the callback.
					Done(this);
				});
			};
			// Start population.
			Next();
		}
		#endregion

		#region IChapter
		/// <summary>
		/// Contains the number.
		/// </summary>
		public double Number { get; private set; }

		/// <summary>
		/// Contains each page.
		/// </summary>
		public IEnumerable<IPage> Pages { get; private set; }

		/// <summary>
		/// Contains the title.
		/// </summary>
		public string Title { get; private set; }

		/// <summary>
		/// Contains the unique identifier.
		/// </summary>
		public string UniqueIdentifier { get; private set; }

		/// <summary>
		/// Contains the volume.
		/// </summary>
		public double Volume { get; private set; }
		#endregion

		#region IDisposable
		/// <summary>
		/// Dispose of the object.
		/// </summary>
		public void Dispose() {
			// Check if the pages are valid.
			if (Pages != null) {
				// Iterate through each page.
				foreach (Page Page in Pages) {
					// Dispose of the object.
					Page.Dispose();
				}
				// Remove the pages.
				Pages = null;
			}
		}
		#endregion
	}
}