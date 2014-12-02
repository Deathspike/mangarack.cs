// ======================================================================
using HtmlAgilityPack;

namespace MangaRack.Provider.Batoto.Extensions {
	/// <summary>
	/// Represents the class providing extensions for the HtmlNode class.
	/// </summary>
	static class ExtensionForHtmlNode {
		#region Methods
		/// <summary>
		/// Retrieve the last element.
		/// </summary>
		/// <param name="node">The node.</param>
		public static HtmlNode LastElement(this HtmlNode node) {
			var result = null as HtmlNode;
			foreach (var childNode in node.ChildNodes) {
				if (childNode.NodeType == HtmlNodeType.Element) {
					result = childNode;
				}
			}
			return result;
		}
		#endregion
	}
}