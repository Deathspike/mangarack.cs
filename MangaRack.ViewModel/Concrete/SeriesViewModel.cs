// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using MangaRack.Provider;
using System;

namespace MangaRack.ViewModel {
	/// <summary>
	/// Represents the series view model.
	/// </summary>
	public sealed class SeriesViewModel : ViewModelStore {
		#region Constructor
		/// <summary>
		/// Initialize a new instance of the SeriesViewModel class.
		/// </summary>
		/// <param name="Source">The source.</param>
		public SeriesViewModel(ISeries Source) {
			// Set the source.
			this.Source = Source;
		}
		#endregion

		#region Properties
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
							// Set the results.
							// Results = Source.Results.Select(x => new SeriesViewModel(x)).ToArray();
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
		public ISeries Source {
			get {
				// Retrieve the value.
				return Get<ISeries>(() => Source);
			}
			private set {
				// Set the value.
				Set(() => Source, value);
			}
		}
		#endregion
	}
}