// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System;
using System.Diagnostics.Contracts;
using MangaRack.Provider.Interfaces;

namespace MangaRack.Provider
{
    public static class Extensions
    {
        #region Statics

        public static IProvider EnableApproximation(this IProvider provider)
        {
            Contract.Requires<ArgumentNullException>(provider != null);
            Contract.Ensures(Contract.Result<IProvider>() != null);
            return new Internals.Provider(provider);
        }

        #endregion
    }
}