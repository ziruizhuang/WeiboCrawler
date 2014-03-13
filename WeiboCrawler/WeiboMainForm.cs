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
        private uint _uid_crawler;
        private uint _uid_center;
        private string _cookie;
        //debug cookie
        private string _cookie2 = "SINAGLOBAL=6386544029736.754.1394111831866; ULV=1394546631320:94:94:15:2791135704144.726.1394546631270:1394545166850; SUBP=002A2c-gVlwEm1uAWxfgXELuuu1xVxBxAAPKju6XT9DYubls5DRbn70uHY-u_F=; ALF=1397138563; un=397273499@qq.com; wvr=5; UOR=,,login.sina.com.cn; UUG=usr1031; SUP=cv%3D1%26bt%3D1394546569%26et%3D1394632969%26d%3Dc909%26i%3Daa31%26us%3D1%26vf%3D0%26vt%3D0%26ac%3D0%26st%3D0%26uid%3D2398332747%26name%3D397273499%2540qq.com%26nick%3D%25E7%2594%25A8%25E6%2588%25B72398332747%26fmp%3D%26lcp%3D2011-12-23%252016%253A54%253A03; SSOLoginState=1394546569; UV5PAGE=usr513_166; UV5=usrmdins311157; _s_tentry=login.sina.com.cn; Apache=2791135704144.726.1394546631270";

        /// <summary>
        /// Set the uid of crawler's account
        /// </summary>
        /// <param name="__uid">The uid of crawler's account</param>
        public void SetCrawlerUID(uint __uid)
        {
            _uid_crawler = __uid;
        }

        /// <summary>
        /// Set the uid of center user (center vertex)
        /// </summary>
        /// <param name="__uid">The uid of center user</param>
        public void SetCenterUID(uint __uid)
        {
            _uid_center = __uid;
        }

        /// <summary>
        /// 
        /// </summary>
        public WeiboMainForm()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            new WeiboBrowser(this, WeiboWeb.BaseUri).ShowDialog();
        }

        /// <summary>
        /// Set the cookies of crawler
        /// </summary>
        /// <param name="__cookie">The crawler's cookies</param>
        public void SetCookie(string __cookie)
        {
            _cookie = __cookie;
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            


            WebClient client = new WebClient();
            client.Encoding = Encoding.UTF8;
            client.BaseAddress = WeiboWeb.GetFollowUri(_uid_center);            
            client.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/33.0.1750.146 Safari/537.36");
            client.Headers.Add(HttpRequestHeader.Cookie, _cookie2);
            Task<string> task = client.DownloadStringTaskAsync(WeiboWeb.GetFollowUri(1799920014));
            task.ContinueWith(_ =>
                {
                    MessageBox.Show(task.Result);
                });
        }

        private void httpClient()
        {
            throw new NotImplementedException();
        }

    }
}
