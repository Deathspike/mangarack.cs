// ======================================================================
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using MangaRack.Provider.Interfaces;

namespace MangaRack.Provider.Internals
{
    internal class Search : ISearch
    {
        private readonly ISearch _search;
        private IEnumerable<ISeries> _children;

        #region Constructor

        public Search(ISearch search)
        {
            Contract.Requires<ArgumentException>(search != null);
            _search = search;
        }

        #endregion

        #region Contract

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(_search != null);
        }

        #endregion

        #region Implementation of IAsync

        public async Task PopulateAsync()
        {
            await _search.PopulateAsync();

            _children = new List<ISeries>(_search.Children.Select(x => new Series(x) as ISeries));
        }

        #endregion

        #region Implementation of ISearch

        public IEnumerable<ISeries> Children
        {
            get { return _children != null ? _children.AsEnumerable() : _search.Children; }
        }

        public string Input
        {
            get { return _search.Input; }
        }

        #endregion

        #region Implementation of IDisposable

        public void Dispose()
        {
            _search.Dispose();
        }

        #endregion
    }
}