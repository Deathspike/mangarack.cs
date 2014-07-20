// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System.Collections;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MangaRack.Provider.Interfaces;
using TinyHttp;

namespace MangaRack.Provider.MangaFox {
	/// <summary>
	/// Represents a MangaFox search.
	/// </summary>
	class Search : IEnumerable<ISeries> {
		#region Constructor
		/// <summary>
		/// Initialize a new instance of the Search class.
		/// </summary>
		/// <param name="input">The input.</param>
		public Search(string input) {
			// Set the input.
			Input = input;
		}
		#endregion

		#region IAsync
		/// <summary>
		/// Populate asynchronously.
		/// </summary>
		/// <param name="done">The callback.</param>
		public void Populate(Action done) {
			// Get the document.
			Http.Get(Provider.Domain + "search.php?advopts=1&name=" + Uri.EscapeDataString(Input), response => {
				// Initialize a new instance of the HtmlDocument class.
				var htmlDocument = new HtmlDocument();
				// Load the document.
				htmlDocument.LoadHtml(response.AsString());
				// Find each anchor element ...
				Children = htmlDocument.DocumentNode.Descendants("a")
					// ... with a references indicating a series ...
					.Where(x => Regex.Match(HtmlEntity.DeEntitize(x.GetAttributeValue("href", string.Empty)).Trim(), "/manga/([^/]+?)/?$", RegexOptions.IgnoreCase).Success)
					// ... select the results ...
					.Select(x => new Series(HtmlEntity.DeEntitize(x.Attributes["href"].Value).Trim(), HtmlEntity.DeEntitize(x.InnerText).Trim()) as ISeries)
					// ... and create an array.
					.ToArray();
				// Invoke the callback.
				done();
			});
		}
		#endregion

		#region IDisposable
		/// <summary>
		/// Dispose of the object.
		/// </summary>
		public void Dispose() {
			// Check if the children are valid.
			if (Children != null) {
				// Iterate through each child.
				foreach (var child in Children) {
					// Dispose of the object.
					child.Dispose();
				}
				// Remove the children.
				Children = null;
			}
		}
		#endregion

		#region ISearch
		/// <summary>
		/// Contains each child.
		/// </summary>
		public IEnumerable<ISeries> Children { get; private set; }

		/// <summary>
		/// Contains the input.
		/// </summary>
		public string Input { get; private set; }
		#endregion

        public IEnumerator<ISeries> GetEnumerator()
        {
            return Children.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
	}
}