// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System;
using System.Linq;
using System.Reflection;

namespace MangaRack.Provider {
	/// <summary>
	/// Represents the factory.
	/// </summary>
	public static class Factory {
		#region Methods
		/// <summary>
		/// Initialize a new instance of the Provider class.
		/// </summary>
		public static IProvider Create<T>() where T : IProvider, new() {
            return new Provider(new T());
		}
		#endregion
	}
}