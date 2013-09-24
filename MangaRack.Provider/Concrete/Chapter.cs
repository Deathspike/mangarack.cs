// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System;
using System.Collections.Generic;
using System.Linq;

namespace MangaRack.Provider {
	/// <summary>
	/// Represents a chapter.
	/// </summary>
	class Chapter : IChapter {
		/// <summary>
		/// Contains the chapter.
		/// </summary>
		private IChapter _Chapter;

		/// <summary>
		/// Contains the number.
		/// </summary>
		private double _Number;

		/// <summary>
		/// Contains each page.
		/// </summary>
		private IEnumerable<IPage> _Pages;

		#region Constructor
		/// <summary>
		/// Initialize a new instance of the Chapter class.
		/// </summary>
		/// <param name="Chapter">The chapter.</param>
		public Chapter(IChapter Chapter) {
			// Set the chapter.
			_Chapter = Chapter;
			// Set the number.
			_Number = Chapter.Number;
		}
		#endregion

		#region IAsync
		/// <summary>
		/// Populate asynchronously.
		/// </summary>
		/// <param name="Done">The callback.</param>
		public void Populate(Action<IChapter> Done) {
			// Populate asynchronously.
			_Chapter.Populate(() => {
				// Set each page.
				_Pages = _Chapter.Pages.Select(x => new Page(x) as IPage).ToList();
				// Invoke the callback.
				Done(this);
			});
		}
		#endregion

		#region IChapter
		/// <summary>
		/// Contains the number.
		/// </summary>
		public double Number {
			get {
				// Get the number.
				return _Number;
			}
			set {
				// Set the number.
				_Number = value;
			}
		}

		/// <summary>
		/// Contains each page.
		/// </summary>
		public IEnumerable<IPage> Pages {
			get {
				// Get each page.
				return _Pages;
			}
		}

		/// <summary>
		/// Contains the title.
		/// </summary>
		public string Title {
			get {
				// Get the title.
				return _Chapter.Title;
			}
		}

		/// <summary>
		/// Contains the unique identifier.
		/// </summary>
		public string UniqueIdentifier {
			get {
				// Get the unique identifier.
				return _Chapter.UniqueIdentifier;
			}
		}

		/// <summary>
		/// Contains the volume.
		/// </summary>
		public double Volume {
			get {
				// Get the volume.
				return _Chapter.Volume;
			}
		}
		#endregion

		#region IDisposable
		/// <summary>
		/// Dispose of the object.
		/// </summary>
		public void Dispose() {
			// Dispose of the object.
			_Chapter.Dispose();
		}
		#endregion
	}
}