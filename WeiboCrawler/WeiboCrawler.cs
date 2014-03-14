using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WeiboCrawler
{
    class WeiboCrawler
    {
        private ulong _uid = 0;
        private int _depth = 2;
        private HashSet<ulong> _scannedList;
        private string _cookie = "";


        public WeiboCrawler(ulong __uid)
        {
            _uid = __uid;
            _depth = 2;
            _scannedList = new HashSet<ulong>();
        }
        public WeiboCrawler(ulong __uid, int __depth)
        {
            _uid = __uid;
            _depth = __depth;
            _scannedList = new HashSet<ulong>();
        }
        /// <summary>
        /// Find users with control of depth and searching list
        /// </summary>
        /// <param name="__depth">current searching depth</param>
        /// <param name="__current">current searching list</param>
        private void _findUsers(int __depth, ref List<ulong> __current)
        {
            Console.WriteLine(String.Format("WeiboCrawler: _findUsers() at __depth = {0}",__depth));

            if (__depth > _depth)
            {
                Console.WriteLine(String.Format("WeiboCrawler: _findUsers() __depth = {0} exceeds limit _depth = {1}", __depth,_depth));
                return;
            }
            else
            {
                List<ulong> next = new List<ulong>();             
                foreach (ulong uid in __current)
                {
                    //TODO: BFS
                    _findUser(uid, ref next);
                    //__current.Remove(uid);
                }
                _findUsers(__depth + 1, ref next);
            }
        }
        /// <summary>
        /// Find user followings
        /// </summary>
        /// <param name="__source">The user uid being searched</param>
        /// <param name="__next">The next searching list</param>
        private void _findUser(ulong __source, ref List<ulong> __next)
        {
            Console.WriteLine(String.Format("WeiboCrawler: _findUser() at __source = {0}", __source));
            if (_scannedList.Contains(__source))
            {
                Console.WriteLine(String.Format("WeiboCrawler: _findUser() __source = {0} already scanned", __source));
                return;
            }
            else
            {
                //TODO: HTTP Events
                string webDocHtml = getUserFollowingListHtml(__source);
                MatchCollection collection = WeiboWeb.ParseHtml(webDocHtml);
                Console.WriteLine(String.Format("WeiboCrawler: found {0} relation for source {1}", collection.Count, __source));
                foreach (Match li in collection)
                {
                    WeiboUser u = new WeiboUser(li,__source);
                    u.StoreSQLite();
                    __next.Add(u.UID);
                    
                }
                _scannedList.Add(__source);
            }
        }
        
        public void FindUsers()
        {
            Console.WriteLine("WeiboCrawler: FindUsers()");
            List<ulong> current = new List<ulong>();
            current.Add(_uid);
            _findUsers(0, ref current);
            Console.WriteLine("WeiboCrawler: Crawling Ended");
        }

        private string getUserFollowingListHtml(ulong __uid)
        {
            if (__uid > 0)
            {

                WeiboWebClient client = new WeiboWebClient();
                client.Encoding = Encoding.UTF8;
                client.BaseAddress = WeiboWeb.GetFollowUri(__uid);
                //client.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/33.0.1750.146 Safari/537.36");
                client.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.2; WOW64; Trident/7.0; .NET4.0E; .NET4.0C; .NET CLR 3.5.30729; .NET CLR 2.0.50727; .NET CLR 3.0.30729)");
                client.SetCookies(_cookie);
                //Task<string> task = client.DownloadStringTaskAsync(WeiboWeb.GetFollowUri(__uid));
                string str = client.DownloadString(WeiboWeb.GetFollowUri(__uid));
                Console.WriteLine(client.ResponseHeaders.ToString());
                return str;
            }
            else
            {
                Console.Write("__uid not defined");
                return null;
            }
        }

        public void SetCookies(string __cookies)
        {
            _cookie = __cookies;
        }
    }
}
