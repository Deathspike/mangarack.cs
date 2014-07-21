// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using MangaRack.Provider.Interfaces;

namespace MangaRack.Provider.Internals
{
    // TODO: Clean up the abstract here.
    internal class Series : ISeries
    {
        private readonly ISeries _series;
        private IEnumerable<Chapter> _children;

        #region Abstract

        private static Dictionary<double, int> Compute(IEnumerable<IChapter> children,
            IChapter chapter,
            out IChapter previousChapter)
        {
            Contract.Requires<ArgumentNullException>(children != null);
            Contract.Requires<ArgumentNullException>(chapter != null);
            Contract.Ensures(Contract.Result<Dictionary<double, int>>() != null);
            var computedDifferences = new Dictionary<double, int>();
            previousChapter = null;

            foreach (var next in children
                .Where(x => x != chapter)
                .Where(x => x.Number >= 0)
                .Where(x => Math.Abs(x.Volume - chapter.Volume) <= 0))
            {
                if (previousChapter != null)
                {
                    var difference = Math.Round(next.Number - previousChapter.Number, 4);
                    if (!computedDifferences.ContainsKey(difference)) computedDifferences[difference] = 0;
                    computedDifferences[difference]++;
                }

                previousChapter = next;
            }

            return computedDifferences;
        }

        #endregion

        #region Constructor

        public Series(ISeries series)
        {
            Contract.Requires<ArgumentNullException>(series != null);
            _series = series;
        }

        #endregion

        #region Contract

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(_series != null);
        }

        #endregion

        #region Implementation of IAsync

        public async Task PopulateAsync()
        {
            await _series.PopulateAsync();

            _children = _series.Children.Select(x => new Chapter(x)).ToList();

            // TODO: Number should become nullable to indicate it's not set.
            foreach (var chapter in _children.Where(x => x.Number < 0))
            {
                IChapter previousChapter;
                var computedDifferences = Compute(_children, chapter, out previousChapter);

                if (computedDifferences.Count != 0)
                {
                    var bestDifference = computedDifferences.OrderByDescending(x => x.Value).FirstOrDefault().Key;
                    var clampedDifference = (bestDifference <= 0 || bestDifference >= 1 ? 1 : bestDifference);
                    chapter.Number = Math.Round(previousChapter.Number + clampedDifference/2, 4);
                    continue;
                }

                chapter.Number = previousChapter == null ? 0.5 : previousChapter.Number + 0.5;
            }
        }

        #endregion

        #region Implementation of IDisposable

        public void Dispose()
        {
            _series.Dispose();
        }

        #endregion

        #region Implementation of ISeries

        public IEnumerable<string> Artists
        {
            get { return _series.Artists; }
        }

        public IEnumerable<string> Authors
        {
            get { return _series.Authors; }
        }

        public IEnumerable<IChapter> Children
        {
            get { return _children ?? _series.Children; }
        }

        public IEnumerable<string> Genres
        {
            get { return _series.Genres; }
        }

        public string Location
        {
            get { return _series.Location; }
        }

        public byte[] PreviewImage
        {
            get { return _series.PreviewImage; }
        }

        public string Summary
        {
            get { return _series.Summary; }
        }

        public string Title
        {
            get { return _series.Title; }
        }

        #endregion
    }
}