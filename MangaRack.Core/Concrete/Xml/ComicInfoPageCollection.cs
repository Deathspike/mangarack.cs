// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace MangaRack.Core {
	/// <summary>
	/// Represents a collection of comic page information.
	/// </summary>
	public sealed class ComicInfoPageCollection : List<ComicInfoPage>, IXmlSerializable {
		#region Constructor
		/// <summary>
		/// Initialize a new instance of the ComicInfoPageCollection class.
		/// </summary>
		public ComicInfoPageCollection() {
			// Stop the function.
			return;
		}

		/// <summary>
		/// Initialize a new instance of the ComicInfoPageCollection class.
		/// </summary>
		/// <param name="Collection">The collection.</param>
		public ComicInfoPageCollection(IEnumerable<ComicInfoPage> Collection) {
			// Iterate through each page.
			foreach (ComicInfoPage ComicInfoPage in Collection) {
				// Add the page.
				Add(ComicInfoPage);
			}
		}
		#endregion

		#region IXmlSerializable
		/// <summary>
		/// This method is reserved and should not be used.
		/// </summary>
		public XmlSchema GetSchema() {
			// Throw an exception.
			throw new NotImplementedException();
		}

		/// <summary>
		/// Generates an object from its XML representation.
		/// </summary>
		/// <param name="XmlReader">The stream from which the object is deserialized.</param>
		public void ReadXml(XmlReader XmlReader) {
			// Initialize a new instance of the XmlSerializer class.
			XmlSerializer XmlSerializer = new XmlSerializer(typeof(ComicInfoPage));
			// Read into the children.
			XmlReader.Read();
			// Iterate through each child.
			while (XmlReader.Name.Equals("Page")) {
				// Add the deserialized page.
				Add(XmlSerializer.Deserialize(XmlReader) as ComicInfoPage);
			}
			// Read out of the children.
			XmlReader.Read();
		}

		/// <summary>
		/// Converts an object into its XML representation.
		/// </summary>
		/// <param name="XmlWriter">The stream to which the object is serialized.</param>
		public void WriteXml(XmlWriter XmlWriter) {
			// Check if an item is available.
			if (Count != 0) {
				// Initialize a new instance of the XmlSerializer class.
				XmlSerializer XmlSerializer = new XmlSerializer(typeof(ComicInfoPage));
				// Iterate through each page.
				foreach (ComicInfoPage ComicInfoPage in this.OrderBy(x => x.Image)) {
					// Add the serialized page.
					XmlSerializer.Serialize(XmlWriter, ComicInfoPage);
				}
			}
		}
		#endregion
	}
}