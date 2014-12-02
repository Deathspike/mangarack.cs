// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System.Linq;
using HtmlAgilityPack;

namespace MangaRack.Provider.Batoto.Extension {
    /// <summary>
    /// Represents the class providing extensions for the HtmlNode class.
    /// </summary>
    internal static class ExtensionForHtmlNode {
        #region Methods
        /// <summary>
        /// Retrieve the last element.
        /// </summary>
        /// <param name="node">The node.</param>
        public static HtmlNode LastElement(this HtmlNode node) {
            var result = null as HtmlNode;
            foreach (var childNode in node.ChildNodes.Where(childNode => childNode.NodeType == HtmlNodeType.Element)) {
                result = childNode;
            }
            return result;
        }
        #endregion
    }
}