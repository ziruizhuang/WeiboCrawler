using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WeiboCrawler
{
    class WeiboWebClient : WebClient
    {
        [System.Security.SecuritySafeCritical]
        public WeiboWebClient()
            : base()
        {
        }
        private CookieContainer _cookieContainer = new CookieContainer();
        public CookieContainer CookieContainer
        {
            get
            {
                return _cookieContainer;
            }
        }

        protected override WebRequest GetWebRequest(Uri myAddress)
        {
            WebRequest request = base.GetWebRequest(myAddress);
            if (request is HttpWebRequest)
            {
                (request as HttpWebRequest).CookieContainer = CookieContainer;
                (request as HttpWebRequest).AllowAutoRedirect = true;
            }
            return request;
        }

        public void SetCookies(string __cookies)
        {
            //_cookieContainer.SetCookies(new Uri(".weibo.com"), "Cookie: "+__cookies);
            
            foreach (string cookie in __cookies.Split(';'))
            {
                string name = cookie.Split('=')[0];
                string value = cookie.Substring(name.Length + 1);
                string path = "/";
                string domain = ".weibo.com";
                try
                {
                    _cookieContainer.Add(new Cookie(name.Trim(), value.Trim(), path, domain));
                }
                catch
                {

                }
            }
            
        }
    }
}
