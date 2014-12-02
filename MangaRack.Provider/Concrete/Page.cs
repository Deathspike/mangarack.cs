// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System;
using MangaRack.Provider.Abstract;
using MangaRack.Provider.Extension;

namespace MangaRack.Provider.Concrete {
    /// <summary>
    /// Represents a page.
    /// </summary>
    internal sealed class Page : IPage {
        /// <summary>
        /// Contains the page.
        /// </summary>
        private readonly IPage _page;

        #region Constructor
        /// <summary>
        /// Initialize a new instance of the Page class.
        /// </summary>
        /// <param name="page">The page.</param>
        public Page(IPage page) {
            _page = page;
        }
        #endregion

        #region IAsync
        /// <summary>
        /// Populate asynchronously.
        /// </summary>
        /// <param name="done">The callback.</param>
        public void Populate(Action<IPage> done) {
            _page.Populate(() => done(this));
        }
        #endregion

        #region IDisposable
        /// <summary>
        /// Dispose of the object.
        /// </summary>
        public void Dispose() {
            _page.Dispose();
        }
        #endregion

        #region IPage
        /// <summary>
        /// Contains the image.
        /// </summary>
        public byte[] Image {
            get { return _page.Image; }
        }

        /// <summary>
        /// Contains the location.
        /// </summary>
        public string Location {
            get { return _page.Location; }
        }
        #endregion
    }
}