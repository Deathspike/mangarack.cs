// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using HtmlAgilityPack;

namespace MangaRack.Provider.KissManga {
	/// <summary>
	/// Represents the class providing extensions for the HtmlNode class.
	/// </summary>
	static class ExtensionForHtmlNode {
		#region Methods
		/// <summary>
		/// Retrieve the next element.
		/// </summary>
		/// <param name="Node">The node.</param>
		public static HtmlNode NextElement(this HtmlNode Node) {
			// Initialize the sibling.
			HtmlNode Sibling = null;
			// Iterate through each sibling.
			while ((Sibling = Sibling == null ? Node.NextSibling : Sibling.NextSibling) != null) {
				// Check if this is an element.
				if (Sibling.NodeType == HtmlNodeType.Element) {
					// Return the result.
					return Sibling;
				}
			}
			// Return the result.
			return Node;
		}
		#endregion
	}
}