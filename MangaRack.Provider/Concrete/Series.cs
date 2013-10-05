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
		private IEnumerable<IChapter> _Children;

		/// <summary>
		/// Represents a series.
		/// </summary>
		private ISeries _Series;

		#region Constructor
		/// <summary>
		/// Initialize a new instance of the Series class.
		/// </summary>
		/// <param name="Series">The series.</param>
		public Series(ISeries Series) {
			// Set the series.
			_Series = Series;
		}
		#endregion

		#region IAsync
		/// <summary>
		/// Populate asynchronously.
		/// </summary>
		/// <param name="Done">The callback.</param>
		public void Populate(Action<ISeries> Done) {
			// Populate asynchronously.
			_Series.Populate(() => {
				// Set each child.
				_Children = _Series.Children.Select(x => new Chapter(x) as IChapter).ToArray();
				// Iterate through each child.
				foreach (Chapter Source in _Children) {
					// Check if the source number is invalid.
					if (Source.Number == -1) {
						// Initialize the differential.
						Dictionary<double, int> Differential = new Dictionary<double, int>();
						// Initialize the origin.
						Chapter Origin = null;
						// Iterate through each candidate chapter.
						foreach (Chapter Candidate in _Children) {
							// Check if the candidate is valid and matches the source volume.
							if (Candidate != Source && Candidate.Number != -1 && Candidate.Volume == Source.Volume) {
								// Check if the origin is valid.
								if (Origin != null) {
									// Calculate the difference between the candidate and the origin.
									double Difference = Math.Round(Candidate.Number - Origin.Number, 4);
									// Check if the difference has been recorded.
									if (Differential.ContainsKey(Difference)) {
										// Increment the number of occurrences for this difference.
										Differential[Difference]++;
									} else {
										// Set the number of occurrences for this difference.
										Differential[Difference] = 1;
									}
								}
								// Set the origin.
								Origin = Candidate;
							}
						}
						// Check if the differential is invalid.
						if (Differential.Count == 0) {
							// Set the number.
							Source.Number = Origin == null ? 0.5 : Origin.Number + Origin.Number / 2;
						} else {
							// Initialize the most occurred difference as the number to use as shift.
							double Shift = Differential.OrderByDescending(x => x.Value).FirstOrDefault().Key;
							// Set the shifted number.
							Source.Number = Math.Round(Origin.Number + (Shift <= 0 || Shift >= 1 ? 1 : Shift) / 2, 4);
						}
					}
				}
				// Invoke the callback.
				Done(this);
			});
		}
		#endregion

		#region IDisposable
		/// <summary>
		/// Dispose of the object.
		/// </summary>
		public void Dispose() {
			// Dispose of the object.
			_Series.Dispose();
		}
		#endregion

		#region ISeries
		/// <summary>
		/// Contains each artist.
		/// </summary>
		public IEnumerable<string> Artists {
			get {
				// Return each artist.
				return _Series.Artists;
			}
		}

		/// <summary>
		/// Contains each author.
		/// </summary>
		public IEnumerable<string> Authors {
			get {
				// Return each author.
				return _Series.Authors;
			}
		}

		/// <summary>
		/// Contains each child.
		/// </summary>
		public IEnumerable<IChapter> Children {
			get {
				// Return each child.
				return _Children;
			}
		}

		/// <summary>
		/// Contains each genre.
		/// </summary>
		public IEnumerable<string> Genres {
			get {
				// Return each genre.
				return _Series.Genres;
			}
		}

		/// <summary>
		/// Contains the preview image.
		/// </summary>
		public byte[] PreviewImage {
			get {
				// Return the preview image.
				return _Series.PreviewImage;
			}
		}

		/// <summary>
		/// Contains the unique identifier.
		/// </summary>
		public string UniqueIdentifier {
			get {
				// Return the unique identifier.
				return _Series.UniqueIdentifier;
			}
		}

		/// <summary>
		/// Contains the summary.
		/// </summary>
		public string Summary {
			get {
				// Return the summary.
				return _Series.Summary;
			}
		}

		/// <summary>
		/// Contains the title.
		/// </summary>
		public string Title {
			get {
				// Return the title.
				return _Series.Title;
			}
		}
		#endregion
	}
}