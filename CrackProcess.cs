using Ionic.Zip;
using Renci.SshNet;
using Renci.SshNet.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Brake
{
    public partial class CrackProcess : Form
    {
        private static Container xml;
        private Queue<IPAInfo> queue;
        private String DeviceTempDir;

        delegate void LogCallback(string text);
        PasswordConnectionInfo connectionInfo;
        SftpClient sftp;
        SshClient ssh;
        private void log(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (logBox.InvokeRequired)
            {
                LogCallback d = new LogCallback(log);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                logBox.AppendText(text + Environment.NewLine);
                Console.WriteLine("log: " + text);
            }
        }

        delegate void PercentStatusCallback(string text, int percent);

        private void PercentStatus(string text, int percent)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (logBox.InvokeRequired)
            {
                PercentStatusCallback d = new PercentStatusCallback(PercentStatus);
                this.Invoke(d, new object[] { text, percent });
            }
            else
            {
                statusLabel.Text = "status: " + text + " (" + percent + "%)";
                progressBar1.Value = percent;
            }
        }


        private void crackIPA(IPAInfo ipaInfo)
        {
            log("beginning cracking process..");
            PercentStatus("Establishing SSH connection", 5);
            PercentStatus("Establishing SFTP connection", 10);

            log("Cracking " + ipaInfo.AppName);
            PercentStatus("Preparing IPA", 25);
            /*String ipalocation = AppHelper.extractIPA(ipaInfo);
            using (var file = File.OpenRead(ipalocation))
            {
                log("Uploading IPA to device..");
                PercentStatus("Uploading IPA", 40);
                sftp.UploadFile(file, "Upload.ipa");

            }*/
            log("Cracking! (This might take a while)");
            PercentStatus("Cracking", 50);
            String binaryLocation = ipaInfo.BinaryLocation.Replace("Payload/", "");
            String TempDownloadBinary = Path.Combine(AppHelper.GetTemporaryDirectory(), "crackedBinary");
            var crack = ssh.RunCommand("Clutch -i '" + ipaInfo.IPALocationOnDevice +"' " + binaryLocation + " /tmp/crackedBinary");
            log("Clutch -i '" + ipaInfo.IPALocationOnDevice +"' " + binaryLocation + " /tmp/crackedBinary");
            log("cracking output: " + crack.Result);

            using (var file = File.OpenWrite(TempDownloadBinary))
            {
                log("Downloading cracked binary..");
                PercentStatus("Downloading cracked binary", 80);
                try
                {
                    sftp.DownloadFile("/tmp/crackedBinary", file);
                }
                catch (SftpPathNotFoundException e)
                {
                    log("Could not find file, help!!!!!");
                    return;
                }
            }

            PercentStatus("Repacking IPA", 90);
            String repack = AppHelper.repack(ipaInfo, TempDownloadBinary);
            PercentStatus("Done!", 100);

            log("Cracking completed, file at " + repack);
        }
        
        public CrackProcess()
        {
            xml = Brake.Container.getContainer();
            InitializeComponent();
            queue = new Queue<IPAInfo>();
            connectionInfo = new PasswordConnectionInfo(xml.Config.host, xml.Config.port, "root", xml.Config.Password);
            
        }

        public void queueIPA(IPAInfo ipaInfo)
        {
            queue.Enqueue(ipaInfo);
        }

        public void runQueue()
        {
            SetAppStatusText("Establishing SSH connection...");
            connectSSH();
            prepareIPAs();
            SetAppStatusBar(0);
            int i = 1;
            foreach (IPAInfo ipa in queue) {
                SetAppStatusText("Cracking app: " + ipa.AppName + " (" + i + "/" + queue.Count + ")");
                crackIPA(ipa);
                SetAppStatusBar((i* 100) / queue.Count);
                i++;
            }
            try
            {
                sftp.DeleteDirectory(DeviceTempDir);
            }
            catch (Exception e)
            {

            }
            ssh.Disconnect();
            sftp.Disconnect();
            AppHelper.DeleteDirectory(AppHelper.GetWorkingDirectory());
            MessageBox.Show("Cracking complete!");
            CloseWindow();
        }

        private void connectSSH()
        {
            sftp = new SftpClient(connectionInfo);
            sftp.Connect();
            ssh = new SshClient(connectionInfo);
            ssh.Connect();
        }

        public void prepareIPAs()
        {
            DeviceTempDir = "/tmp/Brake-" + RandString();
            try
            {
                sftp.CreateDirectory(DeviceTempDir);
            }
            catch
            {

            }
            using (ZipFile packagedIPAs = new ZipFile())
            {
                packagedIPAs.CompressionLevel = Ionic.Zlib.CompressionLevel.Level0;
                int i = 1;
                foreach (IPAInfo ipaInfo in queue)
                {
                    ipaInfo.IPALocation = AppHelper.extractIPA(ipaInfo);
                    ipaInfo.IPALocationOnDevice = DeviceTempDir + "/" + Path.GetFileName(ipaInfo.IPALocation);
                    Console.WriteLine("ipalocation on device " + ipaInfo.IPALocationOnDevice);
                    packagedIPAs.AddFile(ipaInfo.IPALocation, String.Empty);
                    Console.WriteLine("Added ipa into zip: " + ipaInfo.IPALocation);
                    SetAppStatusText("Preparing IPAs: " + ipaInfo.AppName + " (" + i + "/" + queue.Count + ")");
                    SetAppStatusBar((i * 100) / queue.Count);
                    i++;
                }
                packagedIPAs.Save("Brake-Upload.zip");
            }
            SetAppStatusText("Uploading IPAs..");
            try
            {   
                log("device temp dir: " + DeviceTempDir);

                using (var file = File.OpenRead("Brake-Upload.zip"))
                {
                    sftp.UploadFile(file, DeviceTempDir + "/Brake-Upload.zip");
                }
                ssh.RunCommand("cd '" + DeviceTempDir + "'; unzip Brake-Upload.zip");
                Console.WriteLine("ssh command: cd '" + DeviceTempDir + "'; unzip Brake-Upload.zip");
            }
            catch (Exception e)
            {
                Console.WriteLine("exception occured " + e.ToString());
            }
        }



        delegate void SetAppStatusCallback(string text);
        delegate void CloseWindowCallback();
        delegate void SetAppStatusBarCallback(int value);

        public static String RandString()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var result = new string(
                Enumerable.Repeat(chars, 8)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());
            return result;
        }

        private void SetAppStatusBar(int value)
        {
            // InvokeRequired required compares the thread ID of the 
            // calling thread to the thread ID of the creating thread. 
            // If these threads are different, it returns true. 
            if (this.appProgressBar.InvokeRequired)
            {
                SetAppStatusBarCallback d = new SetAppStatusBarCallback(SetAppStatusBar);
                this.Invoke(d, new object[] { value });
            }
            else
            {
                this.appProgressBar.Value = value;
                Console.WriteLine("progress bar value " + value);
            }
        }

        private void CloseWindow()
        {
            // InvokeRequired required compares the thread ID of the 
            // calling thread to the thread ID of the creating thread. 
            // If these threads are different, it returns true. 
            if (this.appProgressBar.InvokeRequired)
            {
                CloseWindowCallback d = new CloseWindowCallback(CloseWindow);
                this.Invoke(d);
            }
            else
            {
                this.Close();
            }
        }

        private void SetAppStatusText(string text)
        {
            // InvokeRequired required compares the thread ID of the 
            // calling thread to the thread ID of the creating thread. 
            // If these threads are different, it returns true. 
            if (this.appStatusLabel.InvokeRequired)
            {
                SetAppStatusCallback d = new SetAppStatusCallback(SetAppStatusText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.appStatusLabel.Text = text;
            }
        }

    }
}
