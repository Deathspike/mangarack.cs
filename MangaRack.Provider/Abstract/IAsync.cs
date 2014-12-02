// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System;

namespace MangaRack.Provider.Abstract {
    /// <summary>
    /// Represents an asynchronous population.
    /// </summary>
    public interface IAsync<out T> : IDisposable {
        #region Methods
        /// <summary>
        /// Populate asynchronously.
        /// </summary>
        /// <param name="done">The callback.</param>
        void Populate(Action<T> done);
        #endregion
    }
}