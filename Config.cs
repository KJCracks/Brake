using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Brake
{
    public partial class ConfigForm : Form
    {
        private static Container xml;

        public enum Platform
        {
            Windows,
            Linux,
            Mac
        }

        public static Platform RunningPlatform()
        {
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Unix:
                    // Well, there are chances MacOSX is reported as Unix instead of MacOSX.
                    // Instead of platform check, we'll do a feature checks (Mac specific root folders)
                    if (Directory.Exists("/Applications")
                        & Directory.Exists("/System")
                        & Directory.Exists("/Users")
                        & Directory.Exists("/Volumes"))
                        return Platform.Mac;
                    else
                        return Platform.Linux;

                case PlatformID.MacOSX:
                    return Platform.Mac;

                default:
                    return Platform.Windows;
            }
        }


        public ConfigForm()
        {
            InitializeComponent();
            this.MaximizeBox = false;
            xml = Brake.Container.getContainer();
            string location;
            switch (RunningPlatform())
            {
                case Platform.Mac:
                    {
                        location = Environment.GetEnvironmentVariable("HOME") + "/Music/iTunes/iTunes Media/Mobile Applications";
                        break;
                    }
                case Platform.Windows:
                    {
                        string location2 = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
                        location = Path.Combine(location2, "iTunes\\iTunes Media\\Mobile Applications");
                        break;
                    }
                default:
                    {
                        Console.WriteLine("Unknown operating system!");
                        return;
                    }
            }
            if (!Directory.Exists(location))
            {
                MessageBox.Show("Could not find default iTunes IPA location, please select it yourself!");
            }
            ipaDirectoryBox.Text = location;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void Config_Load(object sender, EventArgs e)
        {

        }

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                ipaDirectoryBox.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void doneButton_Click(object sender, EventArgs e)
        {
            xml.Config = new Configuration();
            xml.Config.host = host.Text;
            int.TryParse(port.Text, out xml.Config.port);
            xml.Config.Password = password.Text;
            xml.Config.ipaDir = ipaDirectoryBox.Text;
            xml.SaveXML();
            AppList appListForm = new AppList();
            appListForm.Show();
            this.Hide();
        }
    }
}
