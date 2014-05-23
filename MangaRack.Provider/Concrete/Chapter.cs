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
	sealed class Chapter : IChapter {
		/// <summary>
		/// Contains the chapter.
		/// </summary>
		private readonly IChapter _chapter;

		/// <summary>
		/// Contains each child.
		/// </summary>
		private IEnumerable<IPage> _children;

		/// <summary>
		/// Contains the number.
		/// </summary>
		private double _number;

		#region Constructor
		/// <summary>
		/// Initialize a new instance of the Chapter class.
		/// </summary>
		/// <param name="chapter">The chapter.</param>
		public Chapter(IChapter chapter) {
			// Set the chapter.
			_chapter = chapter;
			// Set the number.
			_number = chapter.Number;
		}
		#endregion

		#region IAsync
		/// <summary>
		/// Populate asynchronously.
		/// </summary>
		/// <param name="done">The callback.</param>
		public void Populate(Action<IChapter> done) {
			// Populate asynchronously.
			_chapter.Populate(() => {
				// Set each child.
				_children = _chapter.Children.Select(x => new Page(x) as IPage).ToArray();
				// Invoke the callback.
				done(this);
			});
		}
		#endregion

		#region IChapter
		/// <summary>
		/// Contains each child.
		/// </summary>
		public IEnumerable<IPage> Children {
			get {
				// Return each child.
				return _children;
			}
		}

		/// <summary>
		/// Contains the number.
		/// </summary>
		public double Number {
			get {
				// Return the number.
				return _number;
			}
			set {
				// Set the number.
				_number = value;
			}
		}

		/// <summary>
		/// Contains the title.
		/// </summary>
		public string Title {
			get {
				// Return the title.
				return _chapter.Title;
			}
		}

		/// <summary>
		/// Contains the unique identifier.
		/// </summary>
		public string UniqueIdentifier {
			get {
				// Return the unique identifier.
				return _chapter.UniqueIdentifier;
			}
		}

		/// <summary>
		/// Contains the volume.
		/// </summary>
		public double Volume {
			get {
				// Return the volume.
				return _chapter.Volume;
			}
		}
		#endregion

		#region IDisposable
		/// <summary>
		/// Dispose of the object.
		/// </summary>
		public void Dispose() {
			// Dispose of the object.
			_chapter.Dispose();
		}
		#endregion
	}
}