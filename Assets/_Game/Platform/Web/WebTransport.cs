using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ReverseSolver.Core;
using UnityEngine;
using UnityEngine.Networking;

namespace ReverseSolver.Platform
{
    /* POSTs telemetry with UnityWebRequest (the browser's fetch underneath).
       Reports the HTTP status, or 0 when there was no response. */
    public sealed class WebTransport : MonoBehaviour, ITelemetryTransport
    {
        static WebTransport _instance;

        public static WebTransport Instance
        {
            get
            {
                if (_instance == null)
                {
                    var go = new GameObject("WebTransport");
                    DontDestroyOnLoad(go);
                    _instance = go.AddComponent<WebTransport>();
                }
                return _instance;
            }
        }

        public void Post(string url, IReadOnlyDictionary<string, string> headers, string body, Action<int> done) =>
            StartCoroutine(Send(url, headers, body, done));

        static IEnumerator Send(string url, IReadOnlyDictionary<string, string> headers, string body, Action<int> done)
        {
            using var req = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST)
            {
                uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(body)),
                downloadHandler = new DownloadHandlerBuffer(),
                timeout = 20
            };
            foreach (var h in headers) req.SetRequestHeader(h.Key, h.Value);
            yield return req.SendWebRequest();
            int status = req.result == UnityWebRequest.Result.ConnectionError ? 0 : (int)req.responseCode;
            done(status);
        }
    }
}
