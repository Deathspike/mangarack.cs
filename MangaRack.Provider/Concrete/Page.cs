// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System;

namespace MangaRack.Provider {
	/// <summary>
	/// Represents a page.
	/// </summary>
	sealed class Page : IPage {
		/// <summary>
		/// Contains the page.
		/// </summary>
		private IPage _page;

		#region Constructor
		/// <summary>
		/// Initialize a new instance of the Page class.
		/// </summary>
		/// <param name="page">The page.</param>
		public Page(IPage page) {
			// Set the page.
			_page = page;
		}
		#endregion

		#region IAsync
		/// <summary>
		/// Populate asynchronously.
		/// </summary>
		/// <param name="done">The callback.</param>
		public void Populate(Action<IPage> done) {
			// Populate asynchronously.
			_page.Populate(() => {
				// Invoke the callback.
				done(this);
			});
		}
		#endregion

		#region IDisposable
		/// <summary>
		/// Dispose of the object.
		/// </summary>
		public void Dispose() {
			// Dispose of the object.
			_page.Dispose();
		}
		#endregion

		#region IPage
		/// <summary>
		/// Contains the image.
		/// </summary>
		public byte[] Image {
			get {
				// Return the image.
				return _page.Image;
			}
		}

		/// <summary>
		/// Contains the unique identifier.
		/// </summary>
		public string UniqueIdentifier {
			get {
				// Return the unique identifier.
				return _page.UniqueIdentifier;
			}
		}
		#endregion
	}
}