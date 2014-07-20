// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System.Xml.Serialization;

namespace MangaRack.Core {
	/// <summary>
	/// Represents comic page information.
	/// </summary>
	[XmlRoot("Page")]
	public class ComicInfoPage {
		#region Properties
		/// <summary>
		/// Contains the key.
		/// </summary>
		[XmlAttribute]
		public string Key { get; set; }

		/// <summary>
		/// Contains the image.
		/// </summary>
		[XmlAttribute]
		public int Image { get; set; }

		/// <summary>
		/// Contains the image height.
		/// </summary>
		[XmlAttribute]
		public int ImageHeight { get; set; }

		/// <summary>
		/// Contains the image size.
		/// </summary>
		[XmlAttribute]
		public int ImageSize { get; set; }

		/// <summary>
		/// Contains the image width.
		/// </summary>
		[XmlAttribute]
		public int ImageWidth { get; set; }

		/// <summary>
		/// Contains the type.
		/// </summary>
		[XmlAttribute]
		public string Type { get; set; }
		#endregion
	}
}