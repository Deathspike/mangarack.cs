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
			PropertyInfo PropertyInfo = Request.GetType().GetProperty(Header.Replace("-", string.Empty));
			if (PropertyInfo != null) {
				PropertyInfo.SetValue(Request, Value, null);
			} else {
				Request.Headers[Header] = Value;
			}
		}
		#endregion
	}
}