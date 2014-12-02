﻿// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using TinyHttp;

namespace MangaRack.Provider.Batoto {
	/// <summary>
	/// Represents a Batoto search.
	/// </summary>
	sealed class Search : ISearch {
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
		public void Populate(Action<ISearch> done) {
			// Get the document.
			Http.Get(Provider.Domain + "search?name=" + Uri.EscapeDataString(Input), Response => {
				// Initialize a new instance of the HtmlDocument class.
				var htmlDocument = new HtmlDocument();
				// Load the document.
				htmlDocument.LoadHtml(Response.AsString());
				// Find each anchor element ...
				Children = htmlDocument.DocumentNode.Descendants("a")
					// ... with a references indicating a series ...
					.Where(x => HtmlEntity.DeEntitize(x.GetAttributeValue("href", string.Empty)).Trim().Contains("/comic/"))
					// ... select the results ...
					.Select(x => new Series(HtmlEntity.DeEntitize(x.Attributes["href"].Value).Trim(), HtmlEntity.DeEntitize(x.InnerText).Trim()) as ISeries)
					// ... and create an array.
					.ToArray();
				// Invoke the callback.
				done(this);
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
	}
}