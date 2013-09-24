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
    class Page : IPage {
		/// <summary>
		/// Contains the page.
		/// </summary>
		private IPage _Page;

		#region Constructor
		/// <summary>
		/// Initialize a new instance of the Page class.
		/// </summary>
		/// <param name="Page">The page.</param>
		public Page(IPage Page) {
			// Set the page.
			_Page = Page;
		}
		#endregion

		#region IAsync
		/// <summary>
		/// Populate asynchronously.
		/// </summary>
		/// <param name="Done">The callback.</param>
		public void Populate(Action<IPage> Done) {
			// Populate asynchronously.
			_Page.Populate(() => {
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
			// Dispose of the object.
			_Page.Dispose();
		}
		#endregion

		#region IPage
		/// <summary>
		/// Contains the image.
		/// </summary>
		public byte[] Image {
			get {
				// Get the image.
				return _Page.Image;
			}
		}

		/// <summary>
		/// Contains the unique identifier.
		/// </summary>
		public string UniqueIdentifier {
			get {
				// Get the unique identifier.
				return _Page.UniqueIdentifier;
			}
		}
		#endregion
	}
}