// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using MangaRack.Provider;
using System;
using System.Collections.Generic;

namespace MangaRack.Core {
	/// <summary>
	/// Represents a synchronizer publisher.
	/// </summary>
	public sealed class Synchronizer : IAsync<Synchronizer> {
		/// <summary>
		/// Contains the chapter.
		/// </summary>
		private IChapter _Chapter;

		/// <summary>
		/// Contains the publisher.
		/// </summary>
		private IPublisher _Publisher;

		/// <summary>
		/// Contains the series.
		/// </summary>
		private ISeries _Series;

		#region Constructor
		/// <summary>
		/// Initialize a new instance of the Synchronizer class.
		/// </summary>
		/// <param name="Publisher">The publisher.</param>
		/// <param name="Series">The series.</param>
		/// <param name="Chapter">The chapter.</param>
		public Synchronizer(IPublisher Publisher, ISeries Series, IChapter Chapter) {
			// Set the chapter.
			_Chapter = Chapter;
			// Set the publisher.
			_Publisher = Publisher;
			// Set the series.
			_Series = Series;
		}
		#endregion

		#region IAsync
		/// <summary>
		/// Populate asynchronously.
		/// </summary>
		/// <param name="Done">The callback.</param>
		public void Populate(Action<Synchronizer> Done) {
			// Initialize the page enumerator.
			IEnumerator<IPage> Pages = _Chapter.Pages.GetEnumerator();
			// Advance the enumerator to the next element.
			if (!Pages.MoveNext()) {
				// Invoke the callback.
				Done(this);
			} else {
				// Initialize a new instance of the BrokenPages class.
				List<string> BrokenPages = new List<string>();
				// Initialize the comic page information.
				ComicInfoPage ComicInfoPage;
				// Initialize the next handler.
				Action Next = null;
				// Initialize the number.
				int Number = 0;
				// Initialize a new instance of the List class.
				List<ComicInfoPage> MetaPages = new List<ComicInfoPage>();
				// Check if the preview image is valid.
				if (_Series.PreviewImage != null) {
					// Publish the preview image.
					MetaPages.Add(_Publisher.Publish(_Series.PreviewImage, true, 0));
				}
				// Populate asynchronously.
				(Next = () => Pages.Current.Populate((Page) => {
					// Increment the number.
					Number++;
					// Use the page and dispose of it when done.
					using (Page) {
						// Publish the page.
						if ((ComicInfoPage = _Publisher.Publish(Page.Image, false, Number)) != null) {
							// Add the page.
							MetaPages.Add(ComicInfoPage);
							// Check if the page is a broken page.
							if (string.Equals(ComicInfoPage.Type, "Deleted")) {
								// Add the broken page.
								BrokenPages.Add(string.Format("{0}: {1}", Number.ToString("000"), Page.UniqueIdentifier));
							}
						}
					}
					// Advance the enumerator to the next element.
					if (Pages.MoveNext()) {
						// Invoke the next handler.
						Next();
					} else {
						// Check if a broken page is available.
						if (BrokenPages.Count != 0) {
							// Publish broken page information.
							_Publisher.Publish(BrokenPages);
						}
						// Check if a valid page is available.
						if (MetaPages.Count != 0) {
							// Initialize a new instance of the ComicInfo class.
							ComicInfo ComicInfo = new ComicInfo();
							// Transcribe the series, chapter and pages information.
							ComicInfo.Transcribe(_Series, _Chapter, MetaPages);
							// Publish comic information.
							_Publisher.Publish(ComicInfo);
						}
						// Invoke the callback.
						Done(this);
					}
				}))();
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