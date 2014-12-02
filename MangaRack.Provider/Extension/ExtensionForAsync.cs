// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System;
using System.Threading;
using MangaRack.Provider.Abstract;

namespace MangaRack.Provider.Extension {
    /// <summary>
    /// Represents the class providing extensions for the IAsync interface.
    /// </summary>
    public static class ExtensionForAsync {
        #region Methods
        /// <summary>
        /// Populate synchronously.
        /// </summary>
        /// <param name="async">The asynchronous population.</param>
        public static T Populate<T>(this IAsync<T> async) {
            var manualResetEventSlim = new ManualResetEvent(false);
            var result = default(T);
            async.Populate(done => {
                result = done;
                manualResetEventSlim.Set();
            });
            manualResetEventSlim.WaitOne();
            return result;
        }

        /// <summary>
        /// Populate asynchronously.
        /// </summary>
        /// <param name="async">The asynchronous population.</param>
        /// <param name="done">The callback.</param>
        public static void Populate<T>(this IAsync<T> async, Action done) {
            async.Populate(result => done());
        }
        #endregion
    }
}