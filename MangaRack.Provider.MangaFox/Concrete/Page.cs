// ======================================================================
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
		/// <param name="UniqueIdentifier">The unique identifier.</param>
		public Page(string UniqueIdentifier) {
			// Set the unique identifier.
			this.UniqueIdentifier = UniqueIdentifier;
		}
		#endregion

		#region IAsync
		/// <summary>
		/// Populate asynchronously.
		/// </summary>
		/// <param name="Done">The callback.</param>
		public void Populate(Action<IPage> Done) {
			// Get the document.
			Http.Get(UniqueIdentifier, (HtmlResponse) => {
				// Initialize a new instance of the HtmlDocument class.
				HtmlDocument HtmlDocument = new HtmlDocument();
				// Initialize a new instance of the HtmlNode class.
				HtmlNode HtmlNode;
				// Load the document.
				HtmlDocument.LoadHtml(HtmlResponse.AsString());
				// Find each image ...
				if ((HtmlNode = HtmlDocument.DocumentNode.Descendants("img")
					// ... whith an identifier indicating the main image ...
					.Where(x => HtmlEntity.DeEntitize(x.GetAttributeValue("id", string.Empty)).Trim().Equals("image"))
					// ... use the first or default.
					.FirstOrDefault()) != null) {
					// Initialize the address.
					string Address = HtmlEntity.DeEntitize(HtmlNode.GetAttributeValue("src", string.Empty)).Trim();
					// Request the image.
					Http.Get(Address, (ImageResponse) => {
						// Check if the image response is invalid.
						if (ImageResponse == null || ImageResponse.StatusCode != HttpStatusCode.OK) {
							// Request an alternative image.
							Http.Get(Address.Replace("http://z.", "http://l."), (AlternativeImageResponse) => {
								// Set the image.
								Image = AlternativeImageResponse.AsBinary();
								// Invoke the callback.
								Done(this);
							});
						} else {
							// Set the image.
							Image = ImageResponse.AsBinary();
							// Invoke the callback.
							Done(this);
						}
					});
				} else {
					// Invoke the callback.
					Done(this);
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