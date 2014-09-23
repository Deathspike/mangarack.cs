// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using HtmlAgilityPack;

namespace MangaRack.Provider.KissManga.Extension {
	/// <summary>
	/// Represents the class providing extensions for the HtmlNode class.
	/// </summary>
	static class ExtensionForHtmlNode {
		#region Methods
		/// <summary>
		/// Retrieve the next element.
		/// </summary>
		/// <param name="node">The node.</param>
		public static HtmlNode NextElement(this HtmlNode node) {
			// Initialize the sibling.
			var sibling = null as HtmlNode;
			// Iterate through each sibling.
			while ((sibling = sibling == null ? node.NextSibling : sibling.NextSibling) != null) {
				// Check if this is an element.
				if (sibling.NodeType == HtmlNodeType.Element) {
					// Return the result.
					return sibling;
				}
			}
			// Return the result.
			return node;
		}
		#endregion
	}
}