// ======================================================================
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MangaRack.Internals {
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
			var exception = (Exception)null;
			var manualResetEventSlim = new ManualResetEvent[threadCount];
			var thread = new Thread[threadCount];
			var queue = new Queue<int>(Enumerable.Range(fromInclusive, toExclusive));
			for (var x = 0; x < threadCount; x++) {
				var offset = x;
				manualResetEventSlim[offset] = new ManualResetEvent(false);
				(thread[offset] = new Thread(async () => {
					try {
						int i;
						thread[offset].CurrentCulture = new CultureInfo("en-US");
						while (true) {
							lock (queue) {
								if (queue.Count == 0) {
									break;
								}
								i = queue.Dequeue();
							}
							await body(i);
						}
						manualResetEventSlim[offset].Set();
					} catch (Exception e) {
						lock (thread) {
							if (exception == null) {
								exception = e;
								for (int y = 0; y < thread.Length; y++) {
									manualResetEventSlim[y].Set();
								}
							}
						}
					}
				})).Start();
			}
			for (var x = 0; x < threadCount; x++) {
				manualResetEventSlim[x].WaitOne();
			}
			if (exception != null) {
				for (var x = 0; x < threadCount; x++) {
					thread[x].Abort();
				}
				throw exception;
			}
		}
		#endregion
	}
}