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
	public interface IPage : IAsync<IPage> {
		#region Properties
		/// <summary>
		/// Contains the image.
		/// </summary>
		byte[] Image { get; }

		/// <summary>
		/// Contains the location.
		/// </summary>
		string Location { get; }
		#endregion
	}
}