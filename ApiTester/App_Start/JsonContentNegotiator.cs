﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web;

namespace ApiTester
{
    /// <summary>
    /// A custom <see cref="IContentNegotiator"/> that only honours JSON
    /// </summary>
    public class JsonContentNegotiator : IContentNegotiator
    {
        private readonly JsonMediaTypeFormatter _jsonFormatter;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonContentNegotiator"/> class.
        /// </summary>
        /// <param name="formatter">The formatter.</param>
        public JsonContentNegotiator(JsonMediaTypeFormatter formatter)
        {
            _jsonFormatter = formatter;
        }

        /// <summary>
        /// Performs content negotiating by selecting the most appropriate <see cref="T:System.Net.Http.Formatting.MediaTypeFormatter" /> out of the passed in formatters for the given request that can serialize an object of the given type.
        /// </summary>
        /// <param name="type">The type to be serialized.</param>
        /// <param name="request">Request message, which contains the header values used to perform negotiation.</param>
        /// <param name="formatters">The set of <see cref="T:System.Net.Http.Formatting.MediaTypeFormatter" /> objects from which to choose.</param>
        /// <returns>
        /// The result of the negotiation containing the most appropriate <see cref="T:System.Net.Http.Formatting.MediaTypeFormatter" /> instance, or null if there is no appropriate formatter.
        /// </returns>
        public ContentNegotiationResult Negotiate(Type type, HttpRequestMessage request, IEnumerable<MediaTypeFormatter> formatters)
        {
            var result = new ContentNegotiationResult(_jsonFormatter, new MediaTypeHeaderValue("application/json"));
            return result;
        }
    }
}