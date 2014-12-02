// ======================================================================
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MangaRack.Provider.Interfaces;
using TinyHttp;

namespace MangaRack.Provider.Batoto.Internals {
	/// <summary>
	/// Represents a Batoto page.
	/// </summary>
	class Page : IPage {
		#region Constructor
		/// <summary>
		/// Initialize a new instance of the Page class.
		/// </summary>
		/// <param name="location">The location.</param>
		public Page(string location) {
			Location = location;
		}
		#endregion

		#region IAsync
		/// <summary>
		/// Populate asynchronously.
		/// </summary>
		public async Task PopulateAsync() {
		    var HtmlResponse = await Http.GetAsync(Location + "?supress_webtoon=t");
			var htmlDocument = new HtmlDocument();
			var htmlNode = null as HtmlNode;
			htmlDocument.LoadHtml(HtmlResponse.AsString());
			if ((htmlNode = htmlDocument.DocumentNode.Descendants("img")
				.Where(x => HtmlEntity.DeEntitize(x.GetAttributeValue("alt", string.Empty)).Trim().EndsWith("Batoto!"))
				.FirstOrDefault()) != null) {
				var imageResponse = await Http.GetAsync(HtmlEntity.DeEntitize(htmlNode.GetAttributeValue("src", string.Empty)).Trim());
				Image = imageResponse.AsBinary();
			}
		}
		#endregion

		#region IDisposable
		/// <summary>
		/// Dispose of the object.
		/// </summary>
		public void Dispose() {
			Image = null;
		}
		#endregion

		#region IPage
		/// <summary>
		/// Contains the image.
		/// </summary>
		public byte[] Image { get; private set; }

		/// <summary>
		/// Contains the location.
		/// </summary>
		public string Location { get; private set; }
		#endregion
	}
}