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
			return (Request, Next) => {
				Request.CookieContainer = _CookieContainer;
				Request.Set("Referer", Referer);
				Next();
			};
		}

		/// <summary>
		/// Attach a state to the response.
		/// </summary>
		/// <param name="Callback">The callback.</param>
		private Action<HttpWebResponse> _Attach(Action<HttpWebResponse> Callback) {
			return (Response) => {
				if (Response != null && Response.ContentType != null && Response.ContentType.StartsWith("text/html")) {
					Referer = Response.ResponseUri.AbsoluteUri;
				}
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
			return;
		}

		/// <summary>
		/// Initialize a new instance of the HttpSession class.
		/// </summary>
		/// <param name="Referer">The value of the Referer HTTP header.</param>
		public HttpSession(string Referer) {
			this._CookieContainer = new CookieContainer();
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
			Delete(Address, Callback, (Request, Next) => {
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
			Http.Delete(Address, _Attach(Callback), _Attach(Middleware));
		}

		/// <summary>
		/// Request a HTTP resource using a GET.
		/// </summary>
		/// <param name="Address">The address.</param>
		/// <param name="Callback">The callback.</param>
		public void Get(string Address, Action<HttpWebResponse> Callback) {
			Get(Address, Callback, (Request, Next) => {
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
			Http.Get(Address, _Attach(Callback), _Attach(Middleware));
		}

		/// <summary>
		/// Request a HTTP resource using a POST.
		/// </summary>
		/// <param name="Address">The address.</param>
		/// <param name="Values">The values.</param>
		/// <param name="Callback">The callback.</param>
		public void Post(string Address, IEnumerable<KeyValuePair<string, string>> Values, Action<HttpWebResponse> Callback) {
			Post(Address, Values, Callback, (Request, Next) => {
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
			Http.Post(Address, Values, _Attach(Callback), _Attach(Middleware));
		}

		/// <summary>
		/// Request a HTTP resource using a PUT.
		/// </summary>
		/// <param name="Address">The address.</param>
		/// <param name="Values">The values.</param>
		/// <param name="Callback">The callback.</param>
		public void Put(string Address, IEnumerable<KeyValuePair<string, string>> Values, Action<HttpWebResponse> Callback) {
			Put(Address, Values, Callback, (Request, Next) => {
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