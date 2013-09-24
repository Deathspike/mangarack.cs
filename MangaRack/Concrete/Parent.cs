// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace MangaRack {
	/// <summary>
	/// A utility class to determine a process parent.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct Parent {
		#region PROCESS_BASIC_INFORMATION
		internal IntPtr Reserved1;
		internal IntPtr PebBaseAddress;
		internal IntPtr Reserved2_0;
		internal IntPtr Reserved2_1;
		internal IntPtr UniqueProcessId;
		internal IntPtr InheritedFromUniqueProcessId;
		#endregion

		#region NtQueryInformationProcess
		[DllImport("ntdll.dll")]
		private static extern int NtQueryInformationProcess(IntPtr processHandle, int processInformationClass, ref Parent processInformation, int processInformationLength, out int returnLength);
		#endregion

		#region Statics
		/// <summary>
		/// Retrieve the parent process of the current process.
		/// </summary>
		/// <returns>An instance of the Process class.</returns>
		public static Process GetParentProcess() {
			// Retrieve the parent process of specified process.
			return GetParentProcess(Process.GetCurrentProcess().Handle);
		}

		/// <summary>
		/// Retrieve the parent process of specified process.
		/// </summary>
		/// <param name="Id">The process identifier.</param>
		public static Process GetParentProcess(int Id) {
			// Retrieve the parent process of specified process.
			return GetParentProcess(Process.GetProcessById(Id).Handle);
		}

		/// <summary>
		/// Retrieve the parent process of a specified process.
		/// </summary>
		/// <param name="Handle">The process handle.</param>
		public static Process GetParentProcess(IntPtr Handle) {
			// Attempt the following code.
			try {
				// Initialize the parent.
				Parent Parent = new Parent();
				// Initialize the return length.
				int ReturnLength;
				// Query for information about the process.
				NtQueryInformationProcess(Handle, 0, ref Parent, Marshal.SizeOf(Parent), out ReturnLength);
				// Return the process.
				return Process.GetProcessById(Parent.InheritedFromUniqueProcessId.ToInt32());
			} catch (ArgumentException) {
				// Return null.
				return null;
			}
		}
		#endregion
	}
}