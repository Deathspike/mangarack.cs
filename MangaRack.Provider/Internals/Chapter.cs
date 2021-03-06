﻿// ======================================================================
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
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

        public Task PopulateAsync()
        {
            return _chapter.PopulateAsync();
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

        public double? Number
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

        public double? Volume
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