using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Brake
{
    public partial class AppList : Form
    {
        public Thread crackThread;
        private static Container xml;
        public AppList()
        {
            InitializeComponent();
            xml = Brake.Container.getContainer();
            AppHelper.getIPAs(xml.Config.ipaDir);
            var items = listView1.Items;
            int i = 0;
            foreach (IPAInfo info in xml.IPAItems)
            {
                ListViewItem item = new ListViewItem(info.AppName + " (" + info.AppVersion + ")");
                item.Tag = i;
                listView1.Items.Add(item);
                i++;
            }
            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        
        private void crackApp(IPAInfo ipaInfo)
        {
            CrackProcess cp = new CrackProcess(ipaInfo);
            cp.Show();
            crackThread = new Thread(new ThreadStart(cp.beginCracking));
            
            crackThread.Start();
            //cp.beginCracking();
        }

        private void crackButton_Click(object sender, EventArgs e)
        {
            if ((crackThread != null) && (crackThread.IsAlive))
            {
                MessageBox.Show("Already cracking an app, please wait!");
                return;
            }
            var selectedItems = listView1.SelectedItems;

            foreach (ListViewItem item in selectedItems)
            {
                IPAInfo info = xml.IPAItems[(int)item.Tag];
                Console.WriteLine("selected " + info.AppBundle + " " + info.Location);
                crackApp(info);
            }
        }

        private void form_closing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
