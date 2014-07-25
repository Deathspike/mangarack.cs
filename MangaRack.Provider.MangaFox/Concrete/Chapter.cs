// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System.Threading.Tasks;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using MangaRack.Provider.Interfaces;
using TinyHttp;

namespace MangaRack.Provider.MangaFox {
	/// <summary>
	/// Represents a MangaFox chapter.
	/// </summary>
	class Chapter : IChapter {
		#region Constructor
		/// <summary>
		/// Initialize a new instance of the Chapter class.
		/// </summary>
		/// <param name="number">The number.</param>
		/// <param name="location">The location.</param>
		/// <param name="title">The title.</param>
		/// <param name="uniqueIdentifier">The unique identifier.</param>
		/// <param name="volume">The volume.</param>
		public Chapter(double? number, string location, string title, string uniqueIdentifier, double? volume) {
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
		public async Task PopulateAsync() {
			// Get the document.
		    var response = await Http.GetAsync(Location.EndsWith("1.html") ? Location : Location + "1.html");
			// Initialize a new instance of the HtmlDocument class.
			var htmlDocument = new HtmlDocument();
			// Load the document.
			htmlDocument.LoadHtml(response.AsString());
			// Find each select element ...
			Children = htmlDocument.DocumentNode.Descendants("select")
				// ... with the page selection change handler ...
				.Where(x => HtmlEntity.DeEntitize(x.GetAttributeValue("onchange", string.Empty)).Trim().StartsWith("change_page"))
				// ... select each option element ...
				.Select(x => x.Descendants("option"))
				// ... select the first set of elements ...
				.First()
				// ... with a possible value ...
				.Where(x => !HtmlEntity.DeEntitize(x.GetAttributeValue("value", string.Empty)).Trim().Equals("0"))
				// ... select each page ...
				.Select(x => new Page(string.Join("/", Location.Split('/').TakeWhile(y => !y.EndsWith(".html")).ToArray()).TrimEnd('/') + "/" + HtmlEntity.DeEntitize(x.GetAttributeValue("value", string.Empty)).Trim() + ".html") as IPage)
				// ... and create an array.
				.ToArray();
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
		public double? Number { get; private set; }

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
		public double? Volume { get; private set; }
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