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
		public static IProvider Create<T>() where T : IProvider {
			// Initialize a new instance of the Provider class.
			return Create(typeof(T));
		}

		/// <summary>
		/// Initialize a new instance of the Provider class.
		/// </summary>
		/// <param name="type">The type.</param>
		public static IProvider Create(Type type) {
			// Check if the interface is implemented.
			if (type.GetInterfaces().FirstOrDefault(x => x.FullName.Equals(typeof(IProvider).FullName)) != null) {
				// Initialize the constructor information.
				var constructorInfo = type.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).FirstOrDefault(x => x.GetParameters().Length == 0);
				// Initialize a new instance of the Provider class.
				return constructorInfo == null ? null : new Provider(constructorInfo.Invoke(null) as IProvider);
			}
			// Return null.
			return null;
		}
		#endregion
	}
}