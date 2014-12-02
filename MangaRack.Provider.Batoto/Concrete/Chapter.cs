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
		/// <param name="number">The number.</param>
		/// <param name="location">The location.</param>
		/// <param name="title">The title.</param>
		/// <param name="uniqueIdentifier">The unique identifier.</param>
		/// <param name="volume">The volume.</param>
		public Chapter(double number, string location, string title, string uniqueIdentifier, double volume) {
			// Set the number.
			Number = number;
			// Set the location.
			Location = location;
			// Set the title.
			Title = title;
			// Set the unique identifier.
			UniqueIdentifier = uniqueIdentifier;
			// Set the volume.
			Volume = volume;
		}
		#endregion
		
		#region IAsync
		/// <summary>
		/// Populate asynchronously.
		/// </summary>
		/// <param name="done">The callback.</param>
		public void Populate(Action<IChapter> done) {
			// Initialize the location.
			var location = Location;
			// Initialize the next.
			var next = null as Action;
			// Initialize the next handler.
			next = () => {
				// Get the document.
				Http.Get(location, response => {
					// Initialize a new instance of the HtmlDocument class.
					var htmlDocument = new HtmlDocument();
					// Initialize the node.
					var htmlNode = null as HtmlNode;
					// Load the document.
					htmlDocument.LoadHtml(response.AsString());
					// Check if a link allowing switching to traditional reading mode is available.
					if ((htmlNode = htmlDocument.DocumentNode.Descendants("a").FirstOrDefault(x => HtmlEntity.DeEntitize(x.GetAttributeValue("href", string.Empty)).Equals("?supress_webtoon=t"))) != null) {
						// Set the location.
						location = location + HtmlEntity.DeEntitize(htmlNode.Attributes["href"].Value).Trim();
						// Invoke the next handler.
						next();
						// Stop the function.
						return;
					}
					// Find each select element ...
					Children = htmlDocument.DocumentNode.Descendants("select")
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
						// ... and create an array.
						.ToArray();
					// Invoke the callback.
					done(this);
				});
			};
			// Start population.
			next();
		}
		#endregion

		#region IChapter
		/// <summary>
		/// Contains each child.
		/// </summary>
		public IEnumerable<IPage> Children { get; private set; }

		/// <summary>
		/// Contains the location.
		/// </summary>
		public string Location { get; private set; }

		/// <summary>
		/// Contains the number.
		/// </summary>
		public double Number { get; private set; }

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
		}
		#endregion
	}
}