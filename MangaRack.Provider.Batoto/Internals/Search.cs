// ======================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MangaRack.Provider.Interfaces;
using TinyHttp;

namespace MangaRack.Provider.Batoto.Internals {
	/// <summary>
	/// Represents a Batoto search.
	/// </summary>
	class Search : ISearch {
		#region Constructor
		/// <summary>
		/// Initialize a new instance of the Search class.
		/// </summary>
		/// <param name="input">The input.</param>
		public Search(string input) {
			Input = input;
		}
		#endregion

		#region IAsync
		/// <summary>
		/// Populate asynchronously.
		/// </summary>
		public async Task PopulateAsync() {
		    var Response = await Http.GetAsync(Provider.Domain + "search?name=" + Uri.EscapeDataString(Input));
			var htmlDocument = new HtmlDocument();
			htmlDocument.LoadHtml(Response.AsString());
			Children = htmlDocument.DocumentNode.Descendants("a")
				.Where(x => HtmlEntity.DeEntitize(x.GetAttributeValue("href", string.Empty)).Trim().Contains("/comic/"))
				.Select(x => new Series(HtmlEntity.DeEntitize(x.Attributes["href"].Value).Trim(), HtmlEntity.DeEntitize(x.InnerText).Trim()) as ISeries)
				.ToArray();
		}
		#endregion

		#region IDisposable
		/// <summary>
		/// Dispose of the object.
		/// </summary>
		public void Dispose() {
			if (Children != null) {
				foreach (var child in Children) {
					child.Dispose();
				}
				Children = null;
			}
		}
		#endregion

		#region ISearch

		public IEnumerable<ISeries> Children { get; private set; }
		public string Input { get; private set; }

		#endregion
	}
}