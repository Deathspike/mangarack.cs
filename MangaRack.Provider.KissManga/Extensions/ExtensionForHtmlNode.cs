// ======================================================================
using HtmlAgilityPack;

namespace MangaRack.Provider.KissManga.Extensions {
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
			var sibling = null as HtmlNode;
			while ((sibling = sibling == null ? node.NextSibling : sibling.NextSibling) != null) {
				if (sibling.NodeType == HtmlNodeType.Element) {
					return sibling;
				}
			}
			return node;
		}
		#endregion
	}
}