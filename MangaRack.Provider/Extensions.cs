// ======================================================================
using System;
using System.Diagnostics.Contracts;
using MangaRack.Provider.Interfaces;

namespace MangaRack.Provider
{
    public static class Extensions
    {
        #region Statics

        public static IProvider WithApproximation(this IProvider provider)
        {
            Contract.Requires<ArgumentNullException>(provider != null);
            Contract.Ensures(Contract.Result<IProvider>() != null);
            return new Internals.Provider(provider);
        }

        #endregion
    }
}