// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System.Net;
using System.Reflection;

namespace TinyHttp {
	/// <summary>
	/// Represents the class providing extensions for the HttpWebRequest class.
	/// </summary>
	public static class ExtensionForHttpWebRequest {
		#region Methods
		/// <summary>
		/// Set the header.
		/// </summary>
		/// <param name="Request">The request.</param>
		/// <param name="Header">The header.</param>
		/// <param name="Value">The value.</param>
		public static void Set(this HttpWebRequest Request, string Header, string Value) {
			// Retrieve the property through reflection.
			PropertyInfo PropertyInfo = Request.GetType().GetProperty(Header.Replace("-", string.Empty));
			// Check if the property is available.
			if (PropertyInfo != null) {
				// Set the value of the header.
				PropertyInfo.SetValue(Request, Value, null);
			} else {
				// Set the value of the header.
				Request.Headers[Header] = Value;
			}
		}
		#endregion
	}
}