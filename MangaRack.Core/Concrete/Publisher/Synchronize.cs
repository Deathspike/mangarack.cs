// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System;
using System.Collections.Generic;
using MangaRack.Core.Abstract;
using MangaRack.Core.Concrete.Xml;
using MangaRack.Provider;
using MangaRack.Provider.Abstract;

namespace MangaRack.Core.Concrete.Publisher {
    /// <summary>
    /// Represents a synchronize publisher.
    /// </summary>
    public sealed class Synchronize : IAsync<Synchronize> {
        /// <summary>
        /// Contains the chapter.
        /// </summary>
        private readonly IChapter _chapter;

        /// <summary>
        /// Contains the publisher.
        /// </summary>
        private readonly IPublisher _publisher;

        /// <summary>
        /// Contains the series.
        /// </summary>
        private readonly ISeries _series;

        #region Constructor
        /// <summary>
        /// Initialize a new instance of the Synchronize class.
        /// </summary>
        /// <param name="publisher">The publisher.</param>
        /// <param name="series">The series.</param>
        /// <param name="chapter">The chapter.</param>
        public Synchronize(IPublisher publisher, ISeries series, IChapter chapter) {
            _chapter = chapter;
            _publisher = publisher;
            _series = series;
        }
        #endregion

        #region IAsync
        /// <summary>
        /// Populate asynchronously.
        /// </summary>
        /// <param name="done">The callback.</param>
        public void Populate(Action<Synchronize> done) {
            var pages = _chapter.Children.GetEnumerator();
            if (!pages.MoveNext()) {
                done(this);
            } else {
                var brokenPages = new List<string>();
                var next = (Action) null;
                var number = 0;
                var metaPages = new List<ComicInfoPage>();
                if (_series.PreviewImage != null) {
                    metaPages.Add(_publisher.Publish(_series.PreviewImage, true, 0));
                }
                (next = () => pages.Current.Populate(page => {
                    number++;
                    using (page) {
                        var comicInfoPage = (ComicInfoPage) null;
                        if ((comicInfoPage = _publisher.Publish(page.Image, false, number)) != null) {
                            metaPages.Add(comicInfoPage);
                            if (string.Equals(comicInfoPage.Type, "Deleted")) {
                                brokenPages.Add(string.Format("{0}: {1}", number.ToString("000"), page.Location));
                            }
                        }
                    }
                    if (pages.MoveNext()) {
                        next();
                    } else {
                        if (brokenPages.Count != 0) {
                            _publisher.Publish(brokenPages);
                        }
                        if (metaPages.Count != 0) {
                            var comicInfo = new ComicInfo();
                            comicInfo.Transcribe(_series, _chapter, metaPages);
                            _publisher.Publish(comicInfo);
                        }
                        done(this);
                    }
                }))();
            }
        }
        #endregion

        #region IDisposable
        /// <summary>
        /// Dispose of the object.
        /// </summary>
        public void Dispose() {}
        #endregion
    }
}