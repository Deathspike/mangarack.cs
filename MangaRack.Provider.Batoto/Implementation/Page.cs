// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System;
using System.Drawing;

namespace MangaRack.Provider.Batoto {
	/// <summary>
	/// Represents a Batoto page.
	/// </summary>
	public sealed class Page : IPage {
		#region Constructor
		/// <summary>
		/// Initialize a new instance of the Page class.
		/// </summary>
		/// <param name="Bytes"></param>
		public Page(byte[] Bytes) {
			// Create a bitmap from the bytes and check if it invalid.
			if ((Bitmap = Bytes.Bitmap()) == null) {
				// Throw an exception.
				throw new Exception("Invalid informatie retrieved for the page.");
			}
		}
		#endregion

		#region IDisposable
		/// <summary>
		/// Dispose of the object.
		/// </summary>
		public void Dispose() {
			// Dispose of the bitmap.
			Bitmap.Dispose();
		}
		#endregion

		#region IPage
		/// <summary>
		/// Contains the bitmap.
		/// </summary>
		public Bitmap Bitmap { get; set; }
		#endregion
	}
}