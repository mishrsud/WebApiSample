using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace ApiTester
{
    public class NotAcceptableConnegHandler : DelegatingHandler
    {
        private const string AllMediaTypesRange = "*/*";

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var acceptHeader = request.Headers.Accept;
            var contentTypeHeader = request.Content.Headers.ContentType;

            bool hasFormatterForRequestedMediaType = false;

            if (IsRequestWithBody(request))
            {
                hasFormatterForRequestedMediaType = GlobalConfiguration
                    .Configuration
                    .Formatters
                    .Any(formatter => formatter.SupportedMediaTypes.Contains(contentTypeHeader));
            }
            else if (acceptHeader.All(x => x.MediaType != AllMediaTypesRange))
            {
                hasFormatterForRequestedMediaType = GlobalConfiguration
                    .Configuration
                    .Formatters
                    .Any(formatter => acceptHeader.Any(mediaType => formatter.SupportedMediaTypes.Contains(mediaType)));
            }


            if (!hasFormatterForRequestedMediaType)
            {
                var httpResponse = new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.NotAcceptable,
                    ReasonPhrase = "Unsupported content-type. Did you forget the Accept or Content-Type header? [only application/json supported]"
                };

                return Task<HttpResponseMessage>.Factory.StartNew(() => httpResponse, cancellationToken);
            }


            return base.SendAsync(request, cancellationToken);
        }

        private bool IsRequestWithBody(HttpRequestMessage request)
        {
            //TODO Patch and Search can also have body
            if (request.Method.Equals(HttpMethod.Post) || request.Method.Equals(HttpMethod.Put))
            {
                return true;
            }

            return false;
        }
    }
}