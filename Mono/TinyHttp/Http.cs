// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace TinyHttp {
	/// <summary>
	/// Represents the HTTP class.
	/// </summary>
	public static class Http {
		#region Methods
		/// <summary>
		/// Request a HTTP resource using a DELETE.
		/// </summary>
		/// <param name="Address">The address.</param>
		/// <param name="Callback">The callback.</param>
		public static void Delete(string Address, Action<HttpWebResponse> Callback) {
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
		public static void Delete(string Address, Action<HttpWebResponse> Callback, Action<HttpWebRequest, Action> Middleware) {
			// Request a HTTP resource.
			Get(Address, Callback, (Request, Next) => {
				// Set the method for the request.
				Request.Method = "DELETE";
				// Invoke the middleware.
				Middleware(Request, Next);
			});
		}

		/// <summary>
		/// Request a HTTP resource using a GET.
		/// </summary>
		/// <param name="Address">The address.</param>
		/// <param name="Callback">The callback.</param>
		public static void Get(string Address, Action<HttpWebResponse> Callback) {
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
		public static void Get(string Address, Action<HttpWebResponse> Callback, Action<HttpWebRequest, Action> Middleware) {
			// Initialize a new instance of the HttpWebRequest class.
			HttpWebRequest Request = (HttpWebRequest) WebRequest.Create(Address);
			// Set the Accept-Encoding header.
			Request.Set("Accept-Encoding", "gzip,deflate");
			// Set the User-Agent header.
			Request.Set("User-Agent", "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; Trident/6.0)");
			// Invoke the middleware.
			Middleware(Request, () => {
				// Begin an asynchronous request to an internet resource.
				Request.BeginGetResponse((IAsyncResult AsyncResult) => {
					// Attempt the following code.
					try {
						// End an asynchronous request to an internet resource.
						Callback((HttpWebResponse) Request.EndGetResponse(AsyncResult));
					} catch (WebException e) {
						// Invoke the callback.
						Callback((HttpWebResponse) e.Response);
					}
				}, Request);
			});
		}

		/// <summary>
		/// Request a HTTP resource using a POST.
		/// </summary>
		/// <param name="Address">The address.</param>
		/// <param name="Values">The values.</param>
		/// <param name="Callback">The callback.</param>
		public static void Post(string Address, IEnumerable<KeyValuePair<string, string>> Values, Action<HttpWebResponse> Callback) {
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
		public static void Post(string Address, IEnumerable<KeyValuePair<string, string>> Values, Action<HttpWebResponse> Callback, Action<HttpWebRequest, Action> Middleware) {
			// Request a HTTP resource.
			Get(Address, Callback, (Request, Next) => {
				// Set the method for the request.
				Request.Method = "POST";
				// Set the Content-Type header.
				Request.Set("Content-Type", "application/x-www-form-urlencoded; charset=utf-8");
				// Invoke the middleware.
				Middleware(Request, () => {
					// Begin an asynchronous request for a stream to write to.
					Request.BeginGetRequestStream((AsyncResult) => {
						// End an asynchronous request for a stream to write to.
						using (Stream Stream = Request.EndGetRequestStream(AsyncResult)) {
							// Initialize the data for the formatted form string.
							byte[] Data = Encoding.UTF8.GetBytes(string.Join("&", Values.Select(x => string.Format("{0}={1}",  Uri.EscapeDataString(x.Key),  Uri.EscapeDataString(x.Value))).ToArray()));
							// Write the data to the stream.
							Stream.Write(Data, 0, Data.Length);
						}
						// Continue.
						Next();
					}, Request);
				});
			});
		}

		/// <summary>
		/// Request a HTTP resource using a PUT.
		/// </summary>
		/// <param name="Address">The address.</param>
		/// <param name="Values">The values.</param>
		/// <param name="Callback">The callback.</param>
		public static void Put(string Address, IEnumerable<KeyValuePair<string, string>> Values, Action<HttpWebResponse> Callback) {
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
		public static void Put(string Address, IEnumerable<KeyValuePair<string, string>> Values, Action<HttpWebResponse> Callback, Action<HttpWebRequest, Action> Middleware) {
			// Request a HTTP resource.
			Post(Address, Values, Callback, (Request, Next) => {
				// Set the method for the request.
				Request.Method = "PUT";
				// Invoke the middleware.
				Middleware(Request, Next);
			});
		}
		#endregion
	}
}