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
		public static void Delete(string Address, Action<HttpWebResponse> Callback, Action<HttpWebRequest, Action> Middleware) {
			Get(Address, Callback, (Request, Next) => {
				Request.Method = "DELETE";
				Middleware(Request, Next);
			});
		}

		/// <summary>
		/// Request a HTTP resource using a GET.
		/// </summary>
		/// <param name="Address">The address.</param>
		/// <param name="Callback">The callback.</param>
		public static void Get(string Address, Action<HttpWebResponse> Callback) {
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
		public static void Get(string Address, Action<HttpWebResponse> Callback, Action<HttpWebRequest, Action> Middleware) {
			HttpWebRequest Request = (HttpWebRequest) WebRequest.Create(Address);
			Request.Set("Accept-Encoding", "gzip,deflate");
			Request.Set("User-Agent", "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; Trident/6.0)");
			Middleware(Request, () => {
				Request.BeginGetResponse((IAsyncResult AsyncResult) => {
					try {
						Callback((HttpWebResponse) Request.EndGetResponse(AsyncResult));
					} catch (WebException e) {
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
		public static void Post(string Address, IEnumerable<KeyValuePair<string, string>> Values, Action<HttpWebResponse> Callback, Action<HttpWebRequest, Action> Middleware) {
			Get(Address, Callback, (Request, Next) => {
				Request.Method = "POST";
				Request.Set("Content-Type", "application/x-www-form-urlencoded; charset=utf-8");
				Middleware(Request, () => {
					Request.BeginGetRequestStream((AsyncResult) => {
						using (Stream Stream = Request.EndGetRequestStream(AsyncResult)) {
							byte[] Data = Encoding.UTF8.GetBytes(string.Join("&", Values.Select(x => string.Format("{0}={1}",  Uri.EscapeDataString(x.Key),  Uri.EscapeDataString(x.Value))).ToArray()));
							Stream.Write(Data, 0, Data.Length);
						}
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
		public static void Put(string Address, IEnumerable<KeyValuePair<string, string>> Values, Action<HttpWebResponse> Callback, Action<HttpWebRequest, Action> Middleware) {
			Post(Address, Values, Callback, (Request, Next) => {
				Request.Method = "PUT";
				Middleware(Request, Next);
			});
		}
		#endregion
	}
}