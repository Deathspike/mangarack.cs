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
		/// <param name="Type">The type.</param>
		public static IProvider Create(Type Type) {
			if (Type.GetInterfaces().FirstOrDefault(x => x.FullName.Equals(typeof(IProvider).FullName)) != null) {
				// Initialize the constructor information.
				ConstructorInfo ConstructorInfo = Type.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).FirstOrDefault(x => x.GetParameters().Length == 0);
				// Initialize a new instance of the Provider class.
				return ConstructorInfo == null ? null : new Provider(ConstructorInfo.Invoke(null) as IProvider);
			}
			return null;
		}
		#endregion
	}
}