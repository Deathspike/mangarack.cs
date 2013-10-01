// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System.Collections.Generic;

namespace MangaRack.Core {
	/// <summary>
	/// Represents a publisher.
	/// </summary>
	public interface IPublisher {
		#region Methods
		/// <summary>
		/// Publish an image.
		/// </summary>
		/// <param name="Image">The image.</param>
		/// <param name="PreviewImage">Indicates whether this is a preview image.</param>
		/// <param name="Number">The number, when not a preview image.</param>
		ComicInfoPage Publish(byte[] Image, bool PreviewImage, int Number);

		/// <summary>
		/// Publish comic information.
		/// </summary>
		/// <param name="ComicInfo">The comic information.</param>
		void Publish(ComicInfo ComicInfo);

		/// <summary>
		/// Publish broken page information.
		/// </summary>
		/// <param name="BrokenPages">The broken pages.</param>
		void Publish(IEnumerable<string> BrokenPages);
		#endregion
	}
}