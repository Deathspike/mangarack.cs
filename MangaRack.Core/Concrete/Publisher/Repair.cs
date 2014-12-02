﻿// ======================================================================
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
			// Set the broken pages.
			_brokenPages = brokenPages;
			// Set the chapter.
			_chapter = chapter;
			// Set the comic information.
			_comicInfo = comicInfo;
			// Set the publisher.
			_publisher = publisher;
			// Set the series.
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
		/// <param name="done">The callback.</param>
		public void Populate(Action<Repair> done) {
			// Initialize the page enumerator.
			var pages = _chapter.Children.GetEnumerator();
			// Advance the enumerator to the next element.
			if (!pages.MoveNext()) {
				// Invoke the callback.
				done(this);
			} else {
				// Initialize whether comic information is available.
				var hasComicInfo = false;
				// Initialize the next handler.
				var next = null as Action;
				// Initialize a new instance of the BrokenPages class.
				var newBrokenPages = new List<string>();
				// Initialize the new comic page information.
				var newComicInfoPage = (ComicInfoPage)null;
				// Initialize the broken page enumerator.
				var oldBrokenPages = _brokenPages.GetEnumerator();
				// Initialize the old comic page information.
				var oldComicInfoPage = null as ComicInfoPage;
				// Run asynchronously.
				(next = () => {
					// Advance the enumerator to the next element.
					while (oldBrokenPages.MoveNext()) {
						// Initialize the separator.
						var separator = oldBrokenPages.Current.IndexOf(':');
						// Check if the separator is valid.
						if (separator != -1) {
							// Initialize the number.
							int number;
							// Check if the number is invalid.
							if (int.TryParse(oldBrokenPages.Current.Substring(0, separator), out number) && (_comicInfo == null || (oldComicInfoPage =_comicInfo.Pages.FirstOrDefault(x => x.Image == number)) != null)) {
								// Initialize the target unique identifier.
								var uniqueIdentifier = oldBrokenPages.Current.Substring(separator + 1).Trim();
								// Advance the enumerator to the next element.
								while (pages.MoveNext()) {
									// Check if the location is valid.
									if (string.Equals(pages.Current.Location, uniqueIdentifier)) {
										// Populate asynchronously.
										pages.Current.Populate(page => {
											// Publish the page.
											if ((newComicInfoPage = _publisher.Publish(page.Image, false, number)) != null) {
												// Check if the page is a broken page.
												if (string.Equals(newComicInfoPage.Type, "Deleted")) {
													// Add the broken page.
													newBrokenPages.Add(string.Format("{0}: {1}", number.ToString("000"), page.Location));
												}
												// Check if comic information is available.
												if (_comicInfo != null) {
													// Set whether comic information is available.
													hasComicInfo = true;
													// Remove the old comic page information.
													_comicInfo.Pages.Remove(oldComicInfoPage);
													// Add the new comic page information.
													_comicInfo.Pages.Add(newComicInfoPage);
												}
												// Invoke the next handler.
												next();
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
						if (newBrokenPages.Count != 0) {
							// Publish broken page information.
							_publisher.Publish(newBrokenPages);
						}
						// Check whether comic information is available.
						if (hasComicInfo) {
							// Publish comic information.
							_publisher.Publish(_comicInfo);
						}
					}
					// Invoke the callback.
					done(this);
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