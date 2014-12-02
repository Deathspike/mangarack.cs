// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using MangaRack.Provider.Abstract;
using MangaRack.Provider.Extension;

namespace MangaRack.Provider.Concrete {
    /// <summary>
    /// Represents a search.
    /// </summary>
    internal sealed class Search : ISearch {
        /// <summary>
        /// Contains the search.
        /// </summary>
        private readonly ISearch _search;

        #region Constructor
        /// <summary>
        /// Initialize a new instance of the Search class.
        /// </summary>
        /// <param name="search">The search.</param>
        public Search(ISearch search) {
            _search = search;
        }
        #endregion

        #region IAsync
        /// <summary>
        /// Populate asynchronously.
        /// </summary>
        /// <param name="done">The callback.</param>
        public void Populate(Action<ISearch> done) {
            _search.Populate(() => {
                Children = _search.Children.Select(x => new Series(x) as ISeries).ToArray();
                done(this);
            });
        }
        #endregion

        #region IDisposable
        /// <summary>
        /// Dispose of the object.
        /// </summary>
        public void Dispose() {
            _search.Dispose();
        }
        #endregion

        #region ISearch
        /// <summary>
        /// Contains each child.
        /// </summary>
        public IEnumerable<ISeries> Children { get; private set; }

        /// <summary>
        /// Contains the input.
        /// </summary>
        public string Input {
            get { return _search.Input; }
        }
        #endregion
    }
}