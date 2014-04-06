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
        private ulong _crawleruid = 0;
        private ulong _uid = 0;
        private int _depth = 2;
        private int _currentDepth = 0;
        private HashSet<ulong> _scannedList;

        private int timeStep = 5;//00;

        private List<ulong> _current = new List<ulong>();
        private List<ulong> _next = new List<ulong>();

        private int _workers = 0;
        private int _workerid = 0;

        public WeiboCrawler()
        {            
            _depth = 2;
            _scannedList = new HashSet<ulong>();
        }

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
        public ulong CrawlerUID
        {
            get
            {
                return _crawleruid;
            }
            set
            {
                _crawleruid = value;
            }

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
                    /*
                    foreach (ulong uid in _current)
                    {
                        //TODO: BFS
                        _findUser(uid);
                    }
                     * */
                    _findUser(_current[0]);
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
        private void _findUser(ulong __source)
        {
            try
            {
                Console.WriteLine(String.Format("WeiboCrawler: _findUser() at __source = {0}", __source));
                if (_scannedList.Contains(__source))
                {
                    Console.WriteLine(String.Format("WeiboCrawler: _findUser() __source = {0} already scanned", __source));
                    _current.Remove(__source);
                    return;
                }
                else if (__source <= 0)
                {
                    Console.WriteLine(String.Format("WeiboCrawler: _findUser() __source = {0} bot valid", __source));
                    return;
                }
                else if (__source == _crawleruid)
                {
                    Console.WriteLine(String.Format("Skip crawler id {}", __source));
                    return;
                }
                else
                {
                    //TODO: HTTP Events
                    WebBrowser b = new WebBrowser();
                    //b.Hide();
                    //b.Visible = false;
                    //b.AllowNavigation = false;
                    //b.AllowWebBrowserDrop = false;
                    b.Navigate(WeiboWeb.GetFollowUri(__source));
                    _workers++;
                    _workerid++;
                    //b.Tag = __source;
                    b.ScriptErrorsSuppressed = true;
                    Console.WriteLine(String.Format("new worker initiated {0},{1}", _workers, _workerid));
                    b.DocumentCompleted += b_DocumentCompleted;
                    b.NewWindow += b_NewWindow;
                    b.Disposed += b_Disposed;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception at line: 162");
                Console.WriteLine(ex.Message);
            }
        }

        void b_Disposed(object sender, EventArgs e)
        {
            _workers--;
            Console.WriteLine("Browser Disposed");
        }

        void b_NewWindow(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                e.Cancel = true;
                Console.WriteLine("Browser raised newWindow Event" + e.ToString());
                Console.WriteLine((sender as WebBrowser).Url);
            }
            catch
            {
                Console.WriteLine("Exception in WeiboCrawler.b_NewWindow()");
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
                                foreach (Match li in collection)
                                {
                                    WeiboUser u = new WeiboUser(li, source);
                                    u.StoreText();
                                    _next.Add(u.UID);
                                }
                                Console.WriteLine(String.Format("WeiboCrawler: found {0} relation for source {1}, pending {2}, next {3}", collection.Count, source, _current.Count, _next.Count));
                            }
                            else
                            {
                                Console.WriteLine(String.Format("no match following list for User {0} and Url {1}", source, (sender as WebBrowser).Document.Url));
                            }
                        }

                        Thread.Sleep(timeStep);
                        String nextPage = WeiboWeb.FindNextPage(webDocHtml);
                        if (nextPage != null)
                        {
                            string url = WeiboWeb.BaseUri.Trim('/') + nextPage.Replace("#place","");
                            Console.WriteLine(String.Format("Going to next page at: {0}", url ));
                            (sender as WebBrowser).Navigate(url);
                        }
                        else
                        {
                            _current.Remove(source);
                            _scannedList.Add(source);
                            //(sender as WebBrowser).Stop();
                            (sender as IDisposable).Dispose();
                            Console.WriteLine(String.Format("Browser Task for id {0} Finished! Current workers count is: {1}",source, _workers));
                            if (_current.Count == 0)
                            {
                                _current = _next;
                                _next = new List<ulong>();
                                _findUsers();
                            }
                            else
                            {
                                //Goto find next current job
                                _findUser(_current[0]);
                            }
                        }
                    }
                    else
                    {
                        //Source == Crawler
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
            try
            {
                _currentDepth = 0;
                Console.WriteLine("WeiboCrawler: FindUsers()");
                _current = new List<ulong>();
                _current.Add(_uid);
                _findUsers();
            }
            catch(Exception ex)
            {
                Console.WriteLine("Exception in WeiboCrawler.FindUsers()");
                Console.WriteLine(ex.Message);
            }
            //Console.WriteLine("WeiboCrawler: Crawling Ended");
        }
    }
}
