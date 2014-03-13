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
        public WeiboBrowser()
        {
            InitializeComponent();
        }
        public WeiboBrowser(Form __callingForm, string __uri)
        {
            mainForm = __callingForm as WeiboMainForm;
            InitializeComponent();
            webBrowser1.Navigate(__uri);
            webBrowser1.DocumentCompleted += webBrowser1_DocumentCompleted;
        }

        void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            //throw new NotImplementedException();
            if (e.Url == webBrowser1.Document.Url)
            {
                uid = WeiboWeb.GetOID(webBrowser1.DocumentText);
                tbOID.Text = uid.ToString();
                tbNick.Text = WeiboWeb.GetONick(webBrowser1.DocumentText);
                if (uid > 0)
                {
                    btnOk.Enabled = true;
                }
            }
        }
        private void btnOk_Click(object sender, EventArgs e)
        {
            if (uid > 0)
            {
                mainForm.SetUID(uid);
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
            webBrowser1.Refresh();
        }
    }

}
