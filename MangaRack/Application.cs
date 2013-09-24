// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using CommandLine;
using ICSharpCode.SharpZipLib.Zip;
using MangaRack.Provider;
using MangaRack.Provider.Batoto;
using MangaRack.Provider.KissManga;
using MangaRack.Provider.MangaFox;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
			// Initialize the application name.
			string ApplicationName = Assembly.GetExecutingAssembly().GetName().Name;
			// Initialize the file name.
			string FileName = string.Format("{0}.txt", ApplicationName);
			// Check if the file does exist.
			if (File.Exists(FileName)) {
				// Initialize a new instance of the List class.
				List<string> ParallelItems = new List<string>();
				// Initialize a new instance of the FileStream class.
				using (FileStream FileStream = File.Open(FileName, FileMode.Open, FileAccess.Read, FileShare.Read)) {
					// Initialize a new instance of the StreamReader class.
					using (StreamReader StreamReader = new StreamReader(FileStream, Encoding.UTF8)) {
						// Read all lines from the file stream.
						foreach (string Line in StreamReader.ReadLines()) {
							// Initialize a new instance of the Options class.
							Options LineOptions = new Options();
							// Parse each command line argument into the options instance and check if an unique identifier is available.
							if (Parser.Default.ParseArguments(Line.Split(' '), LineOptions) && LineOptions.UniqueIdentifiers.Count != 0) {
								// Check if worker processes are not disabled.
								if (!LineOptions.DisableWorkerProcesses && !Options.DisableWorkerProcesses) {
									// Add the line to the parallel item list.
									ParallelItems.Add(Line);
								} else {
									// Run in single processing mode.
									Single(LineOptions);
								}
							}
						}
					}
				}
				// Check if worker processes are not disabled.
				if (!Options.DisableWorkerProcesses) {
					// Iterate through each parallel item.
					Parallel.For(0, ParallelItems.Count, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, i => {
						// Initialize a new instance of the ProcessStartInfo class.
						ProcessStartInfo ProcessStartInfo = new ProcessStartInfo(string.Format("{0}.exe", ApplicationName), string.Format("-tk {0}", ParallelItems[i])) {
							// Prevent the process from creating a window.
							CreateNoWindow = true,
							// Redirect the standard output.
							RedirectStandardOutput = true,
							// Disable shell execution.
							UseShellExecute = false
						};
						// Start the process and read all lines from the process stream.
						foreach (string Line in Process.Start(ProcessStartInfo).StandardOutput.ReadLines()) {
							// Write the line.
							Console.WriteLine(Line);
						}
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
			// Parse each command line argument into the options instance.
			if (Parser.Default.ParseArguments(Arguments, Options)) {
				// Initialize the begin time.
				long Time = DateTime.Now.Ticks;
				// Check if no unique identifier is available.
				if (Options.UniqueIdentifiers.Count == 0) {
					// Run in batch processing mode.
					Batch(Options);
				} else {
					// Check if worker processes are not disabled.
					if (!Options.DisableWorkerProcesses) {
						// Initialize the parent process.
						Process ParentProcess = Parent.GetParentProcess();
						// Check if the parent process is valid.
						if (ParentProcess != null) {
							// Occurs when a process exits.
							ParentProcess.Exited += (sender, e) => {
								// Kill the current process.
								Process.GetCurrentProcess().Kill();
							};
							// Enable raising events.
							ParentProcess.EnableRaisingEvents = true;
						}
					}
					// Run in single processing mode.
					Single(Options);
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
		/// Run in single processing mode.
		/// </summary>
		/// <param name="Options">The collection of options.</param>
		public static void Single(Options Options) {
			// Iterate through each unique identifier.
			foreach (string UniqueIdentifier in Options.UniqueIdentifiers) {
				// Select the provider.
				IProvider Provider = _Providers.FirstOrDefault(x => x.Open(UniqueIdentifier) != null);
				// Check if the provider is valid.
				if (Provider != null) {
					// Initialize the series.
					using (ISeries Series = Provider.Open(UniqueIdentifier).Populate()) {
						// Initialize the series title.
						string Title = Series.Title.InvalidatePath();
						// Iterate through each chapter using the chapter and volume filters.
						foreach (IChapter Chapter in Series.Chapters.Filter(Options)) {
							// Use the chapter and dispose of it when done.
							using (Chapter) {
								// Initialize the file name.
								string FileName = string.Format(Chapter.Volume == -1 ? "{0} #{2}.{3}" : "{0} V{1} #{2}.{3}", Title, Chapter.Volume.ToString("00"), Chapter.Number.ToString("000.####"), Options.FileExtension.InvalidatePath());
								// Initialize the file path.
								string FilePath = Path.Combine(Title, FileName);
								// Check if the file should be downloaded.
								if (Options.DisableDuplicationPrevention || !File.Exists(FilePath)) {
									// Initialize a new instance of the List class.
									List<Meta> MetaInformation = new List<Meta>();
									// Initialize the temporary file path.
									string TempFilePath = Path.GetTempFileName();
									// Initialize the time.
									long Time = DateTime.Now.Ticks;
									// Initialize a new instance of the FileStream class.
									using (FileStream FileStream = File.Open(TempFilePath, FileMode.Create)) {
										// Set the temporary file path.
										TempFilePath = FileStream.Name;
										// Write the message.
										Console.WriteLine("Fetching {0}", FileName);
										// Initialize a new instance of the ZipOutputStream class.
										using (ZipOutputStream ZipOutputStream = new ZipOutputStream(FileStream)) {
											// Initialize the page number.
											int PageNumber = 1;
											// Check if the preview image is valid, write it to the stream and add meta-information to the collection.
											if (Series.PreviewImage != null && Utilities.Write(MetaInformation, ZipOutputStream, Series.PreviewImage)) {
												// Set the image type for the meta-information.
												MetaInformation[0].Type = "Cover";
											}
											// Iterate through each page.
											foreach (IPage Page in Chapter.Populate().Pages.Select(x => x.Populate())) {
												// Use the page and dispose of it when done.
												using (Page) {
													// Write the processed image to the stream and add meta-information to the collection.
													if (!Utilities.Write(MetaInformation, ZipOutputStream, Utilities.Image(Options, Provider, Page.Image))) {
														// Write the message.
														Console.WriteLine("Broken page #{0} in {1}", PageNumber.ToString("000"), FileName);
													}
													// Increment the page number.
													PageNumber++;
												}
											}
											// Check if meta-information is not disabled.
											if (!Options.DisableMetaInformation) {
												// Write the meta-information to the stream.
												Utilities.Write(MetaInformation, ZipOutputStream, Series, Chapter);
											}
										}
									}
									// Check if the series directory does not exist.
									if (!Directory.Exists(Title)) {
										// Create the directory for the series.
										Directory.CreateDirectory(Title);
									}
									// Move the temporary file to the file path.
									File.Copy(TempFilePath, FilePath, true);
									// Delete the temporary file.
									File.Delete(TempFilePath);
									// Check if a page has been processed.
									if (MetaInformation.Count != 0) {
										// Initialize the elapsed time.
										TimeSpan Elapsed = new TimeSpan(DateTime.Now.Ticks - Time);
										// Write the message.
										Console.WriteLine("Finished {0} ({1}:{2}, {3}s/Page)", FileName, Elapsed.Minutes.ToString("00"), Elapsed.Seconds.ToString("00"), (Elapsed.TotalSeconds / MetaInformation.Count).ToString("0.0"));
									}
								}
							}
						}
					}
				}
			}
		}
		#endregion
	}
}