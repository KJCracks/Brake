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
        private CrackProcess crackProcess;
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
        
        private void beginCrackProcess()
        {
            crackProcess.Show();
            crackThread = new Thread(new ThreadStart(crackProcess.runQueue));
            crackThread.Start();
            //cp.beginCracking();
        }
        private void crack(List<int> list)
        {
            if ((crackThread != null) && (crackThread.IsAlive))
            {
                MessageBox.Show("Already cracking an app, please wait!");
                return;
            }

            crackProcess = new CrackProcess();

            foreach (int tag in list)
            {
                IPAInfo info = xml.IPAItems[tag];
                Console.WriteLine("selected " + info.AppBundle + " " + info.Location);
                crackProcess.queueIPA(info);
            }
            beginCrackProcess();
        }
        private void crackButton_Click(object sender, EventArgs e)  
        {          
            List<int> list = listView1.SelectedItems.Cast<ListViewItem>().Select(x => (int) x.Tag).ToList();
            crack(list);
        }

        private void form_closing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void crackAllButton_Click(object sender, EventArgs e)
        {
            List<int> list = listView1.Items.Cast<ListViewItem>().Select(x => (int)x.Tag).ToList();
            crack(list);
        }
    }
}
