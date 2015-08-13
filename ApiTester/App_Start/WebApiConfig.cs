using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;
using AttributeRouting.Web.Http.WebHost;

namespace ApiTester
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpAttributeRoutes();

            var jsonFormatter = new JsonMediaTypeFormatter();

            config.Formatters.Clear();
            config.Formatters.Add(jsonFormatter);
            
            config.Services.Replace(typeof(IContentNegotiator), new JsonContentNegotiator(jsonFormatter));


            config.MessageHandlers.Add(new NotAcceptableConnegHandler());


            // We don't want to return XML, so remove the XmlMediaTypeFormatter
            //MediaTypeFormatterCollection mediaTypeFormatters = config.Formatters;
            //mediaTypeFormatters.Remove(mediaTypeFormatters.XmlFormatter);
        }
    }
}
