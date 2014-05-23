﻿// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using HtmlAgilityPack;
using System;
using System.Linq;
using System.Net;
using TinyHttp;

namespace MangaRack.Provider.MangaFox {
	/// <summary>
	/// Represents a MangaFox page.
	/// </summary>
	sealed class Page : IPage {
		#region Constructor
		/// <summary>
		/// Initialize a new instance of the Page class.
		/// </summary>
		/// <param name="uniqueIdentifier">The unique identifier.</param>
		public Page(string uniqueIdentifier) {
			// Set the unique identifier.
			UniqueIdentifier = uniqueIdentifier;
		}
		#endregion

		#region IAsync
		/// <summary>
		/// Populate asynchronously.
		/// </summary>
		/// <param name="done">The callback.</param>
		public void Populate(Action<IPage> done) {
			// Get the document.
			Http.Get(UniqueIdentifier, htmlResponse => {
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
					Http.Get(address, imageResponse => {
						// Check if the image response is invalid.
						if (imageResponse == null || imageResponse.StatusCode != HttpStatusCode.OK) {
							// Request an alternative image.
							Http.Get(address.Replace("http://z.", "http://l."), alternativeImageResponse => {
								// Set the image.
								Image = alternativeImageResponse.AsBinary();
								// Invoke the callback.
								done(this);
							});
						} else {
							// Set the image.
							Image = imageResponse.AsBinary();
							// Invoke the callback.
							done(this);
						}
					});
				} else {
					// Invoke the callback.
					done(this);
				}
			});
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
		/// Contains the unique identifier.
		/// </summary>
		public string UniqueIdentifier { get; private set; }
		#endregion
	}
}