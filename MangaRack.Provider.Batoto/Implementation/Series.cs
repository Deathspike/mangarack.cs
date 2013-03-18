// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;

namespace MangaRack.Provider.Batoto {
	/// <summary>
	/// Represents a Batoto series.
	/// </summary>
	public sealed class Series : KeyValueStore, ISeries {
		/// <summary>
		/// Contains the document.
		/// </summary>
		private readonly HtmlDocument _HtmlDocument;

		/// <summary>
		/// Indicates whether the preview image has been tried.
		/// </summary>
		private bool _HasTriedPreviewImage;

		/// <summary>
		/// Contains the preview image.
		/// </summary>
		private Bitmap _PreviewImage;

		/// <summary>
		/// Contains the web client.
		/// </summary>
		private readonly StateWebClient _WebClient;

		/// <summary>
		/// Contains the unique identifier.
		/// </summary>
		private readonly string _UniqueIdentifier;

		#region Constructor
		/// <summary>
		/// Initialize a new instance of the Series class.
		/// </summary>
		/// <param name="UniqueIdentifier">The unique identifier.</param>
		public Series(string UniqueIdentifier) {
			// Initialize the properties.
			if (true) {
				// Initialize a new instance of the HtmlDocument class.
				_HtmlDocument = new HtmlDocument();
				// Set the unique identifier.
				_UniqueIdentifier = UniqueIdentifier;
				// Initialize a new instance of the StateWebClient class.
				_WebClient = new StateWebClient();
			}
			// Initialize the document.
			if (true) {
				// Download the document and load it.
				_HtmlDocument.LoadHtml(_WebClient.DownloadString(UniqueIdentifier));
			}
		}
		#endregion

		#region IDisposable
		/// <summary>
		/// Dispose of the object.
		/// </summary>
		public void Dispose() {
			// Check if the preview image is available.
			if (_PreviewImage != null) {
				// Dispose of the preview image.
				_PreviewImage.Dispose();
			}
			// Dispose of the web client.
			_WebClient.Dispose();
		}
		#endregion

		#region ISeries
		/// <summary>
		/// Contains each artist.
		/// </summary>
		public IEnumerable<string> Artists {
			get {
				// Check if the key-value store does not contain the key.
				if (!_Contains(() => Artists)) {
					// Find each table definition element ...
					_Set(() => Artists, _HtmlDocument.DocumentNode.Descendants("td")
						// ... with a reference containing with the appropriate name ...
						.Where(x => x.ParentNode != null && HtmlEntity.DeEntitize(x.InnerText).Equals("Artist:"))
						// .. selecting each anchor element ...
						.SelectMany(x => x.ParentNode.LastElement().Descendants("a")
							// ... and select the text without HTML entities.
							.Select(y => HtmlEntity.DeEntitize(y.InnerText))));
				}
				// Get a value.
				return _Get(() => Artists) as IEnumerable<string>;
			}
		}

		/// <summary>
		/// Contains each author.
		/// </summary>
		public IEnumerable<string> Authors {
			get {
				// Check if the key-value store does not contain the key.
				if (!_Contains(() => Authors)) {
					// Find each table definition element ...
					_Set(() => Authors, _HtmlDocument.DocumentNode.Descendants("td")
						// ... with a reference containing with the appropriate name ...
						.Where(x => x.ParentNode != null && HtmlEntity.DeEntitize(x.InnerText).Equals("Author:"))
						// .. selecting each anchor element ...
						.SelectMany(x => x.ParentNode.LastElement().Descendants("a")
							// ... and select the text without HTML entities.
							.Select(y => HtmlEntity.DeEntitize(y.InnerText))));
				}
				// Get a value.
				return _Get(() => Authors) as IEnumerable<string>;
			}
		}

		/// <summary>
		/// Contains each chapter.
		/// </summary>
		public IEnumerable<IChapter> Chapters {
			get {
				// Check if the key-value store does not contain the key.
				if (!_Contains(() => Chapters)) {
					// Initialize the processed volume.
					double ProcessedVolume;
					// Find each anchor element ...
					Listing[] Listings = _HtmlDocument.DocumentNode.Descendants("a")
						// ... with the English language ...
						.Where(x => x.ParentNode != null && x.ParentNode.ParentNode != null && x.ParentNode.ParentNode.GetAttributeValue("class", string.Empty).Split(' ').Contains("lang_English"))
						// ... with a references indicating a chapter ...
						.Where(x => x.GetAttributeValue("href", string.Empty).Contains("/read/"))
						// ... selecting each valid volume ...
						.Select(x => new { Chapter = x, Match = Regex.Match(HtmlEntity.DeEntitize(x.InnerText).Trim(), @"(\s?Vol\.\s?(?<Volume>[0-9\.]+))?\s?(Ch\.)?\s?(?<Number>([0-9\.]+|Extra|Omake))(\s?-\s?[0-9\.]+)?(\s?v\.?[0-9]+)?(\s?\(?Part\s(?<Part>[0-9]+)\)?)?(\s?(-|\+)\s?)?\s?(Read Onl?ine|:?\s?(?<Title>.+?)?(Read Online)?)$", RegexOptions.IgnoreCase) })
						// ... where the previous match was successful ...
						.Where(x => x.Match.Success)
						// ... selecting a proper type with all relevant information ...
						.Select(x => new Listing(x.Match.Groups["Number"].Value) { Chapter = x.Chapter, Match = x.Match, Volume = double.TryParse(x.Match.Groups["Volume"].Value, out ProcessedVolume) ? ProcessedVolume : -1, Title = x.Match.Groups["Title"].Value })
						// ... in the reverse order ...
						.Reverse()
						// ... and convert the result to an array.
						.ToArray();
					// Iterate through each listing.
					foreach (Listing Listing in Listings) {
						// Check if a part notation has been matched.
						if (!string.IsNullOrEmpty(Listing.Match.Groups["Part"].Value)) {
							// Increment the number according to the part number.
							Listing.Number += double.Parse(Listing.Match.Groups["Part"].Value) / 10;
						}
						// Check if the number is invalid.
						if (Listing.Number == -1) {
							// Initialize the number of occurences per change.
							Dictionary<double, int> Changes = new Dictionary<double, int>();
							// Retrieve the parent.
							Listing Parent = null;
							// Iterate through each listing.
							foreach (Listing Target in Listings) {
								// Check if this is a candidate.
								if (Target != Listing && Target.Volume == Listing.Volume) {
									// Check if the parent has been set.
									if (Parent != null) {
										// Calculate the change.
										double Change = Math.Round(Target.Number - Parent.Number, 4);
										// Check if the change has been added once.
										if (Changes.ContainsKey(Change)) {
											// Increment the number of occurrences for this change.
											Changes[Change]++;
										} else {
											// Set the number of occurrences for this change.
											Changes[Change] = 1;
										}
									}
									// Set the parent.
									Parent = Target;
								}
							}
							// Process the number.
							if (true) {
								// Retrieve the shift number.
								double Shift = Changes.OrderByDescending(x => x.Value).First().Key;
								// Set the corrected number.
								Listing.Number = Parent == null || Changes.Count == 0 ? 0.5 : Math.Round(Parent.Number + (Shift <= 0 || Shift >= 1 ? 1 : Shift) / 2, 4);
							}
						}
					}
					// Set the value.
					_Set(() => Chapters, Listings.Select(x => new Chapter(_WebClient.Referer, x.Chapter, x.Volume, x.Number, x.Title) as IChapter));
				}
				// Get a value.
				return _Get(() => Chapters) as IEnumerable<IChapter>;
			}
		}

		/// <summary>
		/// Contains each genre.
		/// </summary>
		public IEnumerable<string> Genres {
			get {
				// Check if the key-value store does not contain the key.
				if (!_Contains(() => Genres)) {
					// Find each table definition element ...
					_Set(() => Genres, _HtmlDocument.DocumentNode.Descendants("td")
						// ... with a reference containing with the appropriate name ...
						.Where(x => x.ParentNode != null && HtmlEntity.DeEntitize(x.InnerText).Equals("Genres:"))
						// .. selecting each anchor element ...
						.SelectMany(x => x.ParentNode.LastElement().Descendants("span")
							// ... and select the text without HTML entities.
							.Select(y => HtmlEntity.DeEntitize(y.InnerText))));
				}
				// Get a value.
				return _Get(() => Genres) as IEnumerable<string>;
			}
		}

		/// <summary>
		/// Contains the preview image.
		/// </summary>
		public Bitmap PreviewImage {
			get {
				// Check if the preview image is invalid.
				if (!_HasTriedPreviewImage && _PreviewImage == null) {
					// Attempt the following code.
					try {
						// Find each image element ...
						_PreviewImage = _WebClient.DownloadData(_HtmlDocument.DocumentNode.Descendants("img")
							// ... with a reference containing a preview image ...
							.Where(x => HtmlEntity.DeEntitize(x.GetAttributeValue("src", string.Empty)).Contains("/uploads/"))
							// ... select the reference attribute ...
							.Select(x => HtmlEntity.DeEntitize(x.Attributes["src"].Value))
							// ... download the first preview image ...
							.First())
							// ... and create a bitmap from the bytes.
							.Bitmap();
					} catch {
						// Set the status indicating the preview image has been tried.
						_HasTriedPreviewImage = true;
						// Clear the preview image.
						_PreviewImage = null;
					}
				}
				// Return the preview image.
				return _PreviewImage;
			}
		}

		/// <summary>
		/// Contains the summary.
		/// </summary>
		public string Summary {
			get {
				// Check if the key-value store does not contain the key.
				if (!_Contains(() => Summary)) {
					// Find each table definition element ...
					_Set(() => Summary, string.Join("\n", _HtmlDocument.DocumentNode.Descendants("td")
						// ... with a reference containing with the appropriate name ...
						.Where(x => x.ParentNode != null && HtmlEntity.DeEntitize(x.InnerText).Equals("Description:"))
						// .. selecting each anchor element ...
						.SelectMany(x => x.ParentNode.LastElement().Descendants()
							// ... and select the text without HTML entities ...
							.Select(y => HtmlEntity.DeEntitize(y.InnerText)))
						// ... and convert the result to an array ...
						.ToArray())
						// ... and remove multiple new lines.
						.Replace("\n\n", "\n"));
				}
				// Get a value.
				return _Get(() => Summary) as string;
			}
		}

		/// <summary>
		/// Contains the title.
		/// </summary>
		public string Title {
			get {
				// Check if the key-value store does not contain the key.
				if (!_Contains(() => Title)) {
					// Find each title element ...
					_Set(() => Title, _HtmlDocument.DocumentNode.Descendants("h1")
						// ... where the class is the page title ...
						.Where(x=>HtmlEntity.DeEntitize(x.GetAttributeValue("class", string.Empty)).Split(' ').Contains("ipsType_pagetitle"))
						// ... select the text without HTML entities ...
						.Select(x => HtmlEntity.DeEntitize(x.InnerText).Trim())
						// ... using the first match.
						.FirstOrDefault());
				}
				// Get a value.
				return _Get(() => Title) as string;
			}
		}

		/// <summary>
		/// Contains the unique identifier.
		/// </summary>
		public string UniqueIdentifier {
			get {
				// Return the unique identifier.
				return _UniqueIdentifier;
			}
		}
		#endregion
	}
}