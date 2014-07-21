// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System.Threading.Tasks;
using HtmlAgilityPack;
using System;
using System.Linq;
using System.Net;
using MangaRack.Provider.Interfaces;
using TinyHttp;

namespace MangaRack.Provider.MangaFox {
	/// <summary>
	/// Represents a MangaFox page.
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
		    var htmlResponse = await Http.GetAsync(Location);
			// Initialize a new instance of the HtmlDocument class.
			var htmlDocument = new HtmlDocument();
			// Initialize a new instance of the HtmlNode class.
			var htmlNode = null as HtmlNode;
			// Load the document.
			htmlDocument.LoadHtml(htmlResponse.AsString());
			// Find the each meta ...
			if ((htmlNode = htmlDocument.DocumentNode.Descendants("meta")
				// ... with a property indicating the image thumbnail ...
				.Where(x => HtmlEntity.DeEntitize(x.GetAttributeValue("property", string.Empty)).Equals("og:image"))
				// ... use the first or default.
				.FirstOrDefault()) != null) {
				// Initialize the address ...
				var address = HtmlEntity.DeEntitize(htmlNode.GetAttributeValue("content", string.Empty)).Trim()
					// ... replace the thumbnail address for the page address ...
					.Replace("thumbnails/mini.", "compressed/")
					// ... replace the back-up server for the main server.
					.Replace("http://l.", "http://l.");
				// Request the image.
				var imageResponse = await Http.GetAsync(address);
				// Check if the image response is invalid.
				if (imageResponse == null || imageResponse.StatusCode != HttpStatusCode.OK) {
					// Request an alternative image.
				    var alternativeImageResponse = await Http.GetAsync(address.Replace("http://z.", "http://l."));
					// Set the image.
					Image = alternativeImageResponse.AsBinary();
				} else {
					// Set the image.
					Image = imageResponse.AsBinary();
				}
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