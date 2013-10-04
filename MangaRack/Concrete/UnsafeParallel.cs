﻿// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System;
using System.Threading;

namespace MangaRack {
	/// <summary>
	/// Represents the class that provides support for terminating parallel loops.
	/// </summary>
	static class UnsafeParallel {
		#region Statics
		/// <summary>
		/// Execute a for loop in which iterations run in parallel.
		/// </summary>
		/// <param name="FromInclusive">The start index, inclusive.</param>
		/// <param name="ToExclusive">The end index, exclusive.</param>
		/// <param name="Body">The delegate that is invoked once per iteration.</param>
		public static void For(int FromInclusive, int ToExclusive, Action<int> Body) {
			// Execute a for loop in which iterations run in parallel.
			For(FromInclusive, ToExclusive, Environment.ProcessorCount, Body);
		}

		/// <summary>
		/// Execute a for loop in which iterations run in parallel.
		/// </summary>
		/// <param name="FromInclusive">The start index, inclusive.</param>
		/// <param name="ToExclusive">The end index, exclusive.</param>
		/// <param name="ThreadCount">The thread count.</param>
		/// <param name="Body">The delegate that is invoked once per iteration.</param>
		public static void For(int FromInclusive, int ToExclusive, int ThreadCount, Action<int> Body) {
			// Initialize the exception.
			Exception Exception = null;
			// Initialize the reset event array.
			ManualResetEventSlim[] ManualResetEventSlim = new ManualResetEventSlim[ThreadCount];
			// Initialize the thread array.
			Thread[] Thread = new Thread[ThreadCount];
			// Iterate through each thread.
			for (int x = 0; x < ThreadCount; x++) {
				// Initialize the context safe offset.
				int Offset = x;
				// Initialize a new instance of the ManualResetEventSlim class.
				ManualResetEventSlim[x] = new ManualResetEventSlim(false);
				// Initialize a new instance of the Thread class.
				(Thread[x] = new Thread(() => {
					// Attempt the following code.
					try {
						// Iterate through each block.
						for (int i = FromInclusive + Offset; i < ToExclusive; i += ThreadCount) {
							// Invoke the delegate.
							Body(i);
						}
						// Set the reset event.
						ManualResetEventSlim[Offset].Set();
					} catch (Exception e) {
						// Lock the thread array.
						lock (Thread) {
							// Check if the exception has not yet been set.
							if (Exception == null) {
								// Set the exception.
								Exception = e;
								// Iterate through each thread.
								for (int y = 0; y < Thread.Length; y++) {
									// Set the reset event.
									ManualResetEventSlim[y].Set();
								}
							}
						}
					}
				})).Start();
			}
			// Iterate through each reset event.
			for (int x = 0; x < ThreadCount; x++) {
				// Wait on the reset event.
				ManualResetEventSlim[x].Wait();
			}
			// Check if the exception has been set.
			if (Exception != null) {
				// Iterate through each thread.
				for (int x = 0; x < ThreadCount; x++) {
					// Abort the thread.
					Thread[x].Abort();
				}
				// Throw the exception.
				throw Exception;
			}
		}
		#endregion
	}
}