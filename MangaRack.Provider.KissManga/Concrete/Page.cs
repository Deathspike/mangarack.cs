﻿// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System;
using MangaRack.Provider.Abstract;
using TinyHttp;

namespace MangaRack.Provider.KissManga.Concrete {
    /// <summary>
    /// Represents a KissManga page.
    /// </summary>
    internal sealed class Page : IPage {
        #region Constructor
        /// <summary>
        /// Initialize a new instance of the Page class.
        /// </summary>
        /// <param name="location">The location.</param>
        public Page(string location) {
            Location = location;
        }
        #endregion

        #region IAsync
        /// <summary>
        /// Populate asynchronously.
        /// </summary>
        /// <param name="done">The callback.</param>
        public void Populate(Action<IPage> done) {
            Http.Get(Location, imageResponse => {
                Image = imageResponse.AsBinary();
                done(this);
            });
        }
        #endregion

        #region IDisposable
        /// <summary>
        /// Dispose of the object.
        /// </summary>
        public void Dispose() {
            Image = null;
        }
        #endregion

        #region IPage
        /// <summary>
        /// Contains the image.
        /// </summary>
        public byte[] Image { get; private set; }

        /// <summary>
        /// Contains the location.
        /// </summary>
        public string Location { get; private set; }
        #endregion
    }
}