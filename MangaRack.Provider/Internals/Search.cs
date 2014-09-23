// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using MangaRack.Provider.Interfaces;

namespace MangaRack.Provider.Internals
{
    internal class Search : ISearch
    {
        private readonly ISearch _search;
        private readonly IEnumerable<ISeries> _series;

        #region Constructor

        public Search(ISearch search)
        {
            Contract.Requires<ArgumentNullException>(search != null);
            _search = search;
            _series = new List<ISeries>(search.Children.Select(x => new Series(x) as ISeries));
        }

        #endregion

        #region Contract

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(_search != null);
            Contract.Invariant(_series != null);
        }

        #endregion

        #region Implementation of ISearch

        public IEnumerable<ISeries> Children
        {
            get { return _series; }
        }

        public string Input
        {
            get { return _search.Input; }
        }

        #endregion
    }
}