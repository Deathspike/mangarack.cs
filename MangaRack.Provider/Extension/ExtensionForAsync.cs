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
		/// <param name="async">The asynchronous population.</param>
		public static T Populate<T>(this IAsync<T> async) {
			// Initialize a new instance of the ManualResetEventSlim class.
			var manualResetEventSlim = new ManualResetEvent(false);
			// Initialize the result.
			var result = default(T);
			// Populate asynchronously.
			async.Populate(Done => {
				// Set the result.
				result = Done;
				// Raise the event and unblock the calling thread.
				manualResetEventSlim.Set();
			});
			// Block the thread waiting for the event.
			manualResetEventSlim.WaitOne();
			// Return the result.
			return result;
		}

		/// <summary>
		/// Populate asynchronously.
		/// </summary>
		/// <param name="async">The asynchronous population.</param>
		/// <param name="done">The callback.</param>
		public static void Populate<T>(this IAsync<T> async, Action done) {
			// Populate asynchronously.
			async.Populate(Result => {
				// Invoke the callback.
				done();
			});
		}
		#endregion
	}
}