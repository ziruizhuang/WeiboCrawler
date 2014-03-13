using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WeiboCrawler
{
    public partial class WeiboBrowser : Form
    {
        WeiboMainForm _mainForm;
        ulong _uid;
        ulong _oid;

        public WeiboBrowser()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Constructor of WeiboBrowser
        /// </summary>
        /// <param name="__callingForm">The calling form (should be an Instance of WeiboMainForm)</param>
        /// <param name="__uri">The default browser URI</param>
        public WeiboBrowser(Form __callingForm, string __uri)
        {
            _mainForm = __callingForm as WeiboMainForm;
            InitializeComponent();
            webBrowser1.Navigate(__uri);
            webBrowser1.DocumentCompleted += webBrowser1_DocumentCompleted;
            webBrowser1.NewWindow += webBrowser1_NewWindow;
            
        }

        void webBrowser1_NewWindow(object sender, CancelEventArgs e)
        {
            //throw new NotImplementedException();
            string url = ((WebBrowser)sender).Document.ActiveElement.GetAttribute("href");
            e.Cancel = true;
            ((WebBrowser)sender).Navigate(url);
        }

        void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            //throw new NotImplementedException();
            if (e.Url == webBrowser1.Document.Url)
            {
                _uid = WeiboWeb.GetUID(webBrowser1.DocumentText);
                _oid = WeiboWeb.GetOID(webBrowser1.DocumentText);
                tbOID.Text = _oid.ToString();
                tbNick.Text = WeiboWeb.GetONick(webBrowser1.DocumentText);
                if (_uid > 0)
                {
                    btnOk.Enabled = true;
                }
                else
                {
                    btnOk.Enabled = false;
                }
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (_uid > 0 && _oid > 0)
            {
                if (_uid != _oid)
                {
                    _mainForm.SetCrawlerUID(_uid);
                    _mainForm.SetCenterUID(_oid);
                    _mainForm.SetCookie(webBrowser1.Document.Cookie);
                    this.Close();
                }
                else
                {
                    MessageBox.Show
                    (
                        "Crawling is against Weibo's Term of Use. It's highly recommended that use a stand alone account for crawling to avoid any possible punishment or other unhappy events. \n"
                        + "At this point it's detected that the same user account is selected as center vertex and crawler. Please examine and fix it.", "Warning"
                    );
                }
            }
        }

        private void WeiboBrowser_Load(object sender, EventArgs e)
        {

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            //Using Navigate function to fire a DocumentCompleted event
            //Instead of Refresh function which just reload the document but not necessarily redownload the document
            webBrowser1.Navigate(webBrowser1.Url.ToString());
        }
    }

}
