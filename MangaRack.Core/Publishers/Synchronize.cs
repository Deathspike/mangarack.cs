// ======================================================================
using System.Collections.Generic;
using System.Threading.Tasks;
using MangaRack.Core.Abstracts;
using MangaRack.Core.Serializers;
using MangaRack.Provider.Interfaces;

namespace MangaRack.Core.Publishers {
	/// <summary>
	/// Represents a synchronize publisher.
	/// </summary>
	public class Synchronize : IAsync {
		/// <summary>
		/// Contains the chapter.
		/// </summary>
		private IChapter _chapter;

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
		/// Initialize a new instance of the Synchronize class.
		/// </summary>
		/// <param name="publisher">The publisher.</param>
		/// <param name="series">The series.</param>
		/// <param name="chapter">The chapter.</param>
		public Synchronize(IPublisher publisher, ISeries series, IChapter chapter) {
			_chapter = chapter;
			_publisher = publisher;
			_series = series;
		}
		#endregion

		#region IAsync
		/// <summary>
		/// Populate asynchronously.
		/// </summary>
		public async Task PopulateAsync() {
			var brokenPages = new List<string>();
			var comicInfoPage = (ComicInfoPage)null;
			var number = 0;
			var metaPages = new List<ComicInfoPage>();
			if (_series.PreviewImage != null) {
				metaPages.Add(_publisher.Publish(_series.PreviewImage, true, 0));
			}

		    foreach (var Page in _chapter.Children) {
		        await Page.PopulateAsync();
		        number++;
		        using (Page) {
		            if ((comicInfoPage = _publisher.Publish(Page.Image, false, number)) != null) {
		                metaPages.Add(comicInfoPage);
		                if (string.Equals(comicInfoPage.Type, "Deleted")) {
		                    brokenPages.Add(string.Format("{0}: {1}", number.ToString("000"), Page.Location));
		                }
		            }
		        }
		    }
			if (brokenPages.Count != 0) {
				_publisher.Publish(brokenPages);
			}
			if (metaPages.Count != 0) {
				var comicInfo = new ComicInfo();
				comicInfo.Transcribe(_series, _chapter, metaPages);
				_publisher.Publish(comicInfo);
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