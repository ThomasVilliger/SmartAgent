using System;
using System.Collections.Generic;
using Restup.HttpMessage.Models.Contracts;
using Restup.HttpMessage.Models.Schemas;

namespace Restup.HttpMessage
{
    internal class HttpServerRequest : IHttpServerRequest
    {
        public IEnumerable<IHttpRequestHeader> Headers { get; }
        public HttpMethod? Method { get; }
        public Uri Uri { get; }
        public string HttpVersion { get; }
        public string ContentTypeCharset { get; }
        public IEnumerable<string> AcceptCharsets { get; }
        public int ContentLength { get; }
        public string ContentType { get; }
        public IEnumerable<string> AcceptEncodings { get; }
        public IEnumerable<string> AcceptMediaTypes { get; }
        public byte[] Content { get; }
        public bool IsComplete { get; }
        public HttpMethod? AccessControlRequestMethod { get; }
        public IEnumerable<string> AccessControlRequestHeaders { get; }
        public string Origin { get; }

        public HttpServerRequest(IEnumerable<IHttpRequestHeader> headers, HttpMethod? method, Uri uri,
            string httpVersion, string contentTypeCharset, IEnumerable<string> acceptCharsets, int contentLength,
            string contentType, IEnumerable<string> acceptEncodings, IEnumerable<string> acceptMediaTypes,
            byte[] content, bool isComplete, HttpMethod? accessControlRequestMethod, IEnumerable<string> accessControlRequestHeaders,
            string origin)
        {
            Headers = headers;
            Method = method;
            Uri = uri;
            HttpVersion = httpVersion;
            ContentTypeCharset = contentTypeCharset;
            AcceptCharsets = acceptCharsets;
            ContentLength = contentLength;
            ContentType = contentType;
            AcceptEncodings = acceptEncodings;
            AcceptMediaTypes = acceptMediaTypes;
            Content = content;
            IsComplete = isComplete;
            AccessControlRequestMethod = accessControlRequestMethod;
            AccessControlRequestHeaders = accessControlRequestHeaders;
            Origin = origin;
        }
    }
}