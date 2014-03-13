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
        WeiboMainForm mainForm;
        uint uid;
        uint oid;

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
            mainForm = __callingForm as WeiboMainForm;
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
                uid = WeiboWeb.GetUID(webBrowser1.DocumentText);
                oid = WeiboWeb.GetOID(webBrowser1.DocumentText);
                tbOID.Text = oid.ToString();
                tbNick.Text = WeiboWeb.GetONick(webBrowser1.DocumentText);
                if (uid > 0)
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
            if (uid > 0)
            {
                mainForm.SetCrawlerUID(uid);
                mainForm.SetCenterUID(oid);
            }
            this.Close();
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
