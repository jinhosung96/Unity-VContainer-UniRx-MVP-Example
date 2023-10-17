#if UNITASK_SUPPORT
using System;
using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Mine.Code.Framework.Manager.RequestBuilder
{
    public class Request
    {
        #region Inner Classes

        public static RequestBuilder Builder(string baseUrl) => new(baseUrl);

        public class RequestBuilder
        {
            readonly string baseUrl;
            string path;
            readonly List<(string name, string value)> requestHeaderList = new();

            public RequestBuilder(string baseUrl) => this.baseUrl = baseUrl;

            public RequestBuilder SetPath(string path)
            {
                this.path = path;
                return this;
            }

            public RequestBuilder AddRequestHeader((string name, string value) requestHeader)
            {
                requestHeaderList.Add(requestHeader);
                return this;
            }

            public GetRequest ToGetRequest() => new(baseUrl, path, requestHeaderList);
            public PostRequest ToPostRequest() => new(baseUrl, path, requestHeaderList);
            public PutRequest ToPutRequest() => new(baseUrl, path, requestHeaderList);
            public DeleteRequest ToDeleteRequest() => new(baseUrl, path, requestHeaderList);
        }

        #endregion

        protected readonly string baseUrl;
        protected readonly string path;
        readonly List<(string name, string value)> requestHeaderList = new();

        protected Request(string baseUrl, string path, List<(string name, string value)> requestHeaderList)
        {
            this.baseUrl = baseUrl;
            this.path = path;
            this.requestHeaderList = requestHeaderList;
        }

        protected async UniTask<UnityWebRequest> SendAsync(UnityWebRequest request)
        {
            Debug.Log($"Send : {request.url}");

            foreach (var requestHeader in requestHeaderList) request.SetRequestHeader(requestHeader.name, requestHeader.value);

            await request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success) Debug.Log($"{request.result} : {request.error}");

            return request;
        }
    }

    public class GetRequest : Request
    {
        public GetRequest(string baseUrl, string path, List<(string name, string value)> requestHeaderList) : base(baseUrl, path, requestHeaderList) { }
        public async UniTask<UnityWebRequest> GetAsync() => await GetWithHandlerAsync(new DownloadHandlerBuffer());
        public async UniTask<UnityWebRequest> GetWithHandlerAsync(DownloadHandler downloadHandler) => await SendAsync(new UnityWebRequest($"{baseUrl}/{path}", "GET") { downloadHandler = downloadHandler });
    }

    public class PostRequest : Request
    {
        public PostRequest(string baseUrl, string path, List<(string name, string value)> requestHeaderList) : base(baseUrl, path, requestHeaderList) { }
        public async UniTask<UnityWebRequest> PostAsync(string post) => await PostWithHandlerAsync(new UploadHandlerRaw(Encoding.UTF8.GetBytes(post)), new DownloadHandlerBuffer());
        public async UniTask<UnityWebRequest> PostWithHandlerAsync(UploadHandler uploadHandler, DownloadHandler downloadHandler) => await SendAsync(new UnityWebRequest($"{baseUrl}/{path}", "POST") { uploadHandler = uploadHandler, downloadHandler = downloadHandler });
    }

    public class PutRequest : Request
    {
        public PutRequest(string baseUrl, string path, List<(string name, string value)> requestHeaderList) : base(baseUrl, path, requestHeaderList) { }
        public async UniTask<UnityWebRequest> PutAsync(string body) => await PutWithHandlerAsync(new UploadHandlerRaw(Encoding.UTF8.GetBytes(body)), new DownloadHandlerBuffer());
        public async UniTask<UnityWebRequest> PutWithHandlerAsync(UploadHandler uploadHandler, DownloadHandler downloadHandler) => await SendAsync(new UnityWebRequest($"{baseUrl}/{path}", "PUT") { uploadHandler = uploadHandler, downloadHandler = downloadHandler });
    }

    public class DeleteRequest : Request
    {
        public DeleteRequest(string baseUrl, string path, List<(string name, string value)> requestHeaderList) : base(baseUrl, path, requestHeaderList) { }
        public async UniTask<UnityWebRequest> DeleteAsync() => await DeleteWithHandlerAsync(new DownloadHandlerBuffer());
        public async UniTask<UnityWebRequest> DeleteWithHandlerAsync(DownloadHandler downloadHandler) => await SendAsync(new UnityWebRequest($"{baseUrl}/{path}", "DELETE") { downloadHandler = downloadHandler });
    }
}
#endif