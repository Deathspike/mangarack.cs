// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System;
using System.ComponentModel;

namespace MangaRack.ViewModel {
	/// <summary>
	/// Represents a key-value store for a view model.
	/// </summary>
	public abstract class ViewModelStore : KeyValueStore, INotifyPropertyChanged {
		#region Properties
		/// <summary>
		/// Raise a changed event.
		/// </summary>
		public Action<string> Changed {
			get {
				// Retrieve the value.
				return Get<Action<string>>(() => Changed, key => {
					// Check if the property changed has subscribers.
					if (PropertyChanged != null) {
						// Invoke the property changed.
						PropertyChanged(this, new PropertyChangedEventArgs(key));
					}
				});
			}
			set {
				// Set the value.
				Set(() => Changed, value);
			}
		}
		#endregion

		#region KeyValueStore
		/// <summary>
		/// Remove a value.
		/// </summary>
		/// <param name="key">The key.</param>
		public override bool Remove(string key) {
			// Remove a value and check if it succeeded.
			if (base.Remove(key)) {
				// Raise a changed event.
				Changed(key);
				// Return true.
				return true;
			}
			// Return false.
			return false;
		}

		/// <summary>
		/// Set a value.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		public override bool Set(string key, object value) {
			// Check if the value is changing.
			if (base.Set(key, value)) {
				// Raise a changed event.
				Changed(key);
				// Return true.
				return true;
			} else {
				// Set a value.
				return base.Set(key, value);
			}
		}
		#endregion

		#region INotifyPropertyChanged
		/// <summary>
		/// Occurs when a property value changes.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;
		#endregion
	}
}