// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MangaRack.Provider.Interface;
using TinyHttp;

namespace MangaRack.Provider.Batoto.Internal {
	/// <summary>
	/// Represents a Batoto page.
	/// </summary>
	class Page : IPage {
		#region Constructor
		/// <summary>
		/// Initialize a new instance of the Page class.
		/// </summary>
		/// <param name="location">The location.</param>
		public Page(string location) {
			// Set the location.
			Location = location;
		}
		#endregion

		#region IAsync
		/// <summary>
		/// Populate asynchronously.
		/// </summary>
		public async Task PopulateAsync() {
			// Get the document.
		    var HtmlResponse = await Http.GetAsync(Location + "?supress_webtoon=t");
			// Initialize a new instance of the HtmlDocument class.
			var htmlDocument = new HtmlDocument();
			// Initialize a new instance of the HtmlNode class.
			var htmlNode = null as HtmlNode;
			// Load the document.
			htmlDocument.LoadHtml(HtmlResponse.AsString());
			// Find each image ...
			if ((htmlNode = htmlDocument.DocumentNode.Descendants("img")
				// ... whith an attribute indicating the main image ...
				.Where(x => HtmlEntity.DeEntitize(x.GetAttributeValue("alt", string.Empty)).Trim().EndsWith("Batoto!"))
				// ... use the first or default.
				.FirstOrDefault()) != null) {
				// Request the image.
				var imageResponse = await Http.GetAsync(HtmlEntity.DeEntitize(htmlNode.GetAttributeValue("src", string.Empty)).Trim());
				// Set the image.
				Image = imageResponse.AsBinary();
			}
		}
		#endregion

		#region IDisposable
		/// <summary>
		/// Dispose of the object.
		/// </summary>
		public void Dispose() {
			// Remove the image.
			Image = null;
		}
		#endregion

		#region IPage
		/// <summary>
		/// Contains the image.
		/// </summary>
		public byte[] Image { get; private set; }

		/// <summary>
		/// Contains the location.
		/// </summary>
		public string Location { get; private set; }
		#endregion
	}
}