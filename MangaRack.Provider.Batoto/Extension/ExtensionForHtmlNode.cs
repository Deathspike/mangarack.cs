// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using HtmlAgilityPack;

namespace MangaRack.Provider.Batoto {
	/// <summary>
	/// Represents the class providing extensions for the HtmlNode class.
	/// </summary>
	public static class ExtensionForHtmlNode {
		/// <summary>
		/// Retrieve the last element.
		/// </summary>
		/// <param name="Node">The node.</param>
		public static HtmlNode LastElement(this HtmlNode Node) {
			// Initialize the result.
			HtmlNode Result = null;
			// Iterate through each child node.
			foreach (HtmlNode ChildNode in Node.ChildNodes) {
				// Check if this is an element.
				if (ChildNode.NodeType == HtmlNodeType.Element) {
					// Set the result.
					Result = ChildNode;
				}
			}
			// Return the result.
			return Result;
		}
	}
}