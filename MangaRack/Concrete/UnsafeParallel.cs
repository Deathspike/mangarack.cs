// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MangaRack {
	/// <summary>
	/// Represents the class that provides support for terminating parallel loops.
	/// </summary>
	static class UnsafeParallel {
		#region Statics
		/// <summary>
		/// Execute a for loop in which iterations run in parallel.
		/// </summary>
		/// <param name="fromInclusive">The start index, inclusive.</param>
		/// <param name="toExclusive">The end index, exclusive.</param>
		/// <param name="body">The delegate that is invoked once per iteration.</param>
		public static void For(int fromInclusive, int toExclusive, Func<int, Task> body) {
			// Execute a for loop in which iterations run in parallel.
			For(fromInclusive, toExclusive, Environment.ProcessorCount, body);
		}

		/// <summary>
		/// Execute a for loop in which iterations run in parallel.
		/// </summary>
		/// <param name="fromInclusive">The start index, inclusive.</param>
		/// <param name="toExclusive">The end index, exclusive.</param>
		/// <param name="threadCount">The thread count.</param>
		/// <param name="body">The delegate that is invoked once per iteration.</param>
		public static void For(int fromInclusive, int toExclusive, int threadCount, Func<int, Task> body) {
			// Initialize the exception.
			var exception = (Exception)null;
			// Initialize the reset event array.
			var manualResetEventSlim = new ManualResetEvent[threadCount];
			// Initialize the thread array.
			var thread = new Thread[threadCount];
			// Initialize the queue.
			var queue = new Queue<int>(Enumerable.Range(fromInclusive, toExclusive));
			// Iterate through each thread.
			for (var x = 0; x < threadCount; x++) {
				// Initialize the context safe offset.
				var offset = x;
				// Initialize a new instance of the ManualResetEvent class.
				manualResetEventSlim[offset] = new ManualResetEvent(false);
				// Initialize a new instance of the Thread class.
				(thread[offset] = new Thread(async () => {
					// Attempt the following code.
					try {
						// Initialize the integer.
						int i;
						// Set the thread culture.
						thread[offset].CurrentCulture = new CultureInfo("en-US");
						// Iterate while an integer is available.
						while (true) {
							// Lock the queue.
							lock (queue) {
								// Check if the queue is empty.
								if (queue.Count == 0) {
									// Break the loop.
									break;
								}
								// Dequeue an integer.
								i = queue.Dequeue();
							}
							// Run the delegate.
							await body(i);
						}
						// Set the reset event.
						manualResetEventSlim[offset].Set();
					} catch (Exception e) {
						// Lock the thread array.
						lock (thread) {
							// Check if the exception has not yet been set.
							if (exception == null) {
								// Set the exception.
								exception = e;
								// Iterate through each thread.
								for (int y = 0; y < thread.Length; y++) {
									// Set the reset event.
									manualResetEventSlim[y].Set();
								}
							}
						}
					}
				})).Start();
			}
			// Iterate through each reset event.
			for (var x = 0; x < threadCount; x++) {
				// Wait on the reset event.
				manualResetEventSlim[x].WaitOne();
			}
			// Check if the exception has been set.
			if (exception != null) {
				// Iterate through each thread.
				for (var x = 0; x < threadCount; x++) {
					// Abort the thread.
					thread[x].Abort();
				}
				// Throw the exception.
				throw exception;
			}
		}
		#endregion
	}
}