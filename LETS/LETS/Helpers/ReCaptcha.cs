using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace LETS.Helpers
{
    public enum Theme { Red, White, BlackGlass, Clean }

    [Serializable]
    public class InvalidKeyException : ApplicationException
    {
        public InvalidKeyException() { }
        public InvalidKeyException(string message) : base(message) { }
        public InvalidKeyException(string message, Exception inner) : base(message, inner) { }
    }

    public class ReCaptcha : ActionFilterAttribute
    {
        public string OnActionExecuting()
        {
            try
            {
                var userIP = HttpContext.Current.Request.UserHostAddress;

                var privateKey = ConfigurationManager.AppSettings.GetValues("ReCaptcha.PrivateKey").FirstOrDefault();

                if (string.IsNullOrWhiteSpace(privateKey))
                    throw new InvalidKeyException("ReCaptcha.PrivateKey missing from appSettings");

                var postData = string.Format("&privatekey={0}&remoteip={1}&challenge={2}&response={3}",
                                             privateKey,
                                             userIP,
                                             HttpContext.Current.Request.Form["recaptcha_challenge_field"],
                                             HttpContext.Current.Request.Form["recaptcha_response_field"]);

                var postDataAsBytes = Encoding.UTF8.GetBytes(postData);

                // Create web request
                var request = (HttpWebRequest)WebRequest.Create("http://www.google.com/recaptcha/api/verify");
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = postDataAsBytes.Length;
                request.ServicePoint.Expect100Continue = false;
                var dataStream = request.GetRequestStream();
                dataStream.Write(postDataAsBytes, 0, postDataAsBytes.Length);
                dataStream.Close();

                try
                {
                    // Get the response.
                    var response = request.GetResponse();

                    using (dataStream = response.GetResponseStream())
                    {
                        using (var reader = new StreamReader(dataStream))
                        {
                            var responseFromServer = reader.ReadToEnd();
                            return responseFromServer;
                        }
                    }
                }
                catch (Exception)
                {
                    return string.Empty;
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }

    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString GenerateCaptcha(this HtmlHelper helper, Theme theme, string callBack = null)
        {
            const string htmlInjectString = @"<div id=""recaptcha_div""></div>
<script type=""text/javascript"">
    Recaptcha.create(""{0}"", ""recaptcha_div"", {{ theme: ""{1}"" {2}}});
</script>";

            var publicKey = ConfigurationManager.AppSettings.GetValues("ReCaptcha.PublicKey").FirstOrDefault();

            if (string.IsNullOrWhiteSpace(publicKey))
                throw new InvalidKeyException("ReCaptcha.PublicKey missing from appSettings");

            if (!string.IsNullOrWhiteSpace(callBack))
                callBack = string.Concat(", callback: ", callBack);

            var html = string.Format(htmlInjectString, publicKey, theme.ToString().ToLower(), callBack);
            return MvcHtmlString.Create(html);
        }
    }
}