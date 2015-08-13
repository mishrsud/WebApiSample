using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.Http.ModelBinding.Binders;
using ApiTester.BL;
using ApiTester.Models;
using AttributeRouting;
using AttributeRouting.Web.Http;
using Newtonsoft.Json.Linq;

namespace ApiTester.ApiControllers
{
    [RoutePrefix("")]
    public class TestApiController : ApiController
    {
        private CertificateReader _certificateReader;
        private string _certStoreBasePath = ConfigurationManager.AppSettings["CertStoreBase"];

        [HttpGet, GET("read/{filename}/{password}")]
        public HttpResponseMessage ReadCertificate(string filename, string password)
        {
            _certificateReader = new CertificateReader();
            filename = Path.Combine(_certStoreBasePath, filename);
            _certificateReader.SetPrivateKeyFromP12(filename, password);

            CertificateInfo certificateInfo = GetCertificateInfo();


            return Request.CreateResponse(HttpStatusCode.OK, certificateInfo, new JsonMediaTypeFormatter());
        }

        [HttpPost, POST("write")]
        public HttpResponseMessage PostTest(JObject request)
        {
            IContentNegotiator negotiator = this.Configuration.Services.GetContentNegotiator();
            return Request.CreateResponse(HttpStatusCode.OK, request);
        }

        private CertificateInfo GetCertificateInfo()
        {
            var certificateInfo = new CertificateInfo()
            {
                FoundMerchantId = _certificateReader.FoundMerchantId,
                FoundPrivateKeyParameters = _certificateReader.PrivateKeyParameters != null,
                SubjectName = _certificateReader.Subject,
                ValidFrom = _certificateReader.ValidFrom,
                ValidTo = _certificateReader.ValidTo,
                ClientIpAddress = Request.GetClientIpAddress()
            };

            return certificateInfo;
        }
    }
}
