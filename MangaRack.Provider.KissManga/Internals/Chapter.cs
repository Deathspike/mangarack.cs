// ======================================================================
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MangaRack.Provider.Interfaces;
using TinyHttp;

namespace MangaRack.Provider.KissManga.Internals {
	/// <summary>
	/// Represents a KissManga chapter.
	/// </summary>
	class Chapter : IChapter {
		#region Constructor
		/// <summary>
		/// Initialize a new instance of the Chapter class.
		/// </summary>
		/// <param name="number">The number.</param>
		/// <param name="location">The location.</param>
		/// <param name="title">The title.</param>
		/// <param name="uniqueIdentifier">The unique identifier.</param>
		/// <param name="volume">The volume.</param>
		public Chapter(double? number, string location, string title, string uniqueIdentifier, double? volume) {
			Number = number;
			Location = location;
			Title = title;
			UniqueIdentifier = uniqueIdentifier;
			Volume = volume;
		}
		#endregion

		#region IAsync
		/// <summary>
		/// Populate asynchronously.
		/// </summary>
		public async Task PopulateAsync() {
		    var response = await Http.GetAsync(Location);
			Children = Regex.Matches(response.AsString(), @"lstImages\.push\((.*)\)").Cast<Match>()
				.Select(x => new Page(HtmlEntity.DeEntitize(x.Groups[1].Value).Trim('"')) as IPage)
				.ToArray();
		}
		#endregion

		#region IChapter
		/// <summary>
		/// Contains each child.
		/// </summary>
		public IEnumerable<IPage> Children { get; private set; }

		/// <summary>
		/// Contains the location.
		/// </summary>
		public string Location { get; private set; }

		/// <summary>
		/// Contains the number.
		/// </summary>
		public double? Number { get; private set; }

		/// <summary>
		/// Contains the title.
		/// </summary>
		public string Title { get; private set; }

		/// <summary>
		/// Contains the unique identifier.
		/// </summary>
		public string UniqueIdentifier { get; private set; }

		/// <summary>
		/// Contains the volume.
		/// </summary>
		public double? Volume { get; private set; }
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
	}
}