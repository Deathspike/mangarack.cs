// ======================================================================
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using MangaRack.Provider.Interfaces;

namespace MangaRack.Provider.Internals
{
    internal class Series : ISeries
    {
        private readonly ISeries _series;
        private IEnumerable<Chapter> _children;

        #region Abstract

        private static IEnumerable<Chapter> Approximate(ICollection<Chapter> chapters)
        {
            Contract.Requires<ArgumentNullException>(chapters != null);
            Contract.Ensures(Contract.Result<IEnumerable<Chapter>>() != null);

            foreach (var chapter in chapters)
            {
                if (chapter == null || chapter.Number != null) continue;
                Approximate(chapters, chapter);
            }

            return chapters;
        }

        private static void Approximate(IEnumerable<Chapter> chapters, Chapter chapter)
        {
            Contract.Requires<ArgumentNullException>(chapters != null);
            Contract.Requires<ArgumentNullException>(chapter != null);
            var differences = new Dictionary<double, int>();
            var previous = null as Chapter;
            foreach (var next in chapters)
            {
                if (next == null
                    || next == chapter
                    || next.Number == null
                    || next.Volume != chapter.Volume) continue;
                if (previous != null && previous.Number != null)
                {
                    var difference = Math.Round((double) next.Number - (double) previous.Number, 4);
                    differences[difference] = differences.ContainsKey(difference) ? differences[difference] + 1 : 1;
                }
                previous = next;
            }
            if (differences.Count != 0 && previous != null)
            {
                var value = differences.OrderByDescending(x => x.Value).Select(x => x.Value).FirstOrDefault();
                var clampedValue = Math.Min(Math.Max(value, 0), 1);
                chapter.Number = previous.Number + clampedValue/2;
            }
            else
            {
                chapter.Number = previous != null ? previous.Number + 0.5 : 0.5;
            }
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

            if (_series.Children == null) return;

            _children = Approximate(_series.Children.Select(x => new Chapter(x)).ToList());
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