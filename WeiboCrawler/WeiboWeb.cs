using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Net;

namespace WeiboCrawler
{
    static class WeiboWeb
    {
        static private string _baseUri = @"http://weibo.com/";
        static public string BaseUri
        {
            get
            {
                return _baseUri;
            }
        }
        static public string GetFollowUri(ulong __uid)
        {
            return _baseUri + __uid.ToString() + @"/follow";
        }
        static public string GetFansUri(ulong __uid)
        {
            return _baseUri + __uid.ToString() + @"/fans?retcode=6102";
        }
        static private Regex _rx_html = new Regex(
            @"^<SCRIPT>FM.view\(.*""domid"":""Pl_Official_LeftHisRelation__\d{2}"".*""html"":""(?<html>.*)""}\)</SCRIPT>$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);
        static private Regex _rx_ul = new Regex(
            Regex.Escape(@"<ul class=""cnfList"" node-type=""userListBox"">")
            + "(?<ul>.*)"
            + Regex.Escape(@"</ul>"),
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
        static private Regex _rx_li = new Regex(
            Regex.Escape(@"<li class=""clearfix S_line1"" action-type=""itemClick"" action-data=""uid=")
            + @"(?<uid>\d{10})"
            + Regex.Escape(@"&fnick=")
            + @"(?<fnick>.*)"
            + Regex.Escape("&sex=")
            + @"(?<sex>.)"
            + Regex.Escape(@""">"),
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);
        static private Regex _rx_uid = new Regex(
            @"^\s*"
            + Regex.Escape(@"$CONFIG['uid']")
            + @"(\s*=\s*')"//Do compability for white space
            + @"(?<uid>\d{10})"
            + Regex.Escape(@"';")
            + @"\s*$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);
        static private Regex _rx_oid = new Regex(
            @"^\s*"
            + Regex.Escape(@"$CONFIG['oid']")
            + @"(\s*=\s*')"//Do compability for white space
            + @"(?<oid>\d{10})"
            + Regex.Escape(@"';")
            + @"\s*$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);
        static private Regex _rx_onick = new Regex(
            @"^\s*"
            + Regex.Escape(@"$CONFIG['onick']")
            + @"(\s*=\s*')"//Do compability for white space
            + @"(?<onick>.*)"
            + Regex.Escape(@"';")
            + @"\s*$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);

        static private Regex _rx_next = new Regex(
            Regex.Escape(@"<a bpfilter=""page"" class=""W_btn_c"" href=""")
            + @"(?<url>.*)"
            + Regex.Escape(@"""><span>下一页</span></a>"),
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);


        static public MatchCollection ParseHtml(string __text)
        {
            try
            {
                // Find matches.
                MatchCollection htmls = _rx_html.Matches(__text);

                if (htmls!=null&&htmls.Count > 0)
                {
                    // Report on each match. 
                    foreach (Match html in htmls)
                    {
                        __text = Regex.Unescape(html.Groups["html"].Value);

                        // Find matches.
                        MatchCollection uls = _rx_ul.Matches(__text);
                        if (uls != null && uls.Count > 0)
                        {
                            foreach (Match ul in uls)
                            {
                                //__text = Regex.Unescape(ul.Groups["ul"].Value);
                                __text = ul.Groups["ul"].Value;
                                // Find matches.
                                return _rx_li.Matches(__text);
                            }
                        }
                        else
                        {
                            Console.WriteLine("Recognizable Ul Section Not Found.");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Recognizable Html Section Not Found.");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Exception in WeiboWeb.ParseHtml");
                Console.WriteLine(ex.Message);
            }
            return null;
        }
        static public String FindNextPage(string __text)
        {
            // Find matches.
            MatchCollection htmls = _rx_html.Matches(__text);

            // Report on each match. 
            foreach (Match html in htmls)
            {
                __text = Regex.Unescape(html.Groups["html"].Value);

                // Find matches.
                MatchCollection nexts = _rx_next.Matches(__text);

                foreach (Match next in nexts)
                {
                    __text = Regex.Unescape(next.Groups["url"].Value);
                    // Find matches.
                    return __text;
                }
            }
            return null;
        }
        static public ulong GetUID(string __text)
        {
            try
            {
                return ulong.Parse(_rx_uid.Match(__text).Groups["uid"].Value);
            }
            catch
            {
                return 0;
            }
        }
        static public ulong GetOID(string __text)
        {
            try
            {
                return ulong.Parse(_rx_oid.Match(__text).Groups["oid"].Value);
            }
            catch
            {
                return 0;
            }
        }
        static public string GetONick(string __text)
        {
            try
            {
                return _rx_onick.Match(__text).Groups["onick"].Value;
            }
            catch
            {
                return null;
            }
        }
    }
}
