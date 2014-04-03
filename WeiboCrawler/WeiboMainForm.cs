using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WeiboCrawler
{
    public partial class WeiboMainForm : Form
    {
        private ulong _uid_crawler;
        private ulong _uid_center;
        private string _cookie;
        //debug cookie
        private string _cookie2 = "SINAGLOBAL=6386544029736.754.1394111831866; ULV=1394546631320:94:94:15:2791135704144.726.1394546631270:1394545166850; SUBP=002A2c-gVlwEm1uAWxfgXELuuu1xVxBxAAPKju6XT9DYubls5DRbn70uHY-u_F=; ALF=1397138563; un=397273499@qq.com; wvr=5; UOR=,,login.sina.com.cn; UUG=usr1031; SUP=cv%3D1%26bt%3D1394546569%26et%3D1394632969%26d%3Dc909%26i%3Daa31%26us%3D1%26vf%3D0%26vt%3D0%26ac%3D0%26st%3D0%26uid%3D2398332747%26name%3D397273499%2540qq.com%26nick%3D%25E7%2594%25A8%25E6%2588%25B72398332747%26fmp%3D%26lcp%3D2011-12-23%252016%253A54%253A03; SSOLoginState=1394546569; UV5PAGE=usr513_166; UV5=usrmdins311157; _s_tentry=login.sina.com.cn; Apache=2791135704144.726.1394546631270";
        private string _cookie3 = "SINAGLOBAL=4975253441371.023.1365427785740; __utma=15428400.35490190.1370846438.1370846438.1370846438.1; UOR=www.dewen.org,widget.weibo.com,a.muzhigame.com; wvr=5; UUG=usr1024; SUS=SID-2398332747-1394711661-JA-yxobm-943cbf46ac8900ec2aea6a807f8e2c9d; SUE=es%3Df8238c7f25a45f92919d53e93107d394%26ev%3Dv1%26es2%3D1a3cf77b92f1e9021715446473985147%26rs0%3D2uw4h5Z8%252B%252Fifzf8DYYf4tHRCEauM3mlC4T9SBsLsEctPKk8E1OPtpreOnMKy0vavnlhatzb30%252BQaAId9tFFdpYwwRsM5rF8e5QHl0xLH4%252BUbO1ojLf%252FRr6c75hZDfYQXZ0ed9ACvM5W5kqoV0ZGEKQTVTUWM7GG622XMvcPuafQ%253D%26rv%3D0; SUP=cv%3D1%26bt%3D1394711661%26et%3D1394798061%26d%3Dc909%26i%3Df84e%26us%3D1%26vf%3D0%26vt%3D0%26ac%3D41%26st%3D0%26uid%3D2398332747%26name%3D397273499%2540qq.com%26nick%3D%25E7%2594%25A8%25E6%2588%25B72398332747%26fmp%3D%26lcp%3D2011-12-23%252016%253A54%253A03; SUB=AWUs1cOW4poveJZaWKoQEn1wWiGNvQ8JF4FAH14zy0pxyWFLF9Hhk%2BBiyaPjoGkkj3EmtIcsxvAvKEHoE2IgFAC4t%2F8Iy7YmAVmQHn3ynd%2F5iXxe0Zlbwq4AjKu6RJjCWVVMGQi7m%2FmclYu%2B8hrMmB0%3D; SUBP=002A2c-gVlwEm1uAWxfgXELuuu1xVxBxAAPKju6XT9DYubls5DRbn70uHY-u_E=; ALF=1397303659; SSOLoginState=1394711661; UV5PAGE=usr513_168; UV5=usrmdins311165; _s_tentry=weibo.com; Apache=9326469711959.361.1394711684161; ULV=1394711684180:100:8:4:9326469711959.361.1394711684161:1394635408272";
        private string _cookie4 = "SINAGLOBAL=6386544029736.754.1394111831866; ULV=1395628282274:211:211:13:6549806574043.185.1395628281721:1395627819096; SUB=AVgkOS%2FgCAI2i52hXHgpmDa9jfFpCePJtTL9HFTMHCbdyWFLF9Hhk%2BBiyaPjoGkkjydPyAjywsbEOwIqSK9QG7ExqQSrWzcQso2Womu7Eff34mDPbDgcMT4rhvqDkkHyIKLT%2FRmiFlH8Qxf1rz1T0PI%3D; SUBP=002A2c-gVlwEm1uAWxfgXELuuu1xVxBxAAPKju6XT9DYubls5DRbn70uHY-u_E%3D; un=397273499@qq.com; UOR=,,login.sina.com.cn; myuid=2398332747; wvr=5; ALF=1398220274; UUG=usr1026; SUS=SID-2398332747-1395628275-JA-afldm-8e2092f283e8efbbe9e0a9801c185f07; SUE=es%3D93fc0c0a0cc025e90f3786fd8a048e02%26ev%3Dv1%26es2%3Dfe8ff69087884dba945fca05eb2a3142%26rs0%3DxLfqozrHMpc9N3MNdDyUBNgrg7yevz4ZVoxPIpdSlRf86i4JIv3egsfgfiISygNMX%252BDk1dFMLkvvRFi3LaD5hZ86xGHbwhTWE13vDLrt5WgldE%252B%252F7YPpBh5DS1%252BkpLf3dR2mSFAeTJDAWNdo9cSFRaCssAgk6zmmFLEmMQqrVwA%253D%26rv%3D0; SUP=cv%3D1%26bt%3D1395628275%26et%3D1395714675%26d%3Dc909%26i%3Da9f8%26us%3D1%26vf%3D0%26vt%3D0%26ac%3D0%26st%3D0%26uid%3D2398332747%26name%3D397273499%2540qq.com%26nick%3D%25E7%2594%25A8%25E6%2588%25B72398332747%26fmp%3D%26lcp%3D2011-12-23%252016%253A54%253A03; SSOLoginState=1395628275; UV5PAGE=usr512_236; UV5=usrmdins312_168; _s_tentry=login.sina.com.cn; Apache=6549806574043.185.1395628281721";
        private WeiboCrawler _crawler;

        /// <summary>
        /// Set the uid of crawler's account
        /// </summary>
        /// <param name="__uid">The uid of crawler's account</param>
        public void SetCrawlerUID(ulong __uid)
        {
            _uid_crawler = __uid;
        }

        /// <summary>
        /// Set the uid of center user (center vertex)
        /// </summary>
        /// <param name="__uid">The uid of center user</param>
        public void SetCenterUID(ulong __uid)
        {
            _uid_center = __uid;
        }

        /// <summary>
        /// 
        /// </summary>
        public WeiboMainForm()
        {
            InitializeComponent();
            _crawler = new WeiboCrawler();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            new WeiboBrowser(this, WeiboWeb.BaseUri).ShowDialog();
        }

        /// <summary>
        /// Set the cookies of crawler
        /// </summary>
        /// <param name="__cookie">The crawler's cookies</param>
        public void SetCookie(Uri __uri, string __cookie)
        {
            _crawler.SetCookies(__uri,__cookie);
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            Console.WriteLine("WeiboMainForm: Crawler Init");
            //_crawler = new WeiboCrawler(_uid_center, 3);
            _crawler.UID = _uid_center;
            _crawler.Depth = 3;
            //_crawler = new WeiboCrawler(2265894862, 3);
            
            //_crawler.SetCookies(_cookie3);
            Console.WriteLine("WeiboMainForm: Crawler Run");
            _crawler.FindUsers();
        }

        private void getUserAgent()
        {

            string js = @"<script type='text/javascript'>function getUserAgent(){document.write(navigator.userAgent)}</script>";

            WebBrowser wb = new WebBrowser();
            wb.Url = new Uri("about:blank");
            wb.Document.Write(js);
            wb.Document.InvokeScript("getUserAgent");

            string userAgent = wb.DocumentText.Substring(js.Length);

            System.Console.WriteLine(userAgent);
        }

        private void button1_Click(object sender, EventArgs e)
        {

            //WeiboWebClient client = _crawler.Client;
            //client.SetCookies(_cookie4);
            //string html = client.DownloadString(new Uri(WeiboWeb.BaseUri));
            //Console.Write(html);
            WebBrowser b = new WebBrowser();
            b.Hide();
            b.Navigate("http://weibo.com/p/1005052265894862/follow?page=7#place");
            b.DocumentCompleted += b_DocumentCompleted;
        }

        void b_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            //throw new NotImplementedException();
            if (WeiboWeb.GetOID((sender as WebBrowser).DocumentText) > 0)
            {
                if (WeiboWeb.GetOID((sender as WebBrowser).DocumentText) != WeiboWeb.GetUID((sender as WebBrowser).DocumentText))
                {
                    string webDocHtml = (sender as WebBrowser).DocumentText;
                    WeiboWeb.ParseHtml(webDocHtml);
                    (sender as WebBrowser).Dispose();
                }
                else
                {
                    (sender as WebBrowser).Dispose();
                }
            }
        }
    }
}
