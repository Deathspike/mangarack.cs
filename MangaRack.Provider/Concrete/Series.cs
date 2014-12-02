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
    /// Represents a provider.
    /// </summary>
    internal sealed class Series : ISeries {
        /// <summary>
        /// Represents a series.
        /// </summary>
        private readonly ISeries _series;

        /// <summary>
        /// Contains each child.
        /// </summary>
        private IEnumerable<IChapter> _children;

        #region Constructor
        /// <summary>
        /// Initialize a new instance of the Series class.
        /// </summary>
        /// <param name="series">The series.</param>
        public Series(ISeries series) {
            _series = series;
        }
        #endregion

        #region IAsync
        /// <summary>
        /// Populate asynchronously.
        /// </summary>
        /// <param name="done">The callback.</param>
        public void Populate(Action<ISeries> done) {
            _series.Populate(() => {
                _children = _series.Children.Select(x => new Chapter(x) as IChapter).ToArray();
                foreach (var source in _children.OfType<Chapter>()) {
                    if (source.Number != -1) continue;
                    var differential = new Dictionary<double, int>();
                    var origin = null as Chapter;
                    foreach (var candidate in _children.OfType<Chapter>()) {
                        if (candidate != source && candidate.Number != -1 && candidate.Volume == source.Volume) {
                            if (origin != null) {
                                var difference = Math.Round(candidate.Number - origin.Number, 4);
                                if (differential.ContainsKey(difference)) {
                                    differential[difference]++;
                                } else {
                                    differential[difference] = 1;
                                }
                            }
                            origin = candidate;
                        }
                    }
                    if (differential.Count == 0) {
                        source.Number = origin == null ? 0.5 : origin.Number + origin.Number / 2;
                    } else {
                        var shift = differential.OrderByDescending(x => x.Value).FirstOrDefault().Key;
                        source.Number = Math.Round(origin.Number + (shift <= 0 || shift >= 1 ? 1 : shift) / 2, 4);
                    }
                }
                done(this);
            });
        }
        #endregion

        #region IDisposable
        /// <summary>
        /// Dispose of the object.
        /// </summary>
        public void Dispose() {
            _series.Dispose();
        }
        #endregion

        #region ISeries
        /// <summary>
        /// Contains each artist.
        /// </summary>
        public IEnumerable<string> Artists {
            get { return _series.Artists; }
        }

        /// <summary>
        /// Contains each author.
        /// </summary>
        public IEnumerable<string> Authors {
            get { return _series.Authors; }
        }

        /// <summary>
        /// Contains each child.
        /// </summary>
        public IEnumerable<IChapter> Children {
            get { return _children; }
        }

        /// <summary>
        /// Contains each genre.
        /// </summary>
        public IEnumerable<string> Genres {
            get { return _series.Genres; }
        }

        /// <summary>
        /// Contains the location.
        /// </summary>
        public string Location {
            get { return _series.Location; }
        }

        /// <summary>
        /// Contains the preview image.
        /// </summary>
        public byte[] PreviewImage {
            get { return _series.PreviewImage; }
        }

        /// <summary>
        /// Contains the summary.
        /// </summary>
        public string Summary {
            get { return _series.Summary; }
        }

        /// <summary>
        /// Contains the title.
        /// </summary>
        public string Title {
            get { return _series.Title; }
        }
        #endregion
    }
}