// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace MangaRack {
	/// <summary>
	/// Represents the program.
	/// </summary>
	public static class Program {
		/// <summary>
		/// Indicates whether this is a single instance.
		/// </summary>
		private static readonly bool _IsSingleInstance;

		/// <summary>
		/// Contains the mutex.
		/// </summary>
		private static readonly Mutex _Mutex;

		/// <summary>
		/// Contains the name.
		/// </summary>
		private static readonly string _Name;

		/// <summary>
		/// Initialize the program.
		/// </summary>
		static Program() {
			// Initialize the name
			_Name = Assembly.GetExecutingAssembly().GetName().Name;
			// Initialize the mutex.
			_Mutex = new Mutex(false, _Name + "dbg", out _IsSingleInstance);
		}

		/// <summary>
		/// Entry point of the application.
		/// </summary>
		/// <param name="Arguments">Each command line argument.</param>
		public static void Main(string[] Arguments) {
			// Occurs when an exception is not caught.
			AppDomain.CurrentDomain.UnhandledException += (sender, e) => {
				// Write the exception.
				Console.Write(e.ExceptionObject);
				// Check if this process is a slave.
				if (!Debugger.IsAttached && Arguments.Length >= 2) {
					// Exit the application.
					Environment.Exit(0);
				}
			};
			// Check if this process is a slave.
			if (Arguments.Length >= 2) {
				// Initialize the parent identifier.
				int ParentId;
				// Attempt the parse the parent identifier.
				if (int.TryParse(Arguments[0], out ParentId)) {
					// Initialize a new instance of the Runner class.
					IRunner Runner = new Runner(Process.GetProcessById(ParentId), _Mutex, _Name);
					// Set the current culture for the thread.
					Thread.CurrentThread.CurrentCulture = new CultureInfo("en-us");
					// Run the application.
					Runner.Run(string.Join(null, Arguments, 1, Arguments.Length - 1));
				}
				// Return.
				return;
			}
			// Check if the unique identifiers file is available.
			if (_IsSingleInstance && File.Exists(string.Format("{0}.txt", _Name))) {
				// Initialize the completed work.
				var CompletedWork = 0;
				// Initialize the processed work.
				var ProcessedWork = 0;
				// Initialize the processes.
				var Processes = new Process[Environment.ProcessorCount];
				// Initialize the current time.
				var Time = DateTime.Now.Ticks;
				// Initialize the unique identifiers.
				var UniqueIdentifiers = File.ReadAllLines(string.Format("{0}.txt", _Name)).Where(x => !string.IsNullOrEmpty(x)).ToArray();
				// Iterate while the work has not been completed.
				while (CompletedWork < UniqueIdentifiers.Length) {
					// Iterate through each process.
					for (var i = 0; i < Processes.Length; i++) {
						// Check if the process has exited.
						if (Processes[i] != null && Processes[i].HasExited) {
							// Increment the completed work.
							CompletedWork++;
							// Remove the process.
							Processes[i] = null;
						}
						// Check if a process can be started.
						if (Processes[i] == null && ProcessedWork < UniqueIdentifiers.Length) {
							// Initialize the process start information.
							var ProcessStartInfo = new ProcessStartInfo(string.Format("{0}.exe", _Name), string.Format("{0} {1}", Process.GetCurrentProcess().Id, UniqueIdentifiers[ProcessedWork]));
							// Prevent the process from creating a window.
							ProcessStartInfo.CreateNoWindow = true;
							// Redirect the standard output.
							ProcessStartInfo.RedirectStandardOutput = true;
							// Disable shell execution.
							ProcessStartInfo.UseShellExecute = false;
							// Start the process.
							Processes[i] = Process.Start(ProcessStartInfo);
							// Queue the process to a thread.
							ThreadPool.QueueUserWorkItem(new WaitCallback((object Data) => {
								// Retrieve the process.
								var CurrentProcess = (Process) Data;
								// Initialize the line.
								string Line = null;
								// Iterate through the response while a line is available.
								while ((Line = CurrentProcess.StandardOutput.ReadLine()) != null) {
									// Write the line.
									Console.WriteLine(Line);
								}
							}), Processes[i]);
							// Increment the processed work.
							ProcessedWork++;
						}
					}
					// Sleep the thread.
					Thread.Sleep(250);
				}
				// Calculate and write the elapsed time.
				if (true) {
					// Calculate the elapsed time.
					TimeSpan Elapsed = new TimeSpan(DateTime.Now.Ticks - Time);
					// Write the message.
					Console.WriteLine("Completed ({0}:{1}:{2})!", Elapsed.Hours.ToString("00"), Elapsed.Minutes.ToString("00"), Elapsed.Seconds.ToString("00"));
				}
				// Check if this is a Windows-based operating system and ensure the command window will remain visible.
				if (Environment.OSVersion.Platform != PlatformID.MacOSX && Environment.OSVersion.Platform != PlatformID.Unix) {
					// Write the message.
					Console.WriteLine("Press [ENTER] to terminate.");
					// Prevent the console from closing.
					Console.ReadLine();
				}
			}
		}
	}
}