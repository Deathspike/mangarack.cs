// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using HtmlAgilityPack;
using System;
using System.Linq;
using TinyHttp;

namespace MangaRack.Provider.Batoto {
	/// <summary>
	/// Represents a Batoto page.
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
			Http.Get(UniqueIdentifier + "?supress_webtoon=t", (Response) => {
				// Initialize a new instance of the HtmlDocument class.
				HtmlDocument HtmlDocument = new HtmlDocument();
				// Load the document.
				HtmlDocument.LoadHtml(Response.AsString());
				// Find each image element ...
				Http.Get(HtmlEntity.DeEntitize(HtmlDocument.DocumentNode.Descendants("img")
					// ... find the comic image ...
					.First(x => HtmlEntity.DeEntitize(x.GetAttributeValue("alt", string.Empty)).EndsWith("Batoto!"))
					// ... and download the source image.
					.GetAttributeValue("src", string.Empty)), (ImageResponse) => {
						// Set the image.
						Image = ImageResponse == null ? null : ImageResponse.AsBinary();
						// Invoke the callback.
						Done(this);
					});
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