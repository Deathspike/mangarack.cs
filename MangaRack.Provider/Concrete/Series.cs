// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System;
using System.Collections.Generic;
using System.Linq;

namespace MangaRack.Provider {
	/// <summary>
	/// Represents a provider.
	/// </summary>
	sealed class Series : ISeries {
		/// <summary>
		/// Contains each child.
		/// </summary>
		private IEnumerable<IChapter> _children;

		/// <summary>
		/// Represents a series.
		/// </summary>
		private ISeries _series;

		#region Constructor
		/// <summary>
		/// Initialize a new instance of the Series class.
		/// </summary>
		/// <param name="series">The series.</param>
		public Series(ISeries series) {
			// Set the series.
			_series = series;
		}
		#endregion

		#region IAsync
		/// <summary>
		/// Populate asynchronously.
		/// </summary>
		/// <param name="done">The callback.</param>
		public void Populate(Action<ISeries> done) {
			// Populate asynchronously.
			_series.Populate(() => {
				// Set each child.
				_children = _series.Children.Select(x => new Chapter(x) as IChapter).ToArray();
				// Iterate through each child.
				foreach (var source in _children.OfType<Chapter>()) {
					// Check if the source number is invalid.
					if (source.Number == -1) {
						// Initialize the differential.
						var differential = new Dictionary<double, int>();
						// Initialize the origin.
						var origin = null as Chapter;
						// Iterate through each candidate chapter.
						foreach (var candidate in _children.OfType<Chapter>()) {
							// Check if the candidate is valid and matches the source volume.
							if (candidate != source && candidate.Number != -1 && candidate.Volume == source.Volume) {
								// Check if the origin is valid.
								if (origin != null) {
									// Calculate the difference between the candidate and the origin.
									var difference = Math.Round(candidate.Number - origin.Number, 4);
									// Check if the difference has been recorded.
									if (differential.ContainsKey(difference)) {
										// Increment the number of occurrences for this difference.
										differential[difference]++;
									} else {
										// Set the number of occurrences for this difference.
										differential[difference] = 1;
									}
								}
								// Set the origin.
								origin = candidate;
							}
						}
						// Check if the differential is invalid.
						if (differential.Count == 0) {
							// Set the number.
							source.Number = origin == null ? 0.5 : origin.Number + origin.Number / 2;
						} else {
							// Initialize the most occurred difference as the number to use as shift.
							var shift = differential.OrderByDescending(x => x.Value).FirstOrDefault().Key;
							// Set the shifted number.
							source.Number = Math.Round(origin.Number + (shift <= 0 || shift >= 1 ? 1 : shift) / 2, 4);
						}
					}
				}
				// Invoke the callback.
				done(this);
			});
		}
		#endregion

		#region IDisposable
		/// <summary>
		/// Dispose of the object.
		/// </summary>
		public void Dispose() {
			// Dispose of the object.
			_series.Dispose();
		}
		#endregion

		#region ISeries
		/// <summary>
		/// Contains each artist.
		/// </summary>
		public IEnumerable<string> Artists {
			get {
				// Return each artist.
				return _series.Artists;
			}
		}

		/// <summary>
		/// Contains each author.
		/// </summary>
		public IEnumerable<string> Authors {
			get {
				// Return each author.
				return _series.Authors;
			}
		}

		/// <summary>
		/// Contains each child.
		/// </summary>
		public IEnumerable<IChapter> Children {
			get {
				// Return each child.
				return _children;
			}
		}

		/// <summary>
		/// Contains each genre.
		/// </summary>
		public IEnumerable<string> Genres {
			get {
				// Return each genre.
				return _series.Genres;
			}
		}

		/// <summary>
		/// Contains the preview image.
		/// </summary>
		public byte[] PreviewImage {
			get {
				// Return the preview image.
				return _series.PreviewImage;
			}
		}

		/// <summary>
		/// Contains the unique identifier.
		/// </summary>
		public string UniqueIdentifier {
			get {
				// Return the unique identifier.
				return _series.UniqueIdentifier;
			}
		}

		/// <summary>
		/// Contains the summary.
		/// </summary>
		public string Summary {
			get {
				// Return the summary.
				return _series.Summary;
			}
		}

		/// <summary>
		/// Contains the title.
		/// </summary>
		public string Title {
			get {
				// Return the title.
				return _series.Title;
			}
		}
		#endregion
	}
}