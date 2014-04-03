using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WeiboCrawler
{
    class WeiboWebClient : WebClient
    {
        private Regex _jsLocationReplacePattern = new Regex(Regex.Escape(@"location.replace(""") + @"(?<uri>.*)" + Regex.Escape(@""");"));
        private string _lastUri = "";
        [System.Security.SecuritySafeCritical]
        public WeiboWebClient()
            : base()
        {
            _cookieContainer = new CookieContainer();
            _cookieContainer.PerDomainCapacity = 50;
            _cookieContainer.MaxCookieSize = 4096 * 16;
            _cookieContainer.Capacity = 200;

        }
        private CookieContainer _cookieContainer;
        public CookieContainer CookieContainer
        {
            get
            {
                return _cookieContainer;
            }
            set
            {
                _cookieContainer = value;
            }
        }

        public String CustomDownloadString(String __uri)
        {
            string content = this.DownloadString(__uri);
            //TODO: search for redirect
            string uri = _jsLocationReplacePattern.Match(content).Groups["uri"].Value;
            if (uri != "")
            {
                uri = uri.Replace("page.service.", "");
                return this.CustomDownloadString(uri);
            }
            else
            {
                return content;
            }
        }

        protected override WebRequest GetWebRequest(Uri __uri)
        {
            
            this.Encoding = Encoding.UTF8;
            WebRequest request = base.GetWebRequest(__uri);
            Console.WriteLine(String.Format("WeiboWebClient: navigete to Uri {0}", __uri));
            Console.WriteLine("**** -INFO- WeiboWebClient.GetWebRequest ********");
            Console.WriteLine(": ** URI: ********");
            Console.WriteLine(":    " + __uri.ToString());
            Console.WriteLine();
            if (request is HttpWebRequest)
            {
                (request as HttpWebRequest).AllowAutoRedirect = false;
                (request as HttpWebRequest).AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                (request as HttpWebRequest).UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.2; WOW64; Trident/7.0; .NET4.0E; .NET4.0C; .NET CLR 3.5.30729; .NET CLR 2.0.50727; .NET CLR 3.0.30729)";
                (request as HttpWebRequest).Headers.Set(HttpRequestHeader.AcceptEncoding, "gzip,deflate");
                (request as HttpWebRequest).Method = WebRequestMethods.Http.Get;
                (request as HttpWebRequest).CookieContainer = _cookieContainer;
                Console.WriteLine((request as HttpWebRequest).Headers.Get(HttpRequestHeader.Cookie.ToString()));
            }
            //(request as HttpWebRequest).Referer = @"http://weibo.com/";
            (request as HttpWebRequest).Accept = @"text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            //(request as HttpWebRequest).ContentType = @"application/x-www-form-urlencoded";
            //(request as HttpWebRequest).Host = @"weibo.com";
            //(request as HttpWebRequest).Connection = @"keep-alive";//Cannot Set by this Method
            (request as HttpWebRequest).Referer = _lastUri;
            Console.WriteLine("Referer: "+(request as HttpWebRequest).Referer);
            this._lastUri = __uri.ToString();
            return request;
        }

        public void SetCookies(Uri __uri, string __cookies)
        {
            //_cookieContainer.SetCookies(new Uri(".weibo.com"), "Cookie: "+__cookies);

            //_cookie = __cookies;

            _cookieContainer.SetCookies(new Uri(__uri.GetLeftPart(UriPartial.Authority)), __cookies.Replace(",", "%2C").Replace(";", ","));
            
            //CookieContainer _test_ = new CookieContainer();
            //_test_.SetCookies(new Uri("http://www.weibo.com/"), _cookie);
            //Console.WriteLine("_test_");
            /*
            foreach (string cookie in __cookies.Split(';'))
            {
                string name = cookie.Split('=')[0];
                string value = cookie.Substring(name.Length + 1);
                string path = "/";
                string domain = ".weibo.com";
                //string path = "";
                //string domain = "";
                try
                {
                    _cookieContainer.Add(new Cookie(name.Trim(), value.Trim(), path, domain));
                }
                catch
                {

                }
            }
            Console.WriteLine(_cookieContainer.GetCookieHeader(new Uri(@"http://weibo.com")));
             */
        }

        protected override WebResponse GetWebResponse(WebRequest __request)
        {
            
            WebResponse response = base.GetWebResponse(__request);
            
            String setCookieHeader = response.Headers[HttpResponseHeader.SetCookie];

            foreach (Cookie cook in ((HttpWebResponse)response).Cookies)
            {
                Console.WriteLine("Cookie:");
                Console.WriteLine("{0} = {1}", cook.Name, cook.Value);
                Console.WriteLine("Domain: {0}", cook.Domain);
                Console.WriteLine("Path: {0}", cook.Path);
                Console.WriteLine("Port: {0}", cook.Port);
                Console.WriteLine("Secure: {0}", cook.Secure);

                Console.WriteLine("When issued: {0}", cook.TimeStamp);
                Console.WriteLine("Expires: {0} (expired? {1})",
                    cook.Expires, cook.Expired);
                Console.WriteLine("Don't save: {0}", cook.Discard);
                Console.WriteLine("Comment: {0}", cook.Comment);
                Console.WriteLine("Uri for comments: {0}", cook.CommentUri);
                Console.WriteLine("Version: RFC {0}", cook.Version == 1 ? "2109" : "2965");

                // Show the string representation of the cookie.
                Console.WriteLine("String: {0}", cook.ToString());
                _cookieContainer.Add(cook);
            }
            switch ((response as HttpWebResponse).StatusCode)
            {
                case HttpStatusCode.OK:
                    return response;
                case HttpStatusCode.Redirect:
                    string location = (response as HttpWebResponse).GetResponseHeader(HttpResponseHeader.Location.ToString());
                    response.Close();
                    WebRequest request = this.GetWebRequest(new Uri(location));
                    return this.GetWebResponse(request); 
                default:
                    return response;
            }
        }

        



        [DllImport("wininet.dll", SetLastError = true)]
        public static extern bool InternetGetCookieEx(
            string url,
            string cookieName,
            StringBuilder cookieData,
            ref int size,
            Int32 dwFlags,
            IntPtr lpReserved);

        private const Int32 InternetCookieHttponly = 0x2000;

        /// <summary>
        /// Gets the URI cookie container.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns></returns>
        public static CookieContainer GetUriCookieContainer(Uri uri)
        {
            CookieContainer cookies = null;
            // Determine the size of the cookie
            int datasize = 8192 * 16;
            StringBuilder cookieData = new StringBuilder(datasize);
            if (!InternetGetCookieEx(uri.ToString(), null, cookieData, ref datasize, InternetCookieHttponly, IntPtr.Zero))
            {
                if (datasize < 0)
                    return null;
                // Allocate stringbuilder large enough to hold the cookie
                cookieData = new StringBuilder(datasize);
                if (!InternetGetCookieEx(
                    uri.ToString(),
                    null, cookieData,
                    ref datasize,
                    InternetCookieHttponly,
                    IntPtr.Zero))
                    return null;
            }
            if (cookieData.Length > 0)
            {
                cookies = new CookieContainer();
                cookies.SetCookies(uri, cookieData.ToString().Replace(';', ','));
            }
            Console.WriteLine("**** -INFO- WeiboWebClient.GetUriCookieContainer ********");
            Console.WriteLine("** URI: ********");
            Console.WriteLine(":"+uri.ToString());
            Console.WriteLine("** Cookie: ********");
            Console.WriteLine(":" + cookieData);
            return cookies;
        }
    }
}
