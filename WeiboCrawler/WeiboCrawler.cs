using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WeiboCrawler
{
    class WeiboCrawler
    {
        private ulong _uid = 0;
        private int _depth = 2;
        private int _currentDepth = 0;
        private HashSet<ulong> _scannedList;

        private List<ulong> _current = new List<ulong>();
        private List<ulong> _next = new List<ulong>();

        private int _workers = 0;

        private WeiboWebClient _client;
        public WeiboWebClient Client
        {
            get
            {
                return _client;
            }
        }
        public WeiboCrawler()
        {            
            _depth = 2;
            _scannedList = new HashSet<ulong>();
            _client = new WeiboWebClient();
        }

        public WeiboCrawler(ulong __uid)
        {
            _uid = __uid;
            _depth = 2;
            _scannedList = new HashSet<ulong>();
            _client = new WeiboWebClient();
        }
        public WeiboCrawler(ulong __uid, int __depth)
        {
            _uid = __uid;
            _depth = __depth;
            _scannedList = new HashSet<ulong>();
            _client = new WeiboWebClient();
        }
        public ulong UID
        {
            get
            {
                return _uid;
            }
            set
            {
                _uid = value;
            }
        }
        public int Depth
        {
            get
            {
                return _depth;
            }
            set
            {
                _depth = value;
            }
        }

        /// <summary>
        /// Find users with control of depth and searching list
        /// </summary>
        /// <param name="_currentDepth">current searching depth</param>
        /// <param name="__current">current searching list</param>
        private void _findUsers()
        {
            try
            {
                _currentDepth++;
                Console.WriteLine(String.Format("WeiboCrawler: _findUsers() at __depth = {0}", _currentDepth));
                if (_currentDepth > _depth)
                {
                    Console.WriteLine(String.Format("WeiboCrawler: _findUsers() __depth = {0} exceeds limit _depth = {1}", _currentDepth, _depth));
                    return;
                }
                else if (_current.Count == 0)
                {
                    Console.WriteLine("No more targets");
                }
                else
                {
                    foreach (ulong uid in _current)
                    {
                        //TODO: BFS
                        _findUser(uid);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("error in _findUsers()");
                Console.WriteLine(ex.Message);
            }
        }
        /// <summary>
        /// Find user followings
        /// </summary>
        /// <param name="__source">The user uid being searched</param>
        /// <param name="__next">The next searching list</param>
        private void _findUser(ulong __source)
        {
            try
            {
                Console.WriteLine(String.Format("WeiboCrawler: _findUser() at __source = {0}", __source));
                if (_scannedList.Contains(__source))
                {
                    Console.WriteLine(String.Format("WeiboCrawler: _findUser() __source = {0} already scanned", __source));
                    return;
                }
                else if (__source <= 0)
                {
                    Console.WriteLine(String.Format("WeiboCrawler: _findUser() __source = {0} bot valid", __source));
                    return;
                }
                else
                {
                    //TODO: HTTP Events
                    WebBrowser b = new WebBrowser();
                    b.Hide();
                    b.Visible = false;
                    b.Navigate(WeiboWeb.GetFollowUri(__source));
                    _workers++;
                    b.DocumentCompleted += b_DocumentCompleted;
                    b.Disposed += b_Disposed;
                    b.NewWindow += b_NewWindow;
                    /*
                    string webDocHtml = getUserFollowingListHtml(__source);
                
                    if (webDocHtml != null)
                    {
                        MatchCollection collection = WeiboWeb.ParseHtml(webDocHtml);
                        Console.WriteLine(String.Format("WeiboCrawler: found {0} relation for source {1}", collection.Count, __source));
                        foreach (Match li in collection)
                        {
                            WeiboUser u = new WeiboUser(li, __source);
                            u.StoreSQLite();
                            __next.Add(u.UID);
                        }
                    }
                    _scannedList.Add(__source);
                     * */
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception at line: 162");
                Console.WriteLine(ex.Message);
            }
        }

        void b_NewWindow(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //throw new NotImplementedException();
            Console.WriteLine("Browser raised newWindow Event" + e.ToString());
            Console.WriteLine((sender as WebBrowser).Url);
        }

        void b_Disposed(object sender, EventArgs e)
        {
            try
            {
                _workers--;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in b_Disposed");
                Console.WriteLine(ex.Message);
            }
        }

        void b_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                string webDocHtml = (sender as WebBrowser).DocumentText;
                ulong source = WeiboWeb.GetOID(webDocHtml);
                ulong crawler = WeiboWeb.GetUID(webDocHtml);
                if (source > 0)
                {
                    if (source != crawler)
                    {
                        //Got correct response
                        if (webDocHtml != null)
                        {
                            MatchCollection collection = WeiboWeb.ParseHtml(webDocHtml);
                            if (collection!=null)
                            {
                                Console.WriteLine(String.Format("WeiboCrawler: found {0} relation for source {1}", collection.Count, source));
                                foreach (Match li in collection)
                                {
                                    WeiboUser u = new WeiboUser(li, source);
                                    u.StoreText();
                                    _next.Add(u.UID);
                                }
                            }
                            else
                            {
                                Console.WriteLine(String.Format("no match following list for User {0} and Url {1}", source, (sender as WebBrowser).Document.Url));
                            }
                        }
                        
                        String nextPage = WeiboWeb.FindNextPage(webDocHtml);
                        if (nextPage != null)
                        {
                            (sender as WebBrowser).Navigate(WeiboWeb.BaseUri.Trim('/') + nextPage);
                        }
                        else
                        {
                            _current.Remove(source);
                            _scannedList.Add(source);
                            (sender as WebBrowser).Dispose();
                            if (_current.Count == 0 && _workers == 0)
                            {
                                _current = _next;
                                _next = new List<ulong>();
                                _findUsers();
                            }
                        }
                    }
                    else
                    {
                        (sender as WebBrowser).Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception at line: 214");
                Console.WriteLine(ex.Message);
            }
        }
        
        public void FindUsers()
        {
            _currentDepth = 0;
            Console.WriteLine("WeiboCrawler: FindUsers()");
            _current = new List<ulong>();
            _current.Add(_uid);
            _findUsers();
            Console.WriteLine("WeiboCrawler: Crawling Ended");
        }

        private string getUserFollowingListHtml(ulong __uid)
        {
            if (__uid > 0)
            {
                string str = "";
                try
                {
                    str = _client.CustomDownloadString(WeiboWeb.GetFollowUri(__uid));
                    Console.WriteLine("Request Headers: " + _client.Headers.ToString());
                    Console.WriteLine("Response Headers: " + _client.ResponseHeaders.ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                return str;
            }
            else
            {
                Console.Write("__uid not defined");
                return null;
            }
        }

       

        public void SetCookies(Uri __uri, string __cookies)
        {
            _client.SetCookies(__uri, __cookies);
        }
    }
}
