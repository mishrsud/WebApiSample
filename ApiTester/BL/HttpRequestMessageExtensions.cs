using System.Net.Http;
using System.Web;

namespace ApiTester.BL
{
    public static class HttpRequestMessageExtensions
    {
        /// <summary>
        /// Gets the client IP address from the <see cref="HttpRequestMessage"/>.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        public static string GetClientIpAddress(this HttpRequestMessage request)
        {
            if (request.Properties.ContainsKey("MS_HttpContext"))
            {
                return ((HttpContextWrapper)request.Properties["MS_HttpContext"]).Request.UserHostAddress;
            }

            // System.ServiceModel hosted Web API
            //if (request.Properties.ContainsKey(RemoteEndpointMessageProperty.Name))
            //{
            //    RemoteEndpointMessageProperty prop;
            //    prop = (RemoteEndpointMessageProperty)request.Properties[RemoteEndpointMessageProperty.Name];
            //    return prop.Address;
            //}

            // When moving to OWIN
            // if (request.Properties.ContainsKey("MS_OwinContext"))
            // {
            //     return ((OwinContext)request.Properties["MS_OwinContext"]).Request.RemoteIpAddress;
            // }

            return null;
        }
    }
}