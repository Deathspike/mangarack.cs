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
	class Series : ISeries {
		/// <summary>
		/// Contains each chapter.
		/// </summary>
		private IEnumerable<IChapter> _Chapters;

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
				// Set each chapter.
				_Chapters = _Series.Chapters.Select(x => new Chapter(x) as IChapter).ToList();
				// Iterate through each chapter.
				foreach (Chapter Source in _Chapters) {
					// Check if the source number is invalid.
					if (Source.Number == -1) {
						// Initialize the differential.
						Dictionary<double, int> Differential = new Dictionary<double, int>();
						// Initialize the origin.
						Chapter Origin = null;
						// Iterate through each candidate chapter.
						foreach (Chapter Candidate in Chapters) {
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
				// Get each artist.
				return _Series.Artists;
			}
		}

		/// <summary>
		/// Contains each author.
		/// </summary>
		public IEnumerable<string> Authors {
			get {
				// Get each author.
				return _Series.Authors;
			}
		}

		/// <summary>
		/// Contains each chapter.
		/// </summary>
		public IEnumerable<IChapter> Chapters {
			get {
				// Get each chapter.
				return _Chapters;
			}
		}

		/// <summary>
		/// Contains each genre.
		/// </summary>
		public IEnumerable<string> Genres {
			get {
				// Get each genre.
				return _Series.Genres;
			}
		}

		/// <summary>
		/// Contains the preview image.
		/// </summary>
		public byte[] PreviewImage {
			get {
				// Get the preview image.
				return _Series.PreviewImage;
			}
		}

		/// <summary>
		/// Contains the unique identifier.
		/// </summary>
		public string UniqueIdentifier {
			get {
				// Get the unique identifier.
				return _Series.UniqueIdentifier;
			}
		}

		/// <summary>
		/// Contains the summary.
		/// </summary>
		public string Summary {
			get {
				// Get the summary.
				return _Series.Summary;
			}
		}

		/// <summary>
		/// Contains the title.
		/// </summary>
		public string Title {
			get {
				// Get the title.
				return _Series.Title;
			}
		}
		#endregion
	}
}