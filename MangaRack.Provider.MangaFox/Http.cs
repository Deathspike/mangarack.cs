// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace MangaRack.Provider
{
    // TODO: This thing is merely a stub pending TinyHttp-PCL rewrite.
    internal static class Http
    {
        public static Action<HttpWebResponse> Wait(out Task<HttpWebResponse> task)
        {
            var manualResetEvent = new ManualResetEventSlim(false);
            HttpWebResponse outputResponse = null;

            task = Task.Factory.StartNew(() =>
            {
                manualResetEvent.Wait();
                return outputResponse;
            });

            return response =>
            {
                outputResponse = response;
                manualResetEvent.Set();
            };
        }

        public static Task<HttpWebResponse> GetAsync(string address)
        {
            Task<HttpWebResponse> task;
            TinyHttp.Http.Get(address, Wait(out task));
            return task;
        }

        public static Task<HttpWebResponse> PostAsync(string address, IEnumerable<KeyValuePair<string, string>> values)
        {
            Task<HttpWebResponse> task;
            TinyHttp.Http.Post(address, values, Wait(out task));
            return task;
        }
    }
}