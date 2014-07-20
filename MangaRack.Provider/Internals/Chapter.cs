// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using MangaRack.Provider.Interfaces;

namespace MangaRack.Provider.Internals
{
    internal class Chapter : IChapter
    {
        private readonly IChapter _chapter;
        private double? _number;

        #region Constructor

        public Chapter(IChapter chapter)
        {
            Contract.Requires<ArgumentNullException>(chapter != null);
            _chapter = chapter;
        }

        #endregion

        #region Contract

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(_chapter != null);
        }

        #endregion

        #region Implementation of IAsync

        public void Populate(Action<IChapter> done)
        {
            _chapter.Populate(done);
        }

        #endregion

        #region Implementation of IChapter

        public IEnumerable<IPage> Children
        {
            get { return _chapter.Children; }
        }

        public string Location
        {
            get { return _chapter.Location; }
        }

        public double Number
        {
            get { return _number ?? _chapter.Number; }
            set { _number = value; }
        }

        public string Title
        {
            get { return _chapter.Title; }
        }

        public string UniqueIdentifier
        {
            get { return _chapter.UniqueIdentifier; }
        }

        public double Volume
        {
            get { return _chapter.Volume; }
        }

        #endregion

        #region Implementation of IDisposable

        public void Dispose()
        {
            _chapter.Dispose();
        }

        #endregion
    }
}