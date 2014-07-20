// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using MangaRack.Provider.Interfaces;

namespace MangaRack.Provider
{
    // TODO: Name this in a more sensible way. It's not a factory now.
    public static class Factory
    {
        #region Methods

        public static IProvider Create<T>() where T : IProvider, new()
        {
            return new Internals.Provider(new T());
        }

        #endregion
    }
}