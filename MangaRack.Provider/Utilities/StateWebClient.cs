// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System;
using System.Net;
using System.Text;

namespace MangaRack.Provider {
	/// <summary>
	/// Represents a WebClient with state.
	/// </summary>
	public sealed class StateWebClient : WebClient {
		/// <summary>
		/// Contains each cookie.
		/// </summary>
		private CookieContainer _CookieContainer;

		/// <summary>
		/// Initialize a new instance of the StateWebClient class.
		/// </summary>
		public StateWebClient()
			: base() {
			// Initialize a new instance of the CookieContainer class.
			_CookieContainer =  new CookieContainer();
			// Set the encoding used to upload and download strings.
			Encoding = Encoding.UTF8;
		}

		/// <summary>
		/// Initialize a new instance of the StateWebClient class.
		/// </summary>
		/// <param name="Referer">The referer.</param>
		public StateWebClient(Uri Referer)
			: this() {
			// Set the referer.
			this.Referer = Referer;
		}

		/// <summary>
		/// Returns a WebRequest object for the specified resource.
		/// </summary>
		/// <param name="Address">Contains the address.</param>
		protected override WebRequest GetWebRequest(Uri Address) {
			// Retrieve a WebRequest object for the specified resource.
			WebRequest WebRequest = base.GetWebRequest(Address);
			// Check if the request is an HttpWebRequest.
			if (WebRequest is HttpWebRequest) {
				// Retrieve the HttpWebRequest.
				HttpWebRequest HttpWebRequest = (HttpWebRequest) WebRequest;
				// Set the cookies associated with the request.
				HttpWebRequest.CookieContainer = _CookieContainer;
				// Set the value of the user agent.
				HttpWebRequest.UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; Trident/6.0)";
				// Check if the referer is available and set the value of the referer.
				if (Referer != null) HttpWebRequest.Referer = Referer.AbsoluteUri;
			}
			// Return the WebRequest object.
			return WebRequest;
		}

		/// <summary>
		/// Returns a WebResponse object for the specified WebRequest.
		/// </summary>
		/// <param name="WebRequest">Contains the WebRequest object.</param>
		/// <returns></returns>
		protected override WebResponse GetWebResponse(WebRequest WebRequest) {
			// Retrieve a WebResponse object for the specified WebRequest.
			WebResponse WebResponse = base.GetWebResponse(WebRequest);
			// Check if the content type indicates a web document.
			if (WebResponse.ContentType.StartsWith("text/html")) {
				// Update the referer.
				Referer = WebRequest.RequestUri;
			}
			// Return the WebResponse object.
			return WebResponse;
		}

		/// <summary>
		/// Contains the referer.
		/// </summary>
		public Uri Referer { get; set; }
	}
}