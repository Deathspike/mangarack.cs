// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TinyHttp;

namespace MangaRack.Provider.KissManga {
	/// <summary>
	/// Represents a KissManga chapter.
	/// </summary>
	sealed class Chapter : IChapter {
		#region Constructor
		/// <summary>
		/// Initialize a new instance of the Chapter class.
		/// </summary>
		/// <param name="Number">The number.</param>
		/// <param name="Title">The title.</param>
		/// <param name="UniqueIdentifier">The unique identifier.</param>
		/// <param name="Volume">The volume.</param>
		public Chapter(double Number, string Title, string UniqueIdentifier, double Volume) {
			// Set the number.
			this.Number = Number;
			// Set the title.
			this.Title = Title;
			// Set the unique identifier.
			this.UniqueIdentifier = UniqueIdentifier;
			// Set the volume.
			this.Volume = Volume;
		}
		#endregion

		#region IAsync
		/// <summary>
		/// Populate asynchronously.
		/// </summary>
		/// <param name="Done">The callback.</param>
		public void Populate(Action<IChapter> Done) {
			// Get the document.
			Http.Get(UniqueIdentifier, (Response) => {
				// Find the images ...
				Children = Regex.Matches(Response.AsString(), @"lstImages\.push\((.*)\)").Cast<Match>()
					// ... select each page.
					.Select(x => new Page(HtmlEntity.DeEntitize(x.Groups[1].Value).Trim('"')) as IPage)
					// ... and create an array.
					.ToArray();
				// Invoke the callback.
				Done(this);
			});
		}
		#endregion

		#region IChapter
		/// <summary>
		/// Contains each child.
		/// </summary>
		public IEnumerable<IPage> Children { get; private set; }

		/// <summary>
		/// Contains the number.
		/// </summary>
		public double Number { get; private set; }

		/// <summary>
		/// Contains the title.
		/// </summary>
		public string Title { get; private set; }

		/// <summary>
		/// Contains the unique identifier.
		/// </summary>
		public string UniqueIdentifier { get; private set; }

		/// <summary>
		/// Contains the volume.
		/// </summary>
		public double Volume { get; private set; }
		#endregion

		#region IDisposable
		/// <summary>
		/// Dispose of the object.
		/// </summary>
		public void Dispose() {
			// Check if the children are valid.
			if (Children != null) {
				// Iterate through each child.
				foreach (IPage Child in Children) {
					// Dispose of the object.
					Child.Dispose();
				}
				// Remove the children.
				Children = null;
			}
		}
		#endregion
	}
}