// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using MangaRack.Provider;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MangaRack.Core {
	/// <summary>
	/// Represents a repair publisher.
	/// </summary>
	public sealed class Repair : IAsync<Repair> {
		/// <summary>
		/// Contains each broken page.
		/// </summary>
		private IEnumerable<string> _BrokenPages;

		/// <summary>
		/// Contains the chapter.
		/// </summary>
		private IChapter _Chapter;

		/// <summary>
		/// Contains the comic information.
		/// </summary>
		private ComicInfo _ComicInfo;

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
		/// Initialize a new instance of the Repair class.
		/// </summary>
		/// <param name="Publisher">The publisher.</param>
		/// <param name="Series">The series.</param>
		/// <param name="Chapter">The chapter.</param>
		/// <param name="ComicInfo">The comic information.</param>
		/// <param name="BrokenPages">Each broken page.</param>
		public Repair(IPublisher Publisher, ISeries Series, IChapter Chapter, ComicInfo ComicInfo, IEnumerable<string> BrokenPages) {
			// Set the broken pages.
			_BrokenPages = BrokenPages;
			// Set the chapter.
			_Chapter = Chapter;
			// Set the comic information.
			_ComicInfo = ComicInfo;
			// Set the publisher.
			_Publisher = Publisher;
			// Set the series.
			_Series = Series;
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
		/// <param name="Done">The callback.</param>
		public void Populate(Action<Repair> Done) {
			// Initialize the page enumerator.
			IEnumerator<IPage> Pages = _Chapter.Pages.GetEnumerator();
			// Advance the enumerator to the next element.
			if (!Pages.MoveNext()) {
				// Invoke the callback.
				Done(this);
			} else {
				// Initialize whether comic information is available.
				bool HasComicInfo = false;
				// Initialize a new instance of the BrokenPages class.
				List<string> NewBrokenPages = new List<string>();
				// Initialize the comic page information.
				ComicInfoPage NewComicInfoPage = null;
				// Initialize the broken page enumerator.
				IEnumerator<string> OldBrokenPages = _BrokenPages.GetEnumerator();
				// Initialize the comic page information.
				ComicInfoPage OldComicInfoPage = null;
				// Initialize the next handler.
				Action Next = null;
				// Run asynchronously.
				(Next = () => {
					// Advance the enumerator to the next element.
					while (OldBrokenPages.MoveNext()) {
						// Initialize the separator.
						int Separator = OldBrokenPages.Current.IndexOf(':');
						// Check if the separator is valid.
						if (Separator != -1) {
							// Initialize the number.
							int Number;
							// Check if the number is invalid.
							if (int.TryParse(OldBrokenPages.Current.Substring(0, Separator), out Number) && (_ComicInfo == null || (OldComicInfoPage =_ComicInfo.Pages.FirstOrDefault(x => x.Image == Number)) != null)) {
								// Initialize the target unique identifier.
								string UniqueIdentifier = OldBrokenPages.Current.Substring(Separator + 1).Trim();
								// Advance the enumerator to the next element.
								while (Pages.MoveNext()) {
									// Check if the unique identifier is valid.
									if (string.Equals(Pages.Current.UniqueIdentifier, UniqueIdentifier)) {
										// Populate asynchronously.
										Pages.Current.Populate((Page) => {
											// Publish the page.
											if ((NewComicInfoPage = _Publisher.Publish(Page.Image, false, Number)) != null) {
												// Check if the page is a broken page.
												if (string.Equals(NewComicInfoPage.Type, "Deleted")) {
													// Add the broken page.
													NewBrokenPages.Add(string.Format("{0}: {1}", Number.ToString("000"), Page.UniqueIdentifier));
												}
												// Check if comic information is available.
												if (_ComicInfo != null) {
													// Set whether comic information is available.
													HasComicInfo = true;
													// Remove the old comic page information.
													_ComicInfo.Pages.Remove(OldComicInfoPage);
													// Add the new comic page information.
													_ComicInfo.Pages.Add(NewComicInfoPage);
												}
												// Invoke the next handler.
												Next();
											}
										});
										// Stop the function.
										return;
									}
								}
							}
						}
						// Set repairing as failed.
						HasFailed = true;
					}
					// Check if repairing has not failed.
					if (!HasFailed) {
						// Check if a broken page is available.
						if (NewBrokenPages.Count != 0) {
							// Publish broken page information.
							_Publisher.Publish(NewBrokenPages);
						}
						// Check whether comic information is available.
						if (HasComicInfo) {
							// Publish comic information.
							_Publisher.Publish(_ComicInfo);
						}
					}
					// Invoke the callback.
					Done(this);
				})();
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