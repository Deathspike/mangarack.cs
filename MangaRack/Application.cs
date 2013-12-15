// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using CommandLine;
using ICSharpCode.SharpZipLib.Zip;
using MangaRack.Core;
using MangaRack.Provider;
using MangaRack.Provider.Batoto;
using MangaRack.Provider.KissManga;
using MangaRack.Provider.MangaFox;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MangaRack {
	/// <summary>
	/// Represents the application.
	/// </summary>
	static class Application {
		/// <summary>
		/// Contains each provider.
		/// </summary>
		private static IEnumerable<IProvider> _Providers;

		#region Constructor
		/// <summary>
		/// Initialize the Program class.
		/// </summary>
		static Application() {
			// Initialize each provider ...
			_Providers = new IProvider[] {
				// ... with Batoto support ...
				Factory.Create<Batoto>(),
				// ... with KissManga support ...
				Factory.Create<KissManga>(),
				// ... with MangaFox support.
				Factory.Create<MangaFox>()
			};
		}
		#endregion

		#region Methods
		/// <summary>
		/// Run in batch processing mode.
		/// </summary>
		/// <param name="Options">The options.</param>
		public static void Batch(Options Options) {
			// Check if the batch-mode source file does exist.
			if (File.Exists(Options.SourceFile)) {
				// Initialize a new instance of the List class.
				List<KeyValuePair<Options, string>> WorkerItems = new List<KeyValuePair<Options, string>>();
				// Iterate through each line in the source file.
				foreach (string Line in File.ReadAllLines(Options.SourceFile)) {
					// Initialize a new instance of the Options class.
					Options LineOptions = new Options();
					// Parse each command line argument into the options instance and check if an unique identifier is available.
					if (Parser.Default.ParseArguments(Line.Split(' '), LineOptions) && LineOptions.UniqueIdentifiers.Count != 0) {
						// Iterate through each unique identifier.
						foreach (string UniqueIdentifier in LineOptions.UniqueIdentifiers) {
							// Check if worker threads are not disabled.
							if (LineOptions.MaximumParallelWorkerThreads > 1 && Options.MaximumParallelWorkerThreads > 1) {
								// Add the unique identifier of the line to the worker item list.
								WorkerItems.Add(new KeyValuePair<Options, string>(LineOptions, UniqueIdentifier));
							} else {
								// Run in single processing mode.
								Single(LineOptions, UniqueIdentifier);
							}
						}
					}
				}
				// Check if worker items are available.
				if (WorkerItems.Count != 0) {
					// Iterate through each worker item.
					UnsafeParallel.For(0, WorkerItems.Count, Options.MaximumParallelWorkerThreads, i => {
						// Run in single processing mode.
						Single(WorkerItems[i].Key, WorkerItems[i].Value);
					});
				}
			}
		}

		/// <summary>
		/// Entry point of the application.
		/// </summary>
		/// <param name="Arguments">Each command line argument.</param>
		public static void Main(string[] Arguments) {
			// Initialize a new instance of the Options class.
			Options Options = new Options();
			// Occurs when an exception is not caught.
			AppDomain.CurrentDomain.UnhandledException += (sender, e) => {
				// Write the message.
				Console.WriteLine(e.ExceptionObject);
				// Check if the keep-alive behavior is not disabled.
				if (!Options.DisableKeepAliveBehavior) {
					// Write the message.
					Console.WriteLine("Press [ENTER] to exit.");
					// Prevent the console from closing.
					Console.ReadLine();
				}
				// Exit the application.
				Environment.Exit(0);
			};
			// Parse each command line argument into the options instance.
			if (Parser.Default.ParseArguments(Arguments, Options)) {
				// Initialize the begin time.
				long Time = DateTime.Now.Ticks;
				// Check if the maximum number of worker threads is invalid.
				if (Options.MaximumParallelWorkerThreads < 1) {
					// Set the maximum number of worker threads.
					Options.MaximumParallelWorkerThreads = 1;
				}
				// Check if no unique identifier is available.
				if (Options.UniqueIdentifiers.Count == 0) {
					// Run in batch processing mode.
					Batch(Options);
				} else {
					// Check if worker threads are not disabled.
					if (Options.MaximumParallelWorkerThreads > 1) {
						// Iterate through each unique identifier.
						UnsafeParallel.For(0, Options.UniqueIdentifiers.Count, Options.MaximumParallelWorkerThreads, i => {
							// Run in single processing mode for the unique identifier.
							Single(Options, Options.UniqueIdentifiers[i]);
						});
					} else {
						// Iterate through each unique identifier.
						foreach (string UniqueIdentifier in Options.UniqueIdentifiers) {
							// Run in single processing mode for the unique identifier.
							Single(Options, UniqueIdentifier);
						}
					}
				}
				// Check if the total elapsed time notification is not disabled.
				if (!Options.DisableTotalElapsedTime) {
					// Calculate the elapsed time.
					TimeSpan Elapsed = new TimeSpan(DateTime.Now.Ticks - Time);
					// Write the message.
					Console.WriteLine("Completed ({0}:{1}:{2})!", Elapsed.Hours.ToString("00"), Elapsed.Minutes.ToString("00"), Elapsed.Seconds.ToString("00"));
				}
				// Check if the keep-alive behavior is not disabled.
				if (!Options.DisableKeepAliveBehavior) {
					// Write the message.
					Console.WriteLine("Press [ENTER] to exit.");
					// Prevent the console from closing.
					Console.ReadLine();
				}
			}
		}

		/// <summary>
		/// Run in single processing mode for the unique identifier.
		/// </summary>
		/// <param name="Options">The collection of options.</param>
		/// <param name="UniqueIdentifier">The unique identifier.</param>
		public static void Single(Options Options, string UniqueIdentifier) {
			// Select the provider.
			IProvider Provider = _Providers.FirstOrDefault(x => x.Open(UniqueIdentifier) != null);
			// Check if the provider is valid.
			if (Provider != null) {
				// Initialize the series.
				using (ISeries Series = Provider.Open(UniqueIdentifier)) {
					// Populate the series.
					using (Series.Populate()) {
						// Initialize the series title.
						string Title = Series.Title.InvalidatePath();
						// Initialize the persistence.
						List<string> Persistence = new List<string>();
						// Initialize the persistence file path.
						string PersistencePath = Path.Combine(Title, Title + ".txt");
						// Check if persistent synchronization tracking is enabled and a tracking file is available.
						if (Options.EnablePersistentSynchronization && File.Exists(PersistencePath)) {
							// Iterate through each line in the persistence file.
							foreach(string Line in File.ReadAllLines(PersistencePath)){
								// Add the line to the persistence file names.
								Persistence.Add(Line);
							}
						}
						// Iterate through each chapter using the chapter and volume filters.
						foreach (IChapter Chapter in Series.Children.Filter(Options)) {
							// Initialize the file name.
							string FileName = string.Format(Chapter.Volume == -1 ? "{0} #{2}.{3}" : "{0} V{1} #{2}.{3}", Title, Chapter.Volume.ToString("00"), Chapter.Number.ToString("000.####"), Options.FileExtension.InvalidatePath());
							// Initialize the file path.
							string FilePath = Path.Combine(Title, FileName);
							// Check if persistent synchronization tracking is enabled and the file name is persisted.
							if (Options.EnablePersistentSynchronization && Persistence.Contains(FileName)) {
								// Continue to the next chapter.
								continue;
							} else {
								// Add the file name to the persistence file names.
								Persistence.Add(FileName);
							}
							// Check if the file should be synchronized.
							if (Options.DisableDuplicationPrevention || !File.Exists(FilePath)) {
								// Populate the chapter.
								using (Chapter.Populate()) {
									// Initialize a new instance of the Publisher class.
									using (Publisher Publisher = new Publisher(FilePath, Options, Provider)) {
										// Initialize a new instance of the Synchronizer class.
										using (Synchronize Synchronizer = new Synchronize(Publisher, Series, Chapter)) {
											// Populate synchronously.
											Synchronizer.Populate();
										}
									}
								}
							} else if (!Options.DisableRepairAndErrorTracking && File.Exists(string.Format("{0}.txt", FilePath))) {
								// Populate the chapter.
								using (Chapter.Populate()) {
									// Initialize whether repairing has failed.
									bool HasFailed = false;
									// Repeat the following code while repair is failing.
									do {
										// Initialize the comic information.
										ComicInfo ComicInfo = null;
										// Initialize whether there are broken pages.
										bool HasBrokenPages = false;
										// Initialize a new instance of the ZipFile class.
										using (ZipFile ZipFile = new ZipFile(FilePath)) {
											// Find the comic information.
											ZipEntry ZipEntry = ZipFile.GetEntry("ComicInfo.xml");
											// Check if comic information is available.
											if (ZipEntry == null) {
												// Stop the function.
												return;
											} else {
												// Load the comic information.
												ComicInfo = ComicInfo.Load(ZipFile.GetInputStream(ZipEntry));
											}
										}
										// Initialize a new instance of the Publisher class.
										using (Publisher Publisher = new Publisher(FilePath, Options, Provider, true)) {
											// Initialize a new instance of the Repair class.
											using (Repair Repair = new Repair(Publisher, Series, Chapter, ComicInfo, File.ReadAllLines(string.Format("{0}.txt", FilePath)))) {
												// Populate synchronously.
												Repair.Populate();
												// Set whether there are broken pages.
												HasBrokenPages = Publisher.HasBrokenPages;
												// Set whether repairing has failed.
												HasFailed = Publisher.HasFailed = Repair.HasFailed;
											}
										}
										// Check if there are no broken pages.
										if (!HasBrokenPages) {
											// Delete the error file.
											File.Delete(string.Format("{0}.txt", FilePath));
										}
									} while (HasFailed);
								}
							}
							// Check if persistent synchronization tracking is enabled.
							if (Options.EnablePersistentSynchronization) {
								// Write each line to the persistence file path.
								File.WriteAllLines(PersistencePath, Persistence);
							}
						}
					}
				}
			}
		}
		#endregion
	}
}