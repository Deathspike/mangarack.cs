// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using MangaRack.Core.Abstract;
using MangaRack.Core.Concrete.Xml;
using MangaRack.Provider;
using MangaRack.Provider.Abstract;

namespace MangaRack.Core.Concrete.Publisher {
    /// <summary>
    /// Represents a repair publisher.
    /// </summary>
    public sealed class Repair : IAsync<Repair> {
        /// <summary>
        /// Contains each broken page.
        /// </summary>
        private readonly IEnumerable<string> _brokenPages;

        /// <summary>
        /// Contains the chapter.
        /// </summary>
        private readonly IChapter _chapter;

        /// <summary>
        /// Contains the comic information.
        /// </summary>
        private readonly ComicInfo _comicInfo;

        /// <summary>
        /// Contains the publisher.
        /// </summary>
        private readonly IPublisher _publisher;
        
        #region Constructor
        /// <summary>
        /// Initialize a new instance of the Repair class.
        /// </summary>
        /// <param name="publisher">The publisher.</param>
        /// <param name="chapter">The chapter.</param>
        /// <param name="comicInfo">The comic information.</param>
        /// <param name="brokenPages">Each broken page.</param>
        public Repair(IPublisher publisher, IChapter chapter, ComicInfo comicInfo, IEnumerable<string> brokenPages) {
            _brokenPages = brokenPages;
            _chapter = chapter;
            _comicInfo = comicInfo;
            _publisher = publisher;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Indicates whether repairing has failed.
        /// </summary>
        public bool HasFailed { get; private set; }
        #endregion

        #region IAsync
        /// <summary>
        /// Populate asynchronously.
        /// </summary>
        /// <param name="done">The callback.</param>
        public void Populate(Action<Repair> done) {
            var pages = _chapter.Children.GetEnumerator();
            if (!pages.MoveNext()) {
                done(this);
            } else {
                var hasComicInfo = false;
                var next = null as Action;
                var newBrokenPages = new List<string>();
                var newComicInfoPage = (ComicInfoPage) null;
                var oldBrokenPages = _brokenPages.GetEnumerator();
                var oldComicInfoPage = null as ComicInfoPage;
                (next = () => {
                    while (oldBrokenPages.MoveNext()) {
                        var separator = oldBrokenPages.Current.IndexOf(':');
                        if (separator != -1) {
                            int number;
                            if (int.TryParse(oldBrokenPages.Current.Substring(0, separator), out number) && (_comicInfo == null || (oldComicInfoPage = _comicInfo.Pages.FirstOrDefault(x => x.Image == number)) != null)) {
                                var uniqueIdentifier = oldBrokenPages.Current.Substring(separator + 1).Trim();
                                while (pages.MoveNext()) {
                                    if (string.Equals(pages.Current.Location, uniqueIdentifier)) {
                                        pages.Current.Populate(page => {
                                            if ((newComicInfoPage = _publisher.Publish(page.Image, false, number)) != null) {
                                                if (string.Equals(newComicInfoPage.Type, "Deleted")) {
                                                    newBrokenPages.Add(string.Format("{0}: {1}", number.ToString("000"), page.Location));
                                                }
                                                if (_comicInfo != null) {
                                                    hasComicInfo = true;
                                                    _comicInfo.Pages.Remove(oldComicInfoPage);
                                                    _comicInfo.Pages.Add(newComicInfoPage);
                                                }
                                                next();
                                            }
                                        });
                                        return;
                                    }
                                }
                            }
                        }
                        HasFailed = true;
                    }
                    if (!HasFailed) {
                        if (newBrokenPages.Count != 0) {
                            _publisher.Publish(newBrokenPages);
                        }
                        if (hasComicInfo) {
                            _publisher.Publish(_comicInfo);
                        }
                    }
                    done(this);
                })();
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