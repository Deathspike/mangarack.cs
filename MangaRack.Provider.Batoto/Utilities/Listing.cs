// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace MangaRack.Provider.Batoto {
	/// <summary>
	/// Represents a listing.
	/// </summary>
	public sealed class Listing {
		#region Constructor
		/// <summary>
		/// Initialize a new instance of the Listing class.
		/// </summary>
		/// <param name="InputNumber">The input number.</param>
		public Listing(string InputNumber) {
			// Initialize the parsed number.
			double ParsedNumber;
			// Initialize the separation index.
			int SeparationIndex = InputNumber.IndexOf('-');
			// Set the number.
			Number = double.TryParse(SeparationIndex == -1 ? InputNumber : InputNumber.Substring(0, SeparationIndex), out ParsedNumber) ? ParsedNumber : -1;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Contains the chapter.
		/// </summary>
		public HtmlNode Chapter { get; set; }

		/// <summary>
		/// Cotains the number.
		/// </summary>
		public double Number { get; set; }

		/// <summary>
		/// Contains the match.
		/// </summary>
		public Match Match { get; set; }

		/// <summary>
		/// Contains the volume.
		/// </summary>
		public double Volume { get; set; }

		/// <summary>
		/// Contains the title.
		/// </summary>
		public string Title { get; set; }
		#endregion

		#region Methods
		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		public override string ToString() {
			// Return a string that represents the current object.
			return string.Format("V{0} #{1}", Volume.ToString("00"), Number.ToString("00.#"));
		}
		#endregion
	}
}