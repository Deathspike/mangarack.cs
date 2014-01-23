// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using ICSharpCode.SharpZipLib.Zip;
using MangaRack.Core;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;

namespace MangaRack.Analyze {
	/// <summary>
	/// Represents the application.
	/// </summary>
	static class Application {
		/// <summary>
		/// Contains the number of archives containing damaged meta-information.
		/// </summary>
		private static int _NumberOfDamaged;

		/// <summary>
		/// Contains the number of archives lacking meta-information.
		/// </summary>
		private static int _NumberOfMissing;

		#region Methods
		/// <summary>
		/// Inspect the path.
		/// </summary>
		/// <param name="InspectPath">The path.</param>
		/// <param name="PerformDelete">Indicates whether delete is requested.</param>
		public static void Inspect(string InspectPath, bool PerformDelete) {
			// Iterate through each directory.
			foreach (string DirectoryPath in Directory.GetDirectories(InspectPath)) {
				// Inspect the directory.
				Inspect(DirectoryPath, PerformDelete);
			}
			// Iterate through each archive file.
			foreach (string FilePath in Directory.GetFiles(InspectPath).Where(x => x.EndsWith(".cbz"))) {
				// Read the comic information.
				ComicInfo ComicInfo = Read(FilePath);
				// Check if comic information is available.
				if (ComicInfo != null) {
					// Check if the series is invalid.
					if (string.IsNullOrEmpty(ComicInfo.Series)) {
						// Write the message.
						Console.WriteLine("Damaged: {0}", Path.GetFileNameWithoutExtension(FilePath));
						// Increment the number of archives containing damaged meta-information.
						_NumberOfDamaged++;
					}
				} else {
					// Write the message.
					Console.WriteLine("Missing: {0}", Path.GetFileNameWithoutExtension(FilePath));
					// Increment the number of archives lacking meta-information.
					_NumberOfMissing++;
				}
			}
		}

		/// <summary>
		/// Entry point of the application.
		/// </summary>
		/// <param name="Arguments">Each command line argument.</param>
		public static void Main(string[] Arguments) {
			// Set the thread culture.
			Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
			// Occurs when an exception is not caught.
			AppDomain.CurrentDomain.UnhandledException += (sender, e) => {
				// Write the message.
				Console.WriteLine(e.ExceptionObject);
				// Check if the keep-alive behavior is not disabled.
				if (true) {
					// Write the message.
					Console.WriteLine("Press [ENTER] to exit.");
					// Prevent the console from closing.
					Console.ReadLine();
				}
				// Exit the application.
				Environment.Exit(0);
			};
			// Initialize the begin time.
			long Time = DateTime.Now.Ticks;
			// Inspect the current directory.
			Inspect(Directory.GetCurrentDirectory(), false);
			// Write the message.
			Console.WriteLine("Total Damaged: {0}", _NumberOfDamaged);
			// Write the message.
			Console.WriteLine("Total Missing: {0}", _NumberOfMissing);
			// Check if the total elapsed time notification is not disabled.
			if (true) {
				// Calculate the elapsed time.
				TimeSpan Elapsed = new TimeSpan(DateTime.Now.Ticks - Time);
				// Write the message.
				Console.WriteLine("Completed ({0}:{1}:{2})!", Elapsed.Hours.ToString("00"), Elapsed.Minutes.ToString("00"), Elapsed.Seconds.ToString("00"));
			}
			// Check if the keep-alive behavior is not disabled.
			if (true) {
				// Write the message.
				Console.WriteLine("Press [ENTER] to exit.");
				// Prevent the console from closing.
				Console.ReadLine();
			}
		}

		/// <summary>
		/// Read comic information.
		/// </summary>
		/// <param name="FilePath">The file path.</param>
		public static ComicInfo Read(string FilePath) {
			// Attempt the following code.
			try {
				// Initialize a new instance of the ZipFile class.
				using (ZipFile ZipFile = new ZipFile(FilePath)) {
					// Find the comic information.
					ZipEntry ZipEntry = ZipFile.GetEntry("ComicInfo.xml");
					// Check if comic information is available.
					if (ZipEntry == null) {
						// Stop the function.
						return null;
					} else {
						// Load the comic information.
						return ComicInfo.Load(ZipFile.GetInputStream(ZipEntry));
					}
				}
			} catch {
				// Return null.
				return null;
			}
		}
		#endregion
	}
}