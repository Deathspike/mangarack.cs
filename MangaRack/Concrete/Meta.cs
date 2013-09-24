// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================

namespace MangaRack {
	/// <summary>
	/// Represents meta-information.
	/// </summary>
	sealed class Meta {
		#region Properties
		/// <summary>
		/// Contains the image height.
		/// </summary>
		public int Height { get; set; }

		/// <summary>
		/// Contains the file name.
		/// </summary>
		public string FileName { get; set; }

		/// <summary>
		/// Contains the file size.
		/// </summary>
		public int Size { get; set; }

		/// <summary>
		/// Contains the image type.
		/// </summary>
		public string Type { get; set; }

		/// <summary>
		/// Contains the image height.
		/// </summary>
		public int Width { get; set; }
		#endregion
	}
}