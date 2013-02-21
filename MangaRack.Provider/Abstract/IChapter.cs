// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System;
using System.Collections.Generic;

namespace MangaRack.Provider {
	/// <summary>
	/// Represents a chapter.
	/// </summary>
	public interface IChapter : IDisposable {
		/// <summary>
		/// Contains the number.
		/// </summary>
		double Number { get; }

		/// <summary>
		/// Contains each page.
		/// </summary>
		IEnumerable<IPage> Pages { get; }

		/// <summary>
		/// Contains the title.
		/// </summary>
		string Title { get; }

		/// <summary>
		/// Contains the volume.
		/// </summary>
		double Volume { get; }
	}
}