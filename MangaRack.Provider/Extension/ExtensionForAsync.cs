// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System;
using System.Threading;

namespace MangaRack.Provider {
	/// <summary>
	/// Represents the class providing extensions for the IAsync interface.
	/// </summary>
	public static class ExtensionForAsync {
		#region Methods
		/// <summary>
		/// Populate synchronously.
		/// </summary>
		/// <param name="Async">The asynchronous population.</param>
		public static T Populate<T>(this IAsync<T> Async) {
			// Initialize a new instance of the ManualResetEventSlim class.
			ManualResetEvent ManualResetEventSlim = new ManualResetEvent(false);
			// Initialize the result.
			T Result = default(T);
			// Populate asynchronously.
			Async.Populate((Done) => {
				// Set the result.
				Result = Done;
				// Raise the event and unblock the calling thread.
				ManualResetEventSlim.Set();
			});
			// Block the thread waiting for the event.
			ManualResetEventSlim.WaitOne();
			// Return the result.
			return Result;
		}

		/// <summary>
		/// Populate asynchronously.
		/// </summary>
		/// <param name="Async">The asynchronous population.</param>
		/// <param name="Done">The callback.</param>
		public static void Populate<T>(this IAsync<T> Async, Action Done) {
			// Populate asynchronously.
			Async.Populate((Result) => {
				// Invoke the callback.
				Done();
			});
		}
		#endregion
	}
}