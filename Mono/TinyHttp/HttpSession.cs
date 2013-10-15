// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System;
using System.Collections.Generic;
using System.Net;

namespace TinyHttp {
	/// <summary>
	/// Represents a HTTP session with cookie and referrer support.
	/// </summary>
	public sealed class HttpSession {
		/// <summary>
		/// Contains each cookie.
		/// </summary>
		private CookieContainer _CookieContainer;

		#region Attach
		/// <summary>
		/// Attach a state to the request.
		/// </summary>
		/// <param name="Middleware">The middleware.</param>
		private Action<HttpWebRequest, Action> _Attach(Action<HttpWebRequest, Action> Middleware) {
			// Return an attached handler.
			return (Request, Next) => {
				// Set the cookies associated with the request.
				Request.CookieContainer = _CookieContainer;
				// Set the value of the Referer HTTP header
				Request.Set("Referer", Referer);
				// Continue.
				Next();
			};
		}

		/// <summary>
		/// Attach a state to the response.
		/// </summary>
		/// <param name="Callback">The callback.</param>
		private Action<HttpWebResponse> _Attach(Action<HttpWebResponse> Callback) {
			// Return an attached handler.
			return (Response) => {
				// Check if the response indicates a web document.
				if (Response != null && Response.ContentType != null && Response.ContentType.StartsWith("text/html")) {
					// Set the value of the Referer HTTP header
					Referer = Response.ResponseUri.AbsoluteUri;
				}
				// Invoke the callback.
				Callback(Response);
			};
		}
		#endregion

		#region Constructor
		/// <summary>
		/// Initialize a new instance of the HttpSession class.
		/// </summary>
		public HttpSession()
			: this(null) {
			// Stop the function.
			return;
		}

		/// <summary>
		/// Initialize a new instance of the HttpSession class.
		/// </summary>
		/// <param name="Referer">The value of the Referer HTTP header.</param>
		public HttpSession(string Referer) {
			// Initialize a new instance of the CookieContainer class.
			this._CookieContainer = new CookieContainer();
			// Set the value of the Referer HTTP header
			this.Referer = Referer;
		}
		#endregion

		#region Methods
		/// <summary>
		/// Request a HTTP resource using a DELETE.
		/// </summary>
		/// <param name="Address">The address.</param>
		/// <param name="Callback">The callback.</param>
		public void Delete(string Address, Action<HttpWebResponse> Callback) {
			// Request a HTTP resource.
			Delete(Address, Callback, (Request, Next) => {
				// Continue.
				Next();
			});
		}

		/// <summary>
		/// Request a HTTP resource using a DELETE.
		/// </summary>
		/// <param name="Address">The address.</param>
		/// <param name="Callback">The callback.</param>
		/// <param name="Middleware">The middleware.</param>
		public void Delete(string Address, Action<HttpWebResponse> Callback, Action<HttpWebRequest, Action> Middleware) {
			// Request a HTTP resource using a DELETE.
			Http.Delete(Address, _Attach(Callback), _Attach(Middleware));
		}

		/// <summary>
		/// Request a HTTP resource using a GET.
		/// </summary>
		/// <param name="Address">The address.</param>
		/// <param name="Callback">The callback.</param>
		public void Get(string Address, Action<HttpWebResponse> Callback) {
			// Request a HTTP resource.
			Get(Address, Callback, (Request, Next) => {
				// Continue.
				Next();
			});
		}

		/// <summary>
		/// Request a HTTP resource using a GET.
		/// </summary>
		/// <param name="Address">The address.</param>
		/// <param name="Callback">The callback.</param>
		/// <param name="Middleware">The middleware.</param>
		public void Get(string Address, Action<HttpWebResponse> Callback, Action<HttpWebRequest, Action> Middleware) {
			// Request a HTTP resource using a GET.
			Http.Get(Address, _Attach(Callback), _Attach(Middleware));
		}

		/// <summary>
		/// Request a HTTP resource using a POST.
		/// </summary>
		/// <param name="Address">The address.</param>
		/// <param name="Values">The values.</param>
		/// <param name="Callback">The callback.</param>
		public void Post(string Address, IEnumerable<KeyValuePair<string, string>> Values, Action<HttpWebResponse> Callback) {
			// Request a HTTP resource using POST.
			Post(Address, Values, Callback, (Request, Next) => {
				// Continue.
				Next();
			});
		}

		/// <summary>
		/// Request a HTTP resource using a POST.
		/// </summary>
		/// <param name="Address">The address.</param>
		/// <param name="Values">The values.</param>
		/// <param name="Callback">The callback.</param>
		/// <param name="Middleware">The middleware.</param>
		public void Post(string Address, IEnumerable<KeyValuePair<string, string>> Values, Action<HttpWebResponse> Callback, Action<HttpWebRequest, Action> Middleware) {
			// Request a HTTP resource using a POST.
			Http.Post(Address, Values, _Attach(Callback), _Attach(Middleware));
		}

		/// <summary>
		/// Request a HTTP resource using a PUT.
		/// </summary>
		/// <param name="Address">The address.</param>
		/// <param name="Values">The values.</param>
		/// <param name="Callback">The callback.</param>
		public void Put(string Address, IEnumerable<KeyValuePair<string, string>> Values, Action<HttpWebResponse> Callback) {
			// Request a HTTP resource.
			Put(Address, Values, Callback, (Request, Next) => {
				// Continue.
				Next();
			});
		}

		/// <summary>
		/// Request a HTTP resource using a PUT.
		/// </summary>
		/// <param name="Address">The address.</param>
		/// <param name="Values">The values.</param>
		/// <param name="Callback">The callback.</param>
		/// <param name="Middleware">The middleware.</param>
		public void Put(string Address, IEnumerable<KeyValuePair<string, string>> Values, Action<HttpWebResponse> Callback, Action<HttpWebRequest, Action> Middleware) {
			// Request a HTTP resource using a PUT.
			Http.Put(Address, Values, _Attach(Callback), _Attach(Middleware));
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the value of the Referer HTTP header.
		/// </summary>
		public string Referer { get; set; }
		#endregion
	}
}