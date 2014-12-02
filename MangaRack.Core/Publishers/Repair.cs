// ======================================================================
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MangaRack.Core.Abstracts;
using MangaRack.Core.Serializers;
using MangaRack.Provider.Interfaces;

namespace MangaRack.Core.Publishers {
	/// <summary>
	/// Represents a repair publisher.
	/// </summary>
	public class Repair : IAsync {
		/// <summary>
		/// Contains each broken page.
		/// </summary>
		private IEnumerable<string> _brokenPages;

		/// <summary>
		/// Contains the chapter.
		/// </summary>
		private IChapter _chapter;

		/// <summary>
		/// Contains the comic information.
		/// </summary>
		private ComicInfo _comicInfo;

		/// <summary>
		/// Contains the publisher.
		/// </summary>
		private IPublisher _publisher;

		/// <summary>
		/// Contains the series.
		/// </summary>
		private ISeries _series;

		#region Constructor
		/// <summary>
		/// Initialize a new instance of the Repair class.
		/// </summary>
		/// <param name="publisher">The publisher.</param>
		/// <param name="series">The series.</param>
		/// <param name="chapter">The chapter.</param>
		/// <param name="comicInfo">The comic information.</param>
		/// <param name="brokenPages">Each broken page.</param>
		public Repair(IPublisher publisher, ISeries series, IChapter chapter, ComicInfo comicInfo, IEnumerable<string> brokenPages) {
			_brokenPages = brokenPages;
			_chapter = chapter;
			_comicInfo = comicInfo;
			_publisher = publisher;
			_series = series;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Indicates whether repairing has failed.
		/// </summary>
		public bool HasFailed { get; private set; }
		#endregion

		#region IAsync
		/// <summary>
		/// Populate asynchronously.
		/// </summary>
		public async Task PopulateAsync() {
			var hasComicInfo = false;
			var newBrokenPages = new List<string>();
			var newComicInfoPage = (ComicInfoPage)null;
			var oldComicInfoPage = null as ComicInfoPage;
			foreach (var oldBrokenPage in _brokenPages) {
				var separator = oldBrokenPage.IndexOf(':');
				if (separator != -1) {
					int number;
					if (int.TryParse(oldBrokenPage.Substring(0, separator), out number) && (_comicInfo == null || (oldComicInfoPage =_comicInfo.Pages.FirstOrDefault(x => x.Image == number)) != null)) {
                        var uniqueIdentifier = oldBrokenPage.Substring(separator + 1).Trim();
						foreach (var page in _chapter.Children) {
							if (string.Equals(page.Location, uniqueIdentifier)) {
								await page.PopulateAsync();
								if ((newComicInfoPage = _publisher.Publish(page.Image, false, number)) != null) {
									if (string.Equals(newComicInfoPage.Type, "Deleted")) {
										newBrokenPages.Add(string.Format("{0}: {1}", number.ToString("000"), page.Location));
									}
									if (_comicInfo != null) {
										hasComicInfo = true;
										_comicInfo.Pages.Remove(oldComicInfoPage);
										_comicInfo.Pages.Add(newComicInfoPage);
									}
									break;
								}
							}
						}
					}
				}
				HasFailed = true;
			}
			if (!HasFailed) {
				if (newBrokenPages.Count != 0) {
					_publisher.Publish(newBrokenPages);
				}
				if (hasComicInfo) {
					_publisher.Publish(_comicInfo);
				}
			}
		}
		#endregion

		#region IDisposable
		/// <summary>
		/// Dispose of the object.
		/// </summary>
		public void Dispose() {
			return;
		}
		#endregion
	}
}