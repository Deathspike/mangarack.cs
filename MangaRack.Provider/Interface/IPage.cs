// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
namespace MangaRack.Provider.Interface
{
    public interface IPage : IAsync
    {
        #region Properties

        byte[] Image { get; }
        string Location { get; }

        #endregion
    }
}