// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TinyHttp;

namespace MangaRack.Provider.KissManga {
	/// <summary>
	/// Represents a KissManga search.
	/// </summary>
	sealed class Search : ISearch {
		/// <summary>
		/// Contains the input.
		/// </summary>
		private readonly string _Input;

		#region Constructor
		/// <summary>
		/// Initialize a new instance of the Search class.
		/// </summary>
		/// <param name="Input">The input.</param>
		public Search(string Input) {
			// Set the unique identifier.
			_Input = Input;
		}
		#endregion

		#region IAsync
		/// <summary>
		/// Populate asynchronously.
		/// </summary>
		/// <param name="Done">The callback.</param>
		public void Populate(Action<ISearch> Done) {
			// Initialize each value.
			var Values = new Dictionary<string, string>();
			// Add the keyword.
			Values.Add("keyword", _Input);
			// Get the document.
			Http.Post(Provider.Domain + "/Search/Manga", Values, (Response) => {
				// Initialize a new instance of the HtmlDocument class.
				HtmlDocument HtmlDocument = new HtmlDocument();
				// Load the document.
				HtmlDocument.LoadHtml(Response.AsString());
				// Find each anchor element ...
				Results = HtmlDocument.DocumentNode.Descendants("a")
					// ... with a references indicating a series ...
					.Where(x => Regex.Match(HtmlEntity.DeEntitize(x.GetAttributeValue("href", string.Empty)).Trim(), "^(?!http).*/manga/([^/]+?)/?$", RegexOptions.IgnoreCase).Success)
					// ... select the results ...
					.Select(x => new Series(Provider.Domain + HtmlEntity.DeEntitize(x.Attributes["href"].Value).Trim(), HtmlEntity.DeEntitize(x.InnerText).Trim()) as ISeries)
					// ... and create a list.
					.ToList();
				// Invoke the callback.
				Done(this);
			});
		}
		#endregion

		#region IDisposable
		/// <summary>
		/// Dispose of the object.
		/// </summary>
		public void Dispose() {
			// Check if the results are valid.
			if (Results != null) {
				// Iterate through each result.
				foreach (ISeries Result in Results) {
					// Dispose of the object.
					Result.Dispose();
				}
				// Remove the results.
				Results = null;
			}
		}
		#endregion

		#region ISearch
		/// <summary>
		/// Contains the results.
		/// </summary>
		public IEnumerable<ISeries> Results { get; private set; }
		#endregion
	}
}