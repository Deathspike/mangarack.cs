// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using MangaRack.Provider;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MangaRack.ViewModel {
	/// <summary>
	/// Represents the chapter view model.
	/// </summary>
	public sealed class ChapterViewModel : ViewModelStore {
		#region Constructor
		/// <summary>
		/// Initialize a new instance of the ChapterViewModel class.
		/// </summary>
		/// <param name="source">The source.</param>
		public ChapterViewModel(IChapter source) {
			// Set the source.
			Source = source;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Contains each child.
		/// </summary>
		public IEnumerable<PageViewModel> Children {
			get {
				// Retrieve the value.
				return Get<IEnumerable<PageViewModel>>(() => Children);
			}
			private set {
				// Set the value.
				Set(() => Children, value);
			}
		}

		/// <summary>
		/// Indicates whether the view model is populating.
		/// </summary>
		public bool IsPopulating {
			get {
				// Retrieve the value.
				return Get<bool>(() => IsPopulating, false);
			}
			private set {
				// Set the value.
				Set(() => IsPopulating, value);
			}
		}

		/// <summary>
		/// Populate asynchronously.
		/// </summary>
		public Action Populate {
			get {
				// Initialize an action command.
				return () => {
					// Check if the view model is not populating.
					if (!IsPopulating) {
						// Set whether the view model is populating.
						IsPopulating = true;
						// Populate asynchronously.
						Source.Populate(() => {
							// Set the children.
							Children = Source.Children.Select(x => new PageViewModel(x)).ToArray();
							// Set whether the view model is populating.
							IsPopulating = false;
						});
					}
				};
			}
		}
		
		/// <summary>
		/// Contains the source.
		/// </summary>
		public IChapter Source {
			get {
				// Retrieve the value.
				return Get<IChapter>(() => Source);
			}
			private set {
				// Set the value.
				Set(() => Source, value);
			}
		}
		#endregion
	}
}