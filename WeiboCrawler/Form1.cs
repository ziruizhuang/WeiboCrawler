using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Text.RegularExpressions;

namespace WeiboCrawler
{
    //[PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    //[System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            webBrowser1.ScriptErrorsSuppressed = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            webBrowser1.Url = new Uri("http://weibo.com/");
           // webBrowser1.Url = new Uri("http://m.weibo.cn/");
            //webBrowser1.Navigate("https://m.weibo.cn/");
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            webBrowser1.Url = new Uri("http://weibo.com/1799920014/follow");
            webBrowser1.DocumentCompleted += webBrowser1_DocumentCompleted_Crawler;
            //webBrowser1.Navigate("http://weibo.com/1799920014/follow");

           
            /*
            
            */

        }

        void webBrowser1_DocumentCompleted_Crawler(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            //throw new NotImplementedException();
            if (e.Url == webBrowser1.Document.Url)
            {
                MessageBox.Show("Loaded");
                //MessageBox.Show(webBrowser1.DocumentText);
                tbOutput.Text = webBrowser1.DocumentText;

                // Define a test string.         
                string text = webBrowser1.DocumentText;

                // Define a regular expression for repeated words.
                
                Regex rx_html = new Regex(
                    @"^<SCRIPT>FM.view\(.*""domid"":""Pl_Official_LeftHisRelation__25"".*""html"":""(?<html>.*)""}\)</SCRIPT>$",
                    RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);
                

                // Find matches.
                MatchCollection htmls = rx_html.Matches(text);

                // Report the number of matches found.
                MessageBox.Show(htmls.Count + " htmls found");

                // Report on each match. 
                foreach (Match html in htmls)
                {
                    text = Regex.Unescape(html.Groups["html"].Value);

                    tbOutput.Text = text;
                    
                    Regex rx_ul = new Regex(
                        Regex.Escape(@"<ul class=""cnfList"" node-type=""userListBox"">")+
                        "(?<ul>.*)"+
                        Regex.Escape(@"</ul>"),
                        RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
                    // Find matches.
                    MatchCollection uls = rx_ul.Matches(text);

                    // Report the number of matches found.
                    MessageBox.Show(uls.Count + " uls found");
                    foreach (Match ul in uls)
                    {
                        text = Regex.Unescape(ul.Groups["ul"].Value);
                        Regex rx_li = new Regex(
                            Regex.Escape(@"<li class=""clearfix S_line1"" action-type=""itemClick"" action-data=""uid=")
                            +@"(?<uid>\d{10})"
                            +Regex.Escape(@"&fnick=")
                            +@"(?<fnick>.*)"
                            +Regex.Escape("&sex=")
                            +@"(?<sex>.)"
                            +Regex.Escape(@""">")
                            ,
                            //@"<li.*action-data=\\""uid=(?<uid>\d{10})&fnick=(?<fnick>.*)&sex=(?<sex>.*)[^>]\\"">",
                            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);
                        // Find matches.
                        MatchCollection lis = rx_li.Matches(text);

                        // Report the number of matches found.
                        MessageBox.Show(lis.Count + " lis found");
                        foreach (Match li in lis)
                        {
                            MessageBox.Show(li.Groups["uid"].Value + li.Groups["fnick"].Value);
                        }
                    }
                }

                //IEnumerable<HtmlAgilityPack.HtmlNode> nodes = nodeCollection.Nodes();
                //Array array = nodes.ToArray();
                //webBrowser1.DocumentText = array.GetValue(0).ToString();
                
            }
            else
            {
                MessageBox.Show("Not Loaded");
            }
        }
    }
}
