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
    /// Represents a chapter.
    /// </summary>
    internal sealed class Chapter : IChapter {
        /// <summary>
        /// Contains the chapter.
        /// </summary>
        private readonly IChapter _chapter;

        /// <summary>
        /// Contains each child.
        /// </summary>
        private IEnumerable<IPage> _children;

        #region Constructor
        /// <summary>
        /// Initialize a new instance of the Chapter class.
        /// </summary>
        /// <param name="chapter">The chapter.</param>
        public Chapter(IChapter chapter) {
            _chapter = chapter;
            Number = chapter.Number;
        }
        #endregion

        #region IAsync
        /// <summary>
        /// Populate asynchronously.
        /// </summary>
        /// <param name="done">The callback.</param>
        public void Populate(Action<IChapter> done) {
            _chapter.Populate(() => {
                _children = _chapter.Children.Select(x => new Page(x) as IPage).ToArray();
                done(this);
            });
        }
        #endregion

        #region IChapter
        /// <summary>
        /// Contains each child.
        /// </summary>
        public IEnumerable<IPage> Children {
            get { return _children; }
        }

        /// <summary>
        /// Contains the location.
        /// </summary>
        public string Location {
            get { return _chapter.Location; }
        }

        /// <summary>
        /// Contains the number.
        /// </summary>
        public double Number { get; set; }

        /// <summary>
        /// Contains the title.
        /// </summary>
        public string Title {
            get { return _chapter.Title; }
        }

        /// <summary>
        /// Contains the unique identifier.
        /// </summary>
        public string UniqueIdentifier {
            get { return _chapter.UniqueIdentifier; }
        }

        /// <summary>
        /// Contains the volume.
        /// </summary>
        public double Volume {
            get { return _chapter.Volume; }
        }
        #endregion

        #region IDisposable
        /// <summary>
        /// Dispose of the object.
        /// </summary>
        public void Dispose() {
            _chapter.Dispose();
        }
        #endregion
    }
}