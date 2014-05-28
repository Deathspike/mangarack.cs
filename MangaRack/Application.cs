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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;

namespace MangaRack {
	/// <summary>
	/// Represents the application.
	/// </summary>
	public static class Application {
		/// <summary>
		/// Contains each provider.
		/// </summary>
		private static IEnumerable<IProvider> _providers;

		#region Constructor
		/// <summary>
		/// Initialize the Program class.
		/// </summary>
		static Application() {
			// Initialize each provider ...
			_providers = new IProvider[] {
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
		/// <param name="options">The options.</param>
		public static bool Batch(Options options) {
			// Check if the batch-mode source file does exist.
			if (File.Exists(options.SourceFile)) {
				// Initialize a new instance of the List class.
				var workerItems = new List<KeyValuePair<Options, string>>();
				// Iterate through each line in the source file.
				foreach (var line in File.ReadAllLines(options.SourceFile)) {
					// Initialize a new instance of the Options class.
					var lineOptions = new Options();
					// Parse each command line argument into the options instance and check if a location is available.
					if (Parser.Default.ParseArguments(line.Split(' '), lineOptions) && lineOptions.Locations.Count != 0) {
						// Check if meta-information overwriting is enabled.
						if (options.EnableOverwriteMetaInformation) {
							// Enable meta-information overwriting.
							lineOptions.EnableOverwriteMetaInformation = true;
						}
						// Iterate through each location.
						foreach (var location in lineOptions.Locations) {
							// Check if worker threads are not disabled.
							if (lineOptions.MaximumParallelWorkerThreads > 1 && options.MaximumParallelWorkerThreads > 1) {
								// Add the location of the line to the worker item list.
								workerItems.Add(new KeyValuePair<Options, string>(lineOptions, location));
							} else {
								// Run in single processing mode.
								Single(location, lineOptions);
							}
						}
					}
				}
				// Check if worker items are available.
				if (workerItems.Count != 0) {
					// Iterate through each worker item.
					UnsafeParallel.For(0, workerItems.Count, options.MaximumParallelWorkerThreads, i => {
						// Run in single processing mode.
						Single(workerItems[i].Value, workerItems[i].Key);
					});
					// Return true.
					return true;
				}
			}
			// Return false.
			return false;
		}

		/// <summary>
		/// Entry point of the application.
		/// </summary>
		/// <param name="arguments">Each command line argument.</param>
		public static void Main(string[] arguments) {
			// Initialize a new instance of the Options class.
			var options = new Options();
			// Set the thread culture.
			Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
			// Occurs when an exception is not caught.
			AppDomain.CurrentDomain.UnhandledException += (sender, e) => {
				// Write the message.
				Console.WriteLine(e.ExceptionObject);
				// Check if the keep-alive behavior is not disabled.
				if (!options.DisableKeepAliveBehavior) {
					// Write the message.
					Console.WriteLine("Press [ENTER] to exit.");
					// Prevent the console from closing.
					Console.ReadLine();
				}
				// Exit the application.
				Environment.Exit(0);
			};
			// Parse each command line argument into the options instance.
			if (Parser.Default.ParseArguments(arguments, options)) {
				// Initialize the begin time.
				var time = DateTime.Now.Ticks;
				// Check if the maximum number of worker threads is invalid.
				if (options.MaximumParallelWorkerThreads < 1) {
					// Set the maximum number of worker threads.
					options.MaximumParallelWorkerThreads = 1;
				}
				// Check if no location is available.
				if (options.Locations.Count == 0) {
					// Run in batch processing mode.
					if (!Batch(options)) {
						// Write the message.
						Console.WriteLine("Breaking due to invalid source file.");
					}
				} else {
					// Check if worker threads are not disabled.
					if (options.MaximumParallelWorkerThreads > 1) {
						// Iterate through each location.
						UnsafeParallel.For(0, options.Locations.Count, options.MaximumParallelWorkerThreads, i => {
							// Run in single processing mode for the location.
							Single(options.Locations[i], options);
						});
					} else {
						// Iterate through each location.
						foreach (var location in options.Locations) {
							// Run in single processing mode for the location.
							Single(location, options);
						}
					}
				}
				// Check if the total elapsed time notification is not disabled.
				if (!options.DisableTotalElapsedTime) {
					// Calculate the elapsed time.
					var elapsed = new TimeSpan(DateTime.Now.Ticks - time);
					// Write the message.
					Console.WriteLine("Complete ({0}:{1}:{2})!", elapsed.Hours.ToString("00"), elapsed.Minutes.ToString("00"), elapsed.Seconds.ToString("00"));
				}
				// Check if the keep-alive behavior is not disabled.
				if (!options.DisableKeepAliveBehavior) {
					// Write the message.
					Console.WriteLine("Press [ENTER] to exit.");
					// Prevent the console from closing.
					Console.ReadLine();
				}
			}
		}

		/// <summary>
		/// Persist the state.
		/// </summary>
		/// <param name="persistencePath">The persistence path.</param>
		/// <param name="persistence">The persistence.</param>
		public static void Persist(string persistencePath, List<List<string>> persistence) {
			// Write each line to the persistence file path.
			File.WriteAllLines(persistencePath, persistence.Select(x => string.Join("\0", x.ToArray())).ToArray());
		}

		/// <summary>
		/// Run in single processing mode for the location.
		/// </summary>
		/// <param name="location">The location.</param>
		/// <param name="options">The collection of options.</param>
		public static void Single(string location, Options options) {
			// Select the provider.
			var provider = _providers.FirstOrDefault(x => x.Open(location) != null);
			// Check if the provider is valid.
			if (provider != null) {
				// Initialize the series.
				using (var series = provider.Open(location)) {
					// Populate the series.
					using (series.Populate()) {
						// Initialize the series title.
						var seriesTitle = series.Title.InvalidatePath();
						// Initialize the persistence file path.
						var persistencePath = Path.Combine(seriesTitle, ".mangarack-persist");
						// Initialize the persistence.
						var persistence = new List<List<string>>();
						// Check if a persistence file is available.
						if (File.Exists(persistencePath)) {
							// Initialize the persistence version.
							const int persistenceVersion = 2;
							// Iterate through each line in the persistence file.
							foreach (var line in File.ReadAllLines(persistencePath)) {
								// Initialize the pieces.
								var pieces = new List<string>(line.Split('\0'));
								// Ensure suffient pieces are available.
								while (pieces.Count < persistenceVersion) pieces.Add(string.Empty);
								// Add the pieces.
								persistence.Add(pieces);
							}
						}
						// Iterate through each chapter.
						foreach (var chapter in series.Children) {
							// Initialize the line matching the unique identifier.
							var line = persistence.Where(x => string.Equals(x[1], chapter.UniqueIdentifier)).FirstOrDefault();
							// Check if the line is available.
							if (line != null) {
								// Initialize the current file path.
								var currentFilePath = Path.Combine(seriesTitle, line[0]);
								// Initialize the next file name.
								var nextFileName = chapter.ToFileName(seriesTitle, options);
								// Check if the file names differs from the persistet file name.
								if (!string.Equals(line[0], nextFileName) && File.Exists(currentFilePath)) {
									// Rename the file.
									File.Move(currentFilePath, Path.Combine(seriesTitle, nextFileName));
									// Update the file name.
									line[0] = nextFileName;
									// Persist the state.
									Persist(persistencePath, persistence);
									// Write a message.
									Console.WriteLine("Switched {0}", nextFileName);
								}
							}
						}
						// Iterate through each chapter using the chapter and volume filters.
						foreach (var chapter in series.Children.Filter(options)) {
							// Initialize whether sychronization has failed.
							var hasFailed = false;
							// Initialize the file name.
							var fileName = chapter.ToFileName(seriesTitle, options);
							// Initialize the file path.
							var filePath = Path.Combine(seriesTitle, fileName);
							// Initialize the persistence file.
							var persistenceFile = persistence.FirstOrDefault(x => string.Equals(x[0], fileName));
							// Check if persistent synchronization is enabled and the file has been persistent.
							if (options.EnablePersistentSynchronization && persistenceFile != null) {
								// Continue to the next chapter.
								continue;
							} else if (persistenceFile != null) {
								// Update the unique identifier.
								persistenceFile[1] = chapter.UniqueIdentifier ?? string.Empty;
							} else {
								// Add the file name and unique identifier.
								persistence.Add(new List<string> { fileName, chapter.UniqueIdentifier ?? string.Empty });
							}
							// Do the following code.
							do {
								// Check if the file should be synchronized.
								if (options.DisableDuplicationPrevention || !File.Exists(filePath)) {
									// Populate the chapter.
									using (chapter.Populate()) {
										// Initialize a new instance of the Publisher class.
										using (var publisher = new Publisher(filePath, options, provider)) {
											// Initialize a new instance of the Synchronizer class.
											using (var synchronizer = new Synchronize(publisher, series, chapter)) {
												// Populate synchronously.
												synchronizer.Populate();
												// Set whether synchronization has failed.
												hasFailed = false;
											}
										}
									}
								} else {
									// Check if a meta-information overwrite should be performed.
									if (options.EnableOverwriteMetaInformation) {
										// Initialize the comic information.
										var comicInfo = new ComicInfo();
										// Initialize the previous comic information.
										var previousComicInfo = (ComicInfo)null;
										// Initialize a new instance of the ZipFile class.
										using (var zipFile = new ZipFile(filePath)) {
											// Find the comic information.
											var zipEntry = zipFile.GetEntry("ComicInfo.xml");
											// Check if comic information is available.
											if (zipEntry != null) {
												// Load the comic information.
												previousComicInfo = ComicInfo.Load(zipFile.GetInputStream(zipEntry));
												// Transcribe the series, chapter and pages information.
												comicInfo.Transcribe(series, chapter, previousComicInfo.Pages);
												// Check if a current genre differs ...
												if (comicInfo.Genre.Any(x => !previousComicInfo.Genre.Contains(x)) ||
													// ... or if a previous genre differs ...
													previousComicInfo.Genre.Any(x => !comicInfo.Genre.Contains(x)) ||
													// ... or the manga specification differs ...
													comicInfo.Manga != previousComicInfo.Manga ||
													// ... or the number differs ...
													comicInfo.Number != previousComicInfo.Number ||
													// ... or if the page count differs ...
													comicInfo.PageCount != previousComicInfo.PageCount ||
													// ... or if a current penciller difffers ...
													comicInfo.Penciller.Any(x => !previousComicInfo.Penciller.Contains(x)) ||
													// ... or if a previous penciller difffers ...
													previousComicInfo.Penciller.Any(x => !comicInfo.Penciller.Contains(x)) ||
													// ... or if the series differs ...
													comicInfo.Series != previousComicInfo.Series ||
													// ... or if the summary differs ...
													comicInfo.Summary != previousComicInfo.Summary ||
													// ... or if the title differs ...
													comicInfo.Title != previousComicInfo.Title ||
													// ... or if the volume differs ...
													comicInfo.Volume != previousComicInfo.Volume ||
													// ... or if a current writer differs.
													comicInfo.Writer.Any(x => !previousComicInfo.Writer.Contains(x)) ||
													// ... or if a previous writer differs.
													previousComicInfo.Writer.Any(x => !comicInfo.Writer.Contains(x))) {
													// Initialize a new instance of the MemoryStream class.
													using (var memoryStream = new MemoryStream()) {
														// Save the comic information.
														comicInfo.Save(memoryStream);
														// Rewind the stream.
														memoryStream.Position = 0;
														// Begin updating the compressed file.
														zipFile.BeginUpdate();
														// Add the file.
														zipFile.Add(new DataSource(memoryStream), "ComicInfo.xml");
														// End updating the compressed file.
														zipFile.CommitUpdate();
														// Write a message.
														Console.WriteLine("Modified {0}", fileName);
													}
												}
											}
										}
									}
									// Check if a repair should be performed.
									if (!options.DisableRepairAndErrorTracking && File.Exists(string.Format("{0}.txt", filePath))) {
										// Populate the chapter.
										using (chapter.Populate()) {
											// Initialize the comic information.
											var comicInfo = (ComicInfo)null;
											// Initialize whether there are broken pages.
											var hasBrokenPages = false;
											// Initialize a new instance of the ZipFile class.
											using (var zipFile = new ZipFile(filePath)) {
												// Find the comic information.
												var zipEntry = zipFile.GetEntry("ComicInfo.xml");
												// Check if comic information is available.
												if (zipEntry == null) {
													// Stop the function.
													return;
												} else {
													// Load the comic information.
													comicInfo = ComicInfo.Load(zipFile.GetInputStream(zipEntry));
												}
											}
											// Initialize a new instance of the Publisher class.
											using (var publisher = new Publisher(filePath, options, provider, true)) {
												// Initialize a new instance of the Repair class.
												using (var repair = new Repair(publisher, series, chapter, comicInfo, File.ReadAllLines(string.Format("{0}.txt", filePath)))) {
													// Populate synchronously.
													repair.Populate();
													// Set whether there are broken pages.
													hasBrokenPages = publisher.HasBrokenPages;
													// Set whether synchronization has failed.
													hasFailed = publisher.HasFailed = repair.HasFailed;
												}
											}
											// Check if there are no broken pages.
											if (!hasBrokenPages && File.Exists(string.Format("{0}.txt", filePath))) {
												// Delete the error file.
												File.Delete(string.Format("{0}.txt", filePath));
											}
										}
									}
								}
							} while (hasFailed);
							// Persist the state.
							Persist(persistencePath, persistence);
						}
					}
				}
			}
		}
		#endregion
	}
}