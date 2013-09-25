// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System;
using TinyHttp;

namespace MangaRack.Provider.KissManga {
	/// <summary>
	/// Represents a KissManga page.
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
			// Download the source image.
			Http.Get(UniqueIdentifier, (ImageResponse) => {
				// Set the image.
				Image = ImageResponse.AsBinary();
				// Invoke the callback.
				Done(this);
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