// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using MangaRack.Provider;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace MangaRack {
	/// <summary>
	/// Represents a runner.
	/// </summary>
	public sealed class Runner : IRunner {
		/// <summary>
		/// Contains the mutex.
		/// </summary>
		private readonly Mutex _Mutex;

		/// <summary>
		/// Contains the name.
		/// </summary>
		private readonly string _Name;

		/// <summary>
		/// Contains the parent process.
		/// </summary>
		private readonly Process _Process;

		/// <summary>
		/// Contains each provider.
		/// </summary>
		private readonly IEnumerable<IProvider> _Providers;

		/// <summary>
		/// Contains the storage.
		/// </summary>
		private readonly IStorage _Storage;

		#region Constructor
		/// <summary>
		/// Initialize a new instance of the Runner class.
		/// </summary>
		/// <param name="Process">The parent process.</param>
		/// <param name="Mutex">The mutex.</param>
		/// <param name="Name">The name.</param>
		public Runner(Process Process, Mutex Mutex, string Name) {
			// Set the mutex.
			_Mutex = Mutex;
			// Set the name.
			_Name = Name;
			// Set the parent process.
			_Process = Process;
			// Find each component from libraries in the current directory ...
			_Providers = Directory.GetFiles(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
				// ... where the libraries are valid ...
				.Where(y => y.EndsWith(".dll"))
				// ... select each available type ...
				.SelectMany(y => Assembly.LoadFile(y).GetTypes())
				// ... union with each available type from loaded libraries ...
				.Union(AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes()))
				// ... with the IProvider interface ...
				.Where(x => x != typeof(Program) && x.GetInterface(typeof(IProvider).FullName) != null)
				// ... and select an instance of each.
				.Select(x => Activator.CreateInstance(x) as IProvider);
			// Initialize the storage.
			_Storage = new Storage(string.Format("{0}.json", _Name));
		}
		#endregion

		#region IRunner
		/// <summary>
		/// Run the application.
		/// </summary>
		/// <param name="UniqueIdentifier">The unique identifier</param>
		public void Run(string UniqueIdentifier) {
			// Retrieve a series.
			using (ISeries Series = _Providers.Select(x => x.Get(UniqueIdentifier)).FirstOrDefault(x => x != null)) {
				// Check if the series is valid.
				if (Series != null) {
					// Initialize the previous number and previous volume.
					double PreviousNumber = -1, PreviousVolume = -1;
					// Initialize the next volume.
					double NextVolume = Series.Chapters.OrderByDescending(x => x.Volume).Select(x => x.Volume).FirstOrDefault() + 1;
					// Initialize the offset.
					double Offset = 0;
					// Iterate through each manga chapter using the volume order.
					foreach (IChapter Chapter in Series.Chapters.OrderBy(x => x.Volume < 0 ? NextVolume : x.Volume)) {
						// Initialize the current number.
						double CurrentNumber = Chapter.Number;
						// Initialize the current volume.
						double CurrentVolume = Chapter.Volume < 0 ? NextVolume : Chapter.Volume;
						// Check if the volume has been changed.
						if (PreviousNumber != -1 && PreviousVolume != -1 && CurrentVolume > PreviousVolume) {
							// Set the offset.
							Offset = CurrentNumber > PreviousNumber ? CurrentNumber - 1 : 0;
						}
						// Check if the number is valid.
						if (PreviousNumber == -1 || Offset < CurrentNumber) {
							// Initialize the (file) name.
							string Name = string.Format("V{0} #{1}", CurrentVolume.ToString("00"), (Chapter.Number - Offset).ToString("000.####"));
							// Check if the (file) name has not been persisted.
							if (!_Storage.Contains(UniqueIdentifier, Name)) {
								// Initialize the number of pages.
								int NumberOfPages = 0;
								// Initialize the current time.
								long Time = DateTime.Now.Ticks;
								// Initialize a new instance of the Writer class.
								using (IWriter Writer = new Writer(Series.Title, Name)) {
									// Check if the writer can continue.
									if (Writer.CanContinue()) {
										// Write the message.
										Console.WriteLine("Fetching {0} {1} ... ", Series.Title, Name);
										// Write the manga.
										NumberOfPages = Writer.Write(Series, Chapter, _Storage.Contains("MangaRack", "DisableWritingOfVolume") ? -1 : CurrentVolume);
									}
								}
								// Calculate and write the elapsed time.
								if (NumberOfPages != 0) {
									// Initialize the elapsed time.
									TimeSpan Elapsed = new TimeSpan(DateTime.Now.Ticks - Time);
									// Write the message.
									Console.WriteLine("Finished {0} {1} ({2}:{3}, {4}s/Page)!", Series.Title, Name, Elapsed.Minutes.ToString("00"), Elapsed.Seconds.ToString("00"), (Elapsed.TotalSeconds / NumberOfPages).ToString("0.0"));
								}
								// Update the persistance provider.
								if (true) {
									// Wait for the mutex.
									_Mutex.WaitOne();
									// Refresh the persistence provider.
									_Storage.Refresh();
									// Persist the (file) name.
									_Storage.Add(UniqueIdentifier, Name);
									// Save the persistence.
									_Storage.Save();
									// Release the mutex.
									_Mutex.ReleaseMutex();
								}
								// Check if the parent process has exited.
								if (!Debugger.IsAttached && _Process.HasExited) {
									// Return.
									return;
								}
							}
						}
						// Set the previous number.
						PreviousNumber = Chapter.Number;
						// Set the previous volume.
						PreviousVolume = Chapter.Volume;
						// Dispose of the object.
						Chapter.Dispose();
					}
					// Dispose of the object.
					Series.Dispose();
				}
			}
		}
		#endregion
	}
}