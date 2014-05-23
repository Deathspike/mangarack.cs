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
		private static int _numberOfDamaged;

		/// <summary>
		/// Contains the number of archives lacking meta-information.
		/// </summary>
		private static int _numberOfMissing;

		#region Methods
		/// <summary>
		/// Inspect the path.
		/// </summary>
		/// <param name="inspectPath">The path.</param>
		public static void Inspect(string inspectPath) {
			// Iterate through each directory.
			foreach (var directoryPath in Directory.GetDirectories(inspectPath)) {
				// Inspect the directory.
				Inspect(directoryPath);
			}
			// Iterate through each archive file.
			foreach (var filePath in Directory.GetFiles(inspectPath).Where(x => x.EndsWith(".cbz"))) {
				// Read the comic information.
				var comicInfo = Read(filePath);
				// Check if comic information is available.
				if (comicInfo != null) {
					// Check if the series is invalid.
					if (string.IsNullOrEmpty(comicInfo.Series)) {
						// Write the message.
						Console.WriteLine("Damaged: {0}", Path.GetFileNameWithoutExtension(filePath));
						// Increment the number of archives containing damaged meta-information.
						_numberOfDamaged++;
					}
				} else {
					// Write the message.
					Console.WriteLine("Missing: {0}", Path.GetFileNameWithoutExtension(filePath));
					// Increment the number of archives lacking meta-information.
					_numberOfMissing++;
				}
			}
		}

		/// <summary>
		/// Entry point of the application.
		/// </summary>
		/// <param name="arguments">Each command line argument.</param>
		public static void Main(string[] arguments) {
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
			// Parse each command line argument into the options instance.
			if (true) {
				// Initialize the begin time.
				var time = DateTime.Now.Ticks;
				// Inspect the current directory.
				Inspect(Directory.GetCurrentDirectory());
				// Write the message.
				Console.WriteLine("Total Damaged: {0}", _numberOfDamaged);
				// Write the message.
				Console.WriteLine("Total Missing: {0}", _numberOfMissing);
				// Check if the total elapsed time notification is not disabled.
				if (true) {
					// Calculate the elapsed time.
					var elapsed = new TimeSpan(DateTime.Now.Ticks - time);
					// Write the message.
					Console.WriteLine("Completed ({0}:{1}:{2})!", elapsed.Hours.ToString("00"), elapsed.Minutes.ToString("00"), elapsed.Seconds.ToString("00"));
				}
				// Check if the keep-alive behavior is not disabled.
				if (true) {
					// Write the message.
					Console.WriteLine("Press [ENTER] to exit.");
					// Prevent the console from closing.
					Console.ReadLine();
				}
			}
		}

		/// <summary>
		/// Read comic information.
		/// </summary>
		/// <param name="filePath">The file path.</param>
		public static ComicInfo Read(string filePath) {
			// Attempt the following code.
			try {
				// Initialize a new instance of the ZipFile class.
				using (var zipFile = new ZipFile(filePath)) {
					// Find the comic information.
					var zipEntry = zipFile.GetEntry("ComicInfo.xml");
					// Check if comic information is available.
					if (zipEntry == null) {
						// Stop the function.
						return null;
					} else {
						// Load the comic information.
						return ComicInfo.Load(zipFile.GetInputStream(zipEntry));
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