// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using MangaRack.Provider;
using System;

namespace MangaRack {
	/// <summary>
	/// Represents a writer.
	/// </summary>
	public interface IWriter : IDisposable {
		/// <summary>
		/// Indicates whether the writer can continue.
		/// </summary>
		bool CanContinue();

		/// <summary>
		/// Write the series.
		/// </summary>
		/// <param name="Series">The series.</param>
		/// <param name="Chapter">The chapter.</param>
		/// <param name="Volume">The volume.</param>
		int Write(ISeries Series, IChapter Chapter, double Volume);
	}
}