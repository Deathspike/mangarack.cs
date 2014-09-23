// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
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
			// Set the chapter.
			_chapter = chapter;
			// Set the publisher.
			_publisher = publisher;
			// Set the series.
			_series = series;
		}
		#endregion

		#region IAsync
		/// <summary>
		/// Populate asynchronously.
		/// </summary>
		public async Task PopulateAsync() {
			// Initialize a new instance of the BrokenPages class.
			var brokenPages = new List<string>();
			// Initialize the comic page information.
			var comicInfoPage = (ComicInfoPage)null;
			// Initialize the number.
			var number = 0;
			// Initialize a new instance of the List class.
			var metaPages = new List<ComicInfoPage>();
			// Check if the preview image is valid.
			if (_series.PreviewImage != null) {
				// Publish the preview image.
				metaPages.Add(_publisher.Publish(_series.PreviewImage, true, 0));
			}

		    foreach (var Page in _chapter.Children) {
		        await Page.PopulateAsync();
		        // Increment the number.
		        number++;
		        // Use the page and dispose of it when done.
		        using (Page) {
		            // Publish the page.
		            if ((comicInfoPage = _publisher.Publish(Page.Image, false, number)) != null) {
		                // Add the page.
		                metaPages.Add(comicInfoPage);
		                // Check if the page is a broken page.
		                if (string.Equals(comicInfoPage.Type, "Deleted")) {
		                    // Add the broken page.
		                    brokenPages.Add(string.Format("{0}: {1}", number.ToString("000"), Page.Location));
		                }
		            }
		        }
		    }

			// Check if a broken page is available.
			if (brokenPages.Count != 0) {
				// Publish broken page information.
				_publisher.Publish(brokenPages);
			}
			// Check if a valid page is available.
			if (metaPages.Count != 0) {
				// Initialize a new instance of the ComicInfo class.
				var comicInfo = new ComicInfo();
				// Transcribe the series, chapter and pages information.
				comicInfo.Transcribe(_series, _chapter, metaPages);
				// Publish comic information.
				_publisher.Publish(comicInfo);
			}
		}

		#endregion

		#region IDisposable
		/// <summary>
		/// Dispose of the object.
		/// </summary>
		public void Dispose() {
			// Stop the function.
			return;
		}
		#endregion
	}
}